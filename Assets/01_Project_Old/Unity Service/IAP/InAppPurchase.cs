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

        // IAP v5 — StoreController replaces IStoreController + IDetailedStoreListener
        private StoreController m_StoreController;

        // Quick lookup: productId → config
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

            // v5: Get StoreController from UnityIAPServices
            m_StoreController = UnityIAPServices.StoreController();

            // Attach purchase handlers BEFORE connecting
            m_StoreController.OnPurchasePending += OnPurchasePendingInternal;
            m_StoreController.OnPurchaseFailed += OnPurchaseFailedInternal;
            m_StoreController.OnStoreDisconnected += OnStoreDisconnectedInternal;

            // Build product map and product list from catalog
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

            // v5: Connect async — replaces UnityPurchasing.Initialize()
            await m_StoreController.Connect();

            // v5: Fetch products — replaces ConfigurationBuilder.AddProduct()
            m_StoreController.OnProductsFetched += OnProductsFetchedInternal;
            m_StoreController.OnProductsFetchFailed += OnProductsFetchFailedInternal;
            m_StoreController.FetchProducts(products);
        }

        // ── IAP v5 Event Handlers ─────────────────────────────────────────────

        private void OnProductsFetchedInternal(List<Product> products)
        {
            // Products are ready — fetch existing purchases to restore entitlements
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

        // Called for restored purchases on FetchPurchases
        private void OnPurchasesFetchedInternal(Orders orders)
        {
            // No action needed — entitlements already granted on first purchase
            // Override in subclass if restore flow needed
        }

        private void OnPurchasesFetchFailedInternal(PurchasesFetchFailureDescription failure)
        {
            Debug.LogWarning($"[IAP] Purchases fetch failed: {failure.message}");
        }

        // v5 equivalent of ProcessPurchase — called for new purchases
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

            // v5: Must explicitly confirm purchase
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
            bool isFirst = !m_PurchaseTracker.HasPurchased(bundle);

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

            // v5: GetProducts() returns List<Product>
            var products = m_StoreController.GetProducts();
            foreach (var p in products)
            {
                if (p.definition.id == bundle.ProductId)
                    return p.metadata.localizedPriceString;
            }
            return string.Empty;
        }

        public bool IsFirstPurchase(Rush.IAPBundleConfig bundle) =>
            !m_PurchaseTracker.HasPurchased(bundle);

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