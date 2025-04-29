using UnityEngine;
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

        public void OnPurchaseCompleted(Product product)
        {
            m_IAPManager.OnPurchaseCompleted(product);
        }

        public void OnPurchaseFailed(Product product, PurchaseFailureDescription reason)
        {
            m_IAPManager.OnPurchaseFailed(product, reason);
        }
        public void SetIsAvailable(string id, bool isAvailable)
        {
            m_IAPManager.SetIsAvailable(id, isAvailable);

        }
        public void SetIsBonusAvailable(string id, bool isBonusAvailable)
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
    }
}
