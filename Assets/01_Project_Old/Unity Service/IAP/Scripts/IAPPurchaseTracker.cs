using LegionKnight;
using UnityEngine;

namespace Rush
{
    public class IAPPurchaseTracker : MonoBehaviour
    {
        private const string KeyPrefix = "IAPPurchased_";

        private string KeyFor(IAPBundleConfig bundle) =>
            KeyPrefix + bundle.BaseInfo.Id;

        public bool HasPurchased(IAPBundleConfig bundle) =>
            UnityService.Instance.HasData(KeyFor(bundle))
            && UnityService.Instance.GetData<bool>(KeyFor(bundle));

        public void MarkPurchased(IAPBundleConfig bundle) =>
            UnityService.Instance.SaveData(KeyFor(bundle), true);
    }
}