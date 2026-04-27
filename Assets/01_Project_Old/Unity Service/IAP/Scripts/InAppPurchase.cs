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
        public bool IsInitialized => m_StoreController != null;
        public UnityEvent<Rush.IAPBundleConfig> OnPurchaseSuccess => m_OnPurchaseSuccess;
        public UnityEvent<Rush.CollectibleResultData> OnPurchaseComplete => m_OnPurchaseComplete;
        public UnityEvent<Rush.IAPBundleConfig, string> OnPurchaseFailed => m_OnPurchaseFailed;
        public UnityEvent OnStoreConnected => m_OnStoreConnected;
        public UnityEvent<string> OnStoreConnectFailed => m_OnStoreConnectFailed;

        // ── Init ──────────────────────────────────────────────────────────────

        protected virtual async void Start()
        {
            await InitializeInternal();
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

            var products = new List<ProductDefinition>();
            foreach (var tab in m_Catalog.Tabs)
            {
                if (tab?.Bundles == null) continue;
                foreach (var bundle in tab.Bundles)
                {
                    if (bundle == null || string.IsNullOrEmpty(bundle.ProductId)) continue;
                    products.Add(new ProductDefinition(bundle.ProductId, ProductType.NonConsumable));
                    m_ProductMap[bundle.ProductId] = bundle;
                }
            }

            await m_StoreController.Connect();

            m_StoreController.OnProductsFetched += OnProductsFetchedInternal;
            m_StoreController.OnProductsFetchFailed += OnProductsFetchFailedInternal;
            m_StoreController.FetchProducts(products);
        }

        // ── IAP v5 Event Handlers ─────────────────────────────────────────────

        private void OnProductsFetchedInternal(List<Product> products)
        {
            m_StoreController.OnPurchasesFetched += OnPurchasesFetchedInternal;
            m_StoreController.OnPurchasesFetchFailed += OnPurchasesFetchFailedInternal;
            m_StoreController.FetchPurchases();
            m_OnStoreConnected?.Invoke();
        }

        private void OnProductsFetchFailedInternal(ProductFetchFailed failure)
        {
            Debug.LogWarning($"[IAP] Products fetch failed: {failure.FailureReason}");
            m_OnStoreConnectFailed?.Invoke(failure.FailureReason.ToString());
        }

        private void OnPurchasesFetchedInternal(Orders orders)
        {
            // No action needed — entitlements already granted on first purchase
            // Override in subclass if restore flow needed
        }

        private void OnPurchasesFetchFailedInternal(PurchasesFetchFailureDescription failure)
        {
            Debug.LogWarning($"[IAP] Purchases fetch failed: {failure.message}");
        }

        private void OnPurchasePendingInternal(PendingOrder order)
        {
            var item = order.CartOrdered.Items()?.FirstOrDefault();

            if (item == null)
            {
                Debug.LogWarning("[IAP] No item in cart.");
                m_StoreController.ConfirmPurchase(order);
                return;
            }

            string productId = item.Product.definition.storeSpecificId;

            if (!m_ProductMap.TryGetValue(productId, out var bundle))
            {
                Debug.LogWarning($"[IAP] Unknown product: {productId}");
                m_StoreController.ConfirmPurchase(order);
                return;
            }

            GiveItemsInternal(bundle);
            m_OnPurchaseSuccess?.Invoke(bundle);
            m_StoreController.ConfirmPurchase(order);
        }

        private void OnPurchaseFailedInternal(FailedOrder order)
        {
            var item = order.CartOrdered.Items()?.FirstOrDefault();
            string productId = item?.Product.definition.storeSpecificId;
            m_ProductMap.TryGetValue(productId ?? string.Empty, out var bundle);
            string reason = order.FailureReason.ToString();
            Debug.LogWarning($"[IAP] Purchase failed: {productId} — {reason}");
            m_OnPurchaseFailed?.Invoke(bundle, reason);
        }

        private void OnStoreDisconnectedInternal(StoreConnectionFailureDescription failure)
        {
            Debug.LogWarning($"[IAP] Store disconnected: {failure.message}");
            m_OnStoreConnectFailed?.Invoke(failure.message);
        }

        // ── Purchase ──────────────────────────────────────────────────────────

        public void Purchase(Rush.IAPBundleConfig bundle)
        {
            if (!IsInitialized)
            {
                Debug.LogWarning("[IAP] Store not initialized yet.");
                return;
            }

            if (bundle == null || string.IsNullOrEmpty(bundle.ProductId)) return;

            // Check purchase limit before initiating store flow
            if (!m_PurchaseTracker.CanPurchase(bundle))
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
                return;
            }

            Product product = m_StoreController.GetProductById(bundle.ProductId);

            if (product == null)
            {
                Debug.LogWarning($"[IAP] Product not found: {bundle.ProductId}");
                return;
            }

            m_StoreController.PurchaseProduct(product);
        }

        // ── Items ─────────────────────────────────────────────────────────────

        private void GiveItemsInternal(Rush.IAPBundleConfig bundle)
        {
            var result = new Rush.CollectibleResultData();
            // Use IsFirstPurchase (not HasPurchased) so bonus works correctly
            // regardless of purchase limit type
            bool isFirst = m_PurchaseTracker.IsFirstPurchase(bundle);

            if (bundle.Entries != null)
            {
                foreach (var entry in bundle.Entries)
                {
                    m_CollectibleControl?.AddCollectible(entry.Collectible, entry.Amount);
                    result.AddEntry(entry.Collectible, entry.Amount);
                }
            }

            if (isFirst && bundle.HasFirstPurchaseBonus)
            {
                foreach (var entry in bundle.FirstPurchaseBonusEntries)
                {
                    m_CollectibleControl?.AddCollectible(entry.Collectible, entry.Amount);
                    result.AddEntry(entry.Collectible, entry.Amount);
                }
            }

            m_PurchaseTracker.MarkPurchased(bundle);
            m_OnPurchaseComplete?.Invoke(result);
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
            m_PurchaseTracker.IsFirstPurchase(bundle);

        public bool CanPurchase(Rush.IAPBundleConfig bundle) =>
            m_PurchaseTracker.CanPurchase(bundle);

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
    }
}