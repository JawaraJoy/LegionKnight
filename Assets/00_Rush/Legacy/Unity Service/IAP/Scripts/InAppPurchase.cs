using Rush;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Purchasing;

namespace LegionKnight
{
    public partial class InAppPurchase : MonoBehaviour
    {
        [SerializeField] private Rush.IAPCatalogConfig m_Catalog;
        [SerializeField] private Rush.IAPPurchaseTracker m_PurchaseTracker;
        [SerializeField] private Rush.CollectibleControl m_CollectibleControl;

        [SerializeField] private UnityEvent<Rush.IAPBundleConfig> m_OnPurchaseSuccess;
        [SerializeField] private UnityEvent<Rush.CollectibleResultData> m_OnPurchaseComplete;
        [SerializeField] private UnityEvent<Rush.IAPBundleConfig, string> m_OnPurchaseFailed;
        [SerializeField] private UnityEvent m_OnStoreConnected;
        [SerializeField] private UnityEvent<string> m_OnStoreConnectFailed;

        private StoreController m_StoreController;

        private readonly Dictionary<string, Rush.IAPBundleConfig> m_ProductMap = new();

        public Rush.IAPCatalogConfig Catalog => m_Catalog;
        private readonly IAPAccountHistory m_History = new();
        private readonly SemaphoreSlim m_Settlement = new(1, 1);
        private PaidDiamondWallet m_AccountWallet;
        private Unity.Services.Authentication.IAuthenticationService m_Auth;
        private string m_WalletAccount;
        private bool m_WalletLoading;
        private float m_NextWalletSync;
        private bool m_Ready;
        private bool m_RetryScheduled;
        public bool IsInitialized => m_Ready && m_History.IsCurrent;
        public UnityEvent<Rush.IAPBundleConfig> OnPurchaseSuccess => m_OnPurchaseSuccess;
        public UnityEvent<Rush.CollectibleResultData> OnPurchaseComplete => m_OnPurchaseComplete;
        public UnityEvent<Rush.IAPBundleConfig, string> OnPurchaseFailed => m_OnPurchaseFailed;
        public UnityEvent OnStoreConnected => m_OnStoreConnected;
        public UnityEvent<string> OnStoreConnectFailed => m_OnStoreConnectFailed;

        // ── Init ──────────────────────────────────────────────────────────────

        protected virtual async void Start()
        {
            m_AccountWallet = new PaidDiamondWallet(m_History, m_Settlement);
            try { await InitializeInternal(); }
            catch (Exception ex) { OnStoreConnectFailedInvoke("Store initialization failed: " + ex.GetType().Name); }
        }

        private async System.Threading.Tasks.Task InitializeInternal()
        {
            if (m_Catalog == null)
            {
                Debug.LogWarning("[IAP] No catalog assigned.");
                return;
            }

            m_StoreController = UnityIAPServices.StoreController();

            m_StoreController.OnPurchasePending += OnPurchasePendingInternal;
            m_StoreController.OnPurchaseFailed += OnPurchaseFailedInternal;
            m_StoreController.OnStoreDisconnected += OnStoreDisconnectedInternal;

            m_StoreController.ProcessPendingOrdersOnPurchasesFetched(false);
            m_StoreController.OnProductsFetched += OnProductsFetchedInternal;
            m_StoreController.OnProductsFetchFailed += OnProductsFetchFailedInternal;
            m_StoreController.OnPurchasesFetched += OnPurchasesFetchedInternal;
            m_StoreController.OnPurchasesFetchFailed += OnPurchasesFetchFailedInternal;
            var definitions = ProductCatalog.LoadDefaultCatalog().allProducts.ToDictionary(p => p.id);
            var products = new List<ProductDefinition>();
            foreach (var tab in m_Catalog.Tabs)
            {
                if (tab?.Bundles == null) continue;
                foreach (var bundle in tab.Bundles)
                {
                    if (bundle == null || string.IsNullOrEmpty(bundle.ProductId)) continue;
                    if (m_ProductMap.ContainsKey(bundle.ProductId)) continue;
                    var type = bundle.PurchaseLimit == ShopBundlePurchaseLimit.OneTime ?
                        ProductType.NonConsumable : ProductType.Consumable;
                    if (!definitions.TryGetValue(bundle.ProductId, out var definition) || definition.type != type)
                        throw new InvalidOperationException("IAP catalogs disagree for " + bundle.ProductId);
                    string listing = definition.GetStoreID(DefaultStoreHelper.GetDefaultStoreName());
                    products.Add(new ProductDefinition(bundle.ProductId,
                        string.IsNullOrEmpty(listing) ? bundle.ProductId : listing, type));
                    m_ProductMap[bundle.ProductId] = bundle;
                }
            }

            await m_StoreController.Connect();
            m_StoreController.FetchProducts(products);
        }

