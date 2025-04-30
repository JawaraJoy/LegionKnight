using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Purchasing;
using UnityEngine.Purchasing.Extension;

namespace LegionKnight
{
    public partial class IAPManager : InAppPurchase
    {
        
    }
    public partial class UnityService
    {
        [SerializeField]
        private IAPManager m_IAPManager;

        public void InitIAPManager()
        {
            m_IAPManager.Init();
        }
        public void OnPurchaseCompleted(Product product)
        {
            m_IAPManager.OnPurchaseCompleted(product);
        }

        public void OnPurchaseFailed(Product product, PurchaseFailureDescription reason)
        {
            m_IAPManager.OnPurchaseFailed(product, reason);
        }
        public void SetIsAvailablePurchase(string id, bool isAvailable)
        {
            m_IAPManager.SetIsAvailable(id, isAvailable);

        }
        public void SetIsBonusAvailablePurchase(string id, bool isBonusAvailable)
        {
            m_IAPManager.SetIsBonusAvailable(id, isBonusAvailable);

        }
        public bool IsProductAvailable(string id)
        {
            return m_IAPManager.IsProductAvailable(id);
        }
        public bool IsBonusAvailable(string id)
        {
            return m_IAPManager.IsBonusAvailable(id);
        }
        public void OnPurchasedCompleteAddListerner(UnityAction<SellProduct> callback)
        {
            m_IAPManager.OnPurchasedCompleteAddListerner(callback);
        }
        public void OnPurchasedFailedAddListerner(UnityAction<SellProduct> callback)
        {
            m_IAPManager.OnPurchasedFailedAddListerner(callback);
        }
        public void OnPurchasedCompleteRemoveListerner(UnityAction<SellProduct> callback)
        {
            m_IAPManager.OnPurchasedCompleteRemoveListerner(callback);
        }
        public void OnPurchasedFailedRemoveListerner(UnityAction<SellProduct> callback)
        {
            m_IAPManager.OnPurchasedFailedRemoveListerner(callback);
        }
    }
}
