using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.Purchasing.Extension;

namespace LegionKnight
{
    public partial class InAppPurchaseAgent : MonoBehaviour
    {
        public void OnPurchaseCompleted(Product product)
        {
            UnityService.Instance.OnPurchaseCompleted(product);
        }

        public void OnPurchaseFailed(Product product, PurchaseFailureDescription reason)
        {
            UnityService.Instance.OnPurchaseFailed(product, reason);
        }
        public void SetIsAvailable(string id, bool isAvailable)
        {
            UnityService.Instance.SetIsAvailable(id, isAvailable);

        }
        public void SetIsBonusAvailable(string id, bool isBonusAvailable)
        {
            UnityService.Instance.SetIsBonusAvailable(id, isBonusAvailable);
        }
    }
}