        // ── IAP v5 Event Handlers ─────────────────────────────────────────────

        private void OnProductsFetchedInternal(List<Product> products)
        {
            m_StoreController.FetchPurchases();
        }

        private void OnProductsFetchFailedInternal(ProductFetchFailed failure)
        {
            Debug.LogWarning($"[IAP] Products fetch failed: {failure.FailureReason}");
            OnStoreConnectFailedInvoke(failure.FailureReason.ToString());
        }

        private void Update()
        {
            if (!m_History.UsesCloud ||
                Unity.Services.Core.UnityServices.State != Unity.Services.Core.ServicesInitializationState.Initialized) return;
            m_Auth ??= Unity.Services.Authentication.AuthenticationService.Instance;
            string id = m_Auth.IsSignedIn ? m_Auth.PlayerId : null;
            bool changed = id != m_WalletAccount;
            if (changed)
            {
                m_WalletAccount = id;
                m_Ready = false;
                m_WalletLoaded = false;
                m_NextWalletSync = 0;
                Player.CurrentWallet?.ShowPaidDiamonds(m_AccountWallet);
            }
            if (id != null && Player.CurrentWallet != null && !m_WalletLoading && Time.unscaledTime >= m_NextWalletSync)
                SyncWalletInternal(id, changed);
        }

        // Login and wallet restoration are independent of a successful Play Store connection.
        private async void SyncWalletInternal(string id, bool refreshOrders)
        {
            m_WalletLoading = true;
            m_NextWalletSync = Time.unscaledTime + 60f;
            await m_Settlement.WaitAsync();
            try
            {
                if (!this || m_Auth.PlayerId != id || !m_Auth.IsSignedIn) return;
                if (m_History.AccountId != id || !m_WalletLoaded)
                    await RestoreAccountInternal();
                else
                {
                    await m_History.RefreshBalanceAsync();
                    Player.CurrentWallet?.ShowPaidDiamonds(m_AccountWallet);
                }
                if (refreshOrders && m_StoreController?.GetProducts().Count > 0) m_StoreController.FetchPurchases();
            }
            catch (Exception ex)
            {
                m_NextWalletSync = Time.unscaledTime + 10f;
                Debug.LogWarning("[IAP] Wallet sync unavailable: " + ex.GetType().Name);
            }
            finally { m_WalletLoading = false; m_Settlement.Release(); }
        }

        private bool m_WalletLoaded;
        private async Task RestoreAccountInternal()
        {
            if (m_History.UsesCloud && Player.CurrentWallet == null) throw new InvalidOperationException("Waiting for the player wallet.");
            m_WalletLoaded = false;
            await m_History.LoadAsync(m_ProductMap.Values, m_PurchaseTracker);
            if (m_History.UsesCloud)
            {
                var wallet = Player.CurrentWallet;
                if (wallet == null) throw new InvalidOperationException("Waiting for the player wallet.");
                await m_AccountWallet.RestoreAsync(wallet, UnityService.Instance.LocalSave);
            }
            m_WalletLoaded = true;
        }

        private async void OnPurchasesFetchedInternal(Orders orders)
        {
            await m_Settlement.WaitAsync();
            try
            {
                await RestoreAccountInternal();
                foreach (var confirmed in orders.ConfirmedOrders)
                {
                    // Acknowledged one-time entitlements restore ownership without regranting their wallet balance.
                    var known = confirmed.CartOrdered.Items().Where(i =>
                        i?.Product?.definition != null && m_ProductMap.ContainsKey(i.Product.definition.id)).ToArray();
                    if (known.Length == 0) continue;
                    var paid = IAPPaidOrder.Read(confirmed, m_ProductMap);
                    if (!paid.BelongsTo(m_History.AccountId)) continue;
                    await DeliverInternal(paid, true);
                }
                foreach (var pending in orders.PendingOrders)
                    await ProcessPendingInternal(pending);
                m_Ready = true;
                OnStoreConnectedInvoke();
            }
            catch (Exception ex)
            {
                m_Ready = false;
                OnStoreConnectFailedInvoke("Purchase history unavailable: " + ex.GetType().Name);
                RetryHistoryInternal();
                return;
            }
            finally { m_Settlement.Release(); }
        }

        private void OnPurchasesFetchFailedInternal(PurchasesFetchFailureDescription failure)
        {
            m_Ready = false;
            OnStoreConnectFailedInvoke(failure.message);
            RetryHistoryInternal();
        }

        private async void RetryHistoryInternal()
        {
            if (m_RetryScheduled) return;
            m_RetryScheduled = true;
            await Task.Delay(TimeSpan.FromSeconds(20));
            if (!this) return;
            m_RetryScheduled = false;
            m_StoreController.FetchPurchases();
        }

        private void OnApplicationFocus(bool focus)
        {
            if (focus && m_StoreController != null && m_StoreController.GetProducts().Count > 0)
                m_StoreController.FetchPurchases();
        }

        private async void OnPurchasePendingInternal(PendingOrder order)
        {
            await m_Settlement.WaitAsync();
            try
            {
                if (!IsInitialized) { RetryHistoryInternal(); return; }
                await ProcessPendingInternal(order);
            }
            finally { m_Settlement.Release(); }
        }

        private async Task ProcessPendingInternal(PendingOrder order)
        {
            try
            {
                var paid = IAPPaidOrder.Read(order, m_ProductMap);
                if (!paid.BelongsTo(m_History.AccountId))
                {
                    OnPurchaseFailedInvoke(null, "Sign in to the account used for this purchase.");
                    return;
                }
                var result = await DeliverInternal(paid, paid.PreviouslyAcknowledged);
                m_StoreController.ConfirmPurchase(order);
                if (result != null) OnPurchaseCompleteInvoke(result);
                OnPurchaseSuccessInvoke(paid.Units.Keys.First());
            }
            catch (Exception ex)
            {
                Debug.LogWarning("[IAP] Pending delivery: " + ex.GetType().Name);
                OnPurchaseFailedInvoke(null, "Your purchase is pending delivery.");
                RetryHistoryInternal();
            }
        }

        private void OnPurchaseFailedInternal(FailedOrder order)
        {
            var item = order.CartOrdered.Items()?.FirstOrDefault();
            string productId = item?.Product?.definition?.id;
            m_ProductMap.TryGetValue(productId ?? string.Empty, out var bundle);
            string reason = order.FailureReason.ToString();
            Debug.LogWarning($"[IAP] Purchase failed: {productId} — {reason}");
            OnPurchaseFailedInvoke(bundle, reason);
        }

        private void OnStoreDisconnectedInternal(StoreConnectionFailureDescription failure)
        {
            m_Ready = false;
            Debug.LogWarning($"[IAP] Store disconnected: {failure.message}");
            OnStoreConnectFailedInvoke(failure.message);
        }

        // ── Purchase ──────────────────────────────────────────────────────────

        public void Purchase(Rush.IAPBundleConfig bundle)
        {
            if (!IsInitialized)
            {
                OnPurchaseFailedInvoke(bundle, "The store is still loading.");
                if (m_StoreController != null && !m_History.IsCurrent) RetryHistoryInternal();
                return;
            }

            if (bundle == null || string.IsNullOrEmpty(bundle.ProductId)) return;

            // Check purchase limit before initiating store flow
            if (!CanPurchase(bundle))
            {
                string message = bundle.PurchaseLimit switch
                {
                    Rush.ShopBundlePurchaseLimit.OneTime =>
                        "This bundle can only be purchased once.",
                    Rush.ShopBundlePurchaseLimit.Daily =>
                        "This bundle can only be purchased once per day. Come back tomorrow!",
                    _ => "This bundle is not available right now."
                };

                var popup = LegionKnight.CanvasManager.Instance.GetPanel<TextPopUpPanel>();
                popup?.ShowText(message);
                OnPurchaseFailedInvoke(bundle, message);
                return;
            }

            Product product = m_StoreController.GetProductById(bundle.ProductId);

            if (product == null || !product.availableToPurchase)
            {
                OnPurchaseFailedInvoke(bundle, "This product is unavailable.");
                return;
            }

            if (Application.platform == RuntimePlatform.Android)
                m_StoreController.GooglePlayStoreExtendedService?.SetObfuscatedAccountId(IAPPaidOrder.Hash(m_History.AccountId));
            m_StoreController.PurchaseProduct(product);
        }

        // ── Items ─────────────────────────────────────────────────────────────

        private async Task<CollectibleResultData> DeliverInternal(IAPPaidOrder paid, bool acknowledged)
        {
            var save = UnityService.Instance.LocalSave;
            string key = "IAPDelivered_" + paid.Digest;
            bool localDone = save.GetDataValue<bool>(key);
            bool legacy = save.GetDataValue<bool>("IAPTransaction_" + paid.TransactionId);
            var skus = paid.Units.Keys.Select(b => b.ProductId).ToArray();
            if (localDone)
            {
                foreach (var b in paid.Units.Keys) await m_History.RememberAsync(b);
                return null;
            }

            // Older builds delivered one unit before acknowledgement, even on a multi-unit order.
            bool priorUnit = legacy || acknowledged;
            if (priorUnit && paid.Units.Count == 1 && paid.Units.Values.First() == 1)
            {
                var owned = paid.Units.Keys.First();
                await m_History.RememberAsync(owned);
                save.ExecuteTransaction(() => m_PurchaseTracker.MarkPurchased(owned));
                return null;
            }
            bool firstLine = true;
            var cloudRewards = new Dictionary<string, (int units, int bonus)>();
            ItemConfig diamond = null;
            var totals = new Dictionary<ItemConfig, int>();
            foreach (var line in paid.Units)
            {
                var bundle = line.Key;
                int units = line.Value - (priorUnit && firstLine ? 1 : 0);
                firstLine = false;
                if (bundle.Entries == null || bundle.Entries.Length == 0)
                    throw new InvalidOperationException("Missing contents for " + bundle.ProductId);
                if (m_History.UsesCloud)
                {
                    var item = bundle.Entries[0]?.Collectible as ItemConfig;
                    if (!PaidDiamondWallet.IsDiamond(item)) throw new InvalidOperationException("Missing Recharge Diamonds.");
                    diamond = item;
                    int amount = DiamondTotal(bundle.Entries, units);
                    int bonus = !priorUnit && bundle.HasFirstPurchaseBonus ? DiamondTotal(bundle.FirstPurchaseBonusEntries, 1) : 0;
                    cloudRewards.Add(bundle.ProductId, (amount, bonus));
                }
                else
                {
                    if (units > 0) AddCurrencyEntries(totals, bundle.Entries, units);
                    if (!priorUnit && m_PurchaseTracker.IsFirstPurchase(bundle) && bundle.HasFirstPurchaseBonus)
                        AddCurrencyEntries(totals, bundle.FirstPurchaseBonusEntries, 1);
                }
            }
            if (m_History.UsesCloud)
            {
                int credit = await m_History.CreditAsync(paid.Digest, cloudRewards);
                Player.Instance.CurrencyControl.ShowPaidDiamonds(m_AccountWallet);
                if (credit > 0) totals.Add(diamond, credit);
            }
            var result = new CollectibleResultData();
            string source = string.Join(",", skus);
            save.ExecuteTransaction(() =>
            {
                foreach (var credit in totals)
                {
                    if (!m_History.UsesCloud) Player.Instance.CurrencyControl.StagePurchaseCredit(credit.Key, credit.Value);
                    result.AddEntry(credit.Key, credit.Value);
                }
                foreach (var bundle in paid.Units.Keys) m_PurchaseTracker.MarkPurchased(bundle);
                save.SaveData(key + "_player", m_History.AccountId);
                save.SaveData(key, true);
                if (!save.GetDataValue<bool>(key))
                    throw new InvalidOperationException("Local purchase persistence is unavailable.");
            });
            result.MarkPurchaseDelivered(source);
            foreach (var credit in totals)
            {
                try { credit.Key.OnCollect(source, credit.Value); }
                catch (Exception ex) { Debug.LogWarning("[IAP] Analytics unavailable: " + ex.GetType().Name); }
            }
            return totals.Count == 0 ? null : result;
        }

