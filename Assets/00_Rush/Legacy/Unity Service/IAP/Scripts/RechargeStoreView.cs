using System;
using LegionKnight;
using UnityEngine;
using UnityEngine.Purchasing;

namespace Rush
{
    // Shares store presentation state between the catalog and its detail dialog.
    public sealed class RechargeStoreView : MonoBehaviour
    {
        private IAPManager m_Store;
        private StoreController m_Controller;
        private bool m_Offline;
        private bool m_PurchaseInProgress;
        private bool m_Fetched;
        public bool Offline => m_Offline;
        public bool HasProducts => m_Fetched || (m_Store != null && m_Store.IsInitialized &&
            m_Controller != null && m_Controller.GetProducts().Count > 0);
        public bool PurchaseInProgress => m_PurchaseInProgress;
        public event Action Changed;

        private void Start() => BindInternal();
        public void Bind() => BindInternal();
        private void BindInternal()
        {
            if (m_Store != null) return;
            m_Store = UnityService.Instance.IAPManager;
            if (m_Store == null) return;
            m_Controller = UnityIAPServices.StoreController();
            m_Controller.OnPurchaseDeferred += OnDeferredInternal;
            m_Store.OnStoreConnected.AddListener(OnConnectedInternal);
            m_Store.OnStoreConnectFailed.AddListener(OnDisconnectedInternal);
            m_Store.OnPurchaseSuccess.AddListener(OnSuccessInternal);
            m_Store.OnPurchaseFailed.AddListener(OnFailedInternal);
        }

        public bool IsAvailable(IAPBundleConfig bundle) => !m_Offline && bundle != null &&
            m_Store != null && m_Store.IsInitialized &&
            m_Controller.GetProductById(bundle.ProductId) is { availableToPurchase: true } &&
            !string.IsNullOrWhiteSpace(m_Store.GetLocalizedPrice(bundle));

        public void RequestPurchase(IAPBundleConfig bundle)
        {
            if (m_PurchaseInProgress || !IsAvailable(bundle) || !m_Store.CanPurchase(bundle)) return;
            m_PurchaseInProgress = true;
            Changed?.Invoke();
            try { m_Store.Purchase(bundle); }
            catch (Exception ex)
            {
                m_PurchaseInProgress = false;
                Changed?.Invoke();
                Debug.LogException(ex, this);
            }
        }

        private void OnConnectedInternal()
        {
            m_Offline = false;
            m_Fetched = true;
            Changed?.Invoke();
        }

        private void OnDisconnectedInternal(string reason)
        {
            m_Offline = true;
            m_Fetched = false;
            FinishInternal();
        }

        private void OnSuccessInternal(IAPBundleConfig bundle) => FinishInternal();
        private void OnFailedInternal(IAPBundleConfig bundle, string reason) => FinishInternal();
        private void OnDeferredInternal(DeferredOrder order) => FinishInternal();
        private void FinishInternal()
        {
            m_PurchaseInProgress = false;
            Changed?.Invoke();
        }

        private void OnDestroy()
        {
            if (m_Controller != null) m_Controller.OnPurchaseDeferred -= OnDeferredInternal;
            if (m_Store == null) return;
            m_Store.OnStoreConnected.RemoveListener(OnConnectedInternal);
            m_Store.OnStoreConnectFailed.RemoveListener(OnDisconnectedInternal);
            m_Store.OnPurchaseSuccess.RemoveListener(OnSuccessInternal);
            m_Store.OnPurchaseFailed.RemoveListener(OnFailedInternal);
        }
    }
}