        private static int DiamondTotal(IAPBundleEntry[] entries, int units)
        {
            int total = 0;
            foreach (var entry in entries)
            {
                if (!PaidDiamondWallet.IsDiamond(entry?.Collectible as ItemConfig) || entry.Amount <= 0)
                    throw new InvalidOperationException("Invalid Diamond offer.");
                total = checked(total + entry.Amount * units);
            }
            return total;
        }

        private static void AddCurrencyEntries(Dictionary<ItemConfig, int> totals, IAPBundleEntry[] entries, int units)
        {
            foreach (var entry in entries)
            {
                if (entry?.Collectible is not ItemConfig item || entry.Amount < 1)
                    throw new InvalidOperationException("Invalid recharge reward.");
                totals.TryGetValue(item, out int previous);
                totals[item] = checked(previous + entry.Amount * units);
            }
        }

        // ── Product info helpers ──────────────────────────────────────────────

        public string GetLocalizedPrice(Rush.IAPBundleConfig bundle)
        {
            if (!IsInitialized || bundle == null) return string.Empty;
            var products = m_StoreController.GetProducts();
            foreach (var p in products)
            {
                if (p.definition.id == bundle.ProductId)
                    return p.metadata.localizedPriceString;
            }
            return string.Empty;
        }

        // Kept for backward compatibility — delegates to tracker
        public bool IsFirstPurchase(Rush.IAPBundleConfig bundle) =>
            (Application.isEditor || IsInitialized) && !m_History.HasPurchased(bundle.ProductId) && m_PurchaseTracker.IsFirstPurchase(bundle);

        public bool CanPurchase(Rush.IAPBundleConfig bundle) =>
            m_PurchaseTracker.CanPurchase(bundle) &&
            (bundle.PurchaseLimit != ShopBundlePurchaseLimit.OneTime || !m_History.HasPurchased(bundle.ProductId));

        // ── Cleanup ───────────────────────────────────────────────────────────

        private void OnDestroy()
        {
            if (m_StoreController == null) return;
            m_StoreController.OnPurchasePending -= OnPurchasePendingInternal;
            m_StoreController.OnPurchaseFailed -= OnPurchaseFailedInternal;
            m_StoreController.OnStoreDisconnected -= OnStoreDisconnectedInternal;
            m_StoreController.OnProductsFetched -= OnProductsFetchedInternal;
            m_StoreController.OnProductsFetchFailed -= OnProductsFetchFailedInternal;
            m_StoreController.OnPurchasesFetched -= OnPurchasesFetchedInternal;
            m_StoreController.OnPurchasesFetchFailed -= OnPurchasesFetchFailedInternal;
        }
        private void OnPurchaseSuccessInvoke(Rush.IAPBundleConfig value0)
        {
            m_OnPurchaseSuccess?.Invoke(value0);
        }

        private void OnPurchaseCompleteInvoke(Rush.CollectibleResultData value0)
        {
            m_OnPurchaseComplete?.Invoke(value0);
        }

        private void OnPurchaseFailedInvoke(Rush.IAPBundleConfig value0, string value1)
        {
            m_OnPurchaseFailed?.Invoke(value0, value1);
        }

        private void OnStoreConnectedInvoke()
        {
            m_OnStoreConnected?.Invoke();
        }

        private void OnStoreConnectFailedInvoke(string value0)
        {
            m_OnStoreConnectFailed?.Invoke(value0);
        }
    }
}

