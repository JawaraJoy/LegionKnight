using UnityEngine;
using UnityEngine.Events;
using LegionKnight;

namespace Rush
{
    public class ShopHandler : MonoBehaviour
    {
        [SerializeField] private ShopConfig m_ShopConfig;
        [SerializeField] private ShopCostResolver m_CostResolver;
        [SerializeField] private ShopPurchaseTracker m_PurchaseTracker;
        [SerializeField] private CollectibleControl m_CollectibleControl;

        [SerializeField] private UnityEvent<ShopConfirmData> m_OnPurchaseRequested;
        [SerializeField] private UnityEvent<CollectibleResultData> m_OnPurchaseComplete;
        [SerializeField] private UnityEvent<ShopBundleConfig> m_OnPurchaseCompleteBundle;
        [SerializeField] private UnityEvent<string> m_OnPurchaseFailed;

        public ShopConfig ShopConfig => m_ShopConfig;
        public UnityEvent<ShopConfirmData> OnPurchaseRequested => m_OnPurchaseRequested;
        public UnityEvent<CollectibleResultData> OnPurchaseComplete => m_OnPurchaseComplete;
        public UnityEvent<ShopBundleConfig> OnPurchaseCompleteBundle => m_OnPurchaseCompleteBundle;
        public UnityEvent<string> OnPurchaseFailed => m_OnPurchaseFailed;

        public void RequestPurchase(ShopBundleConfig bundle)
        {
            if (!ValidateBundleInternal(bundle)) return;

            var availability = GetAvailability(bundle);
            if (!availability.CanPurchase)
            {
                OnPurchaseFailedInvoke("Bundle tidak tersedia saat ini.");
                return;
            }

            var breakdown = m_CostResolver.CalculateBreakdown(
                bundle, GetCurrencyInternal(), availability.IsFirstPurchase);

            if (!breakdown.CanAfford)
            {
                OnPurchaseFailedInvoke("Mata uang tidak cukup.");
                return;
            }

            OnPurchaseRequestedInvoke(new ShopConfirmData(bundle, breakdown));
        }

        public void ExecutePurchase(ShopBundleConfig bundle)
        {
            if (!ValidateBundleInternal(bundle)) return;

            var availability = GetAvailability(bundle);
            if (!availability.CanPurchase)
            {
                OnPurchaseFailedInvoke("Bundle tidak tersedia saat ini.");
                return;
            }

            var breakdown = m_CostResolver.CalculateBreakdown(
                bundle, GetCurrencyInternal(), availability.IsFirstPurchase);

            if (!breakdown.CanAfford)
            {
                OnPurchaseFailedInvoke("Mata uang tidak cukup.");
                return;
            }

            var result = BuildResultInternal(bundle);
            UnityService.Instance.LocalSave.ExecuteTransaction(() =>
            {
                m_CostResolver.DeductCost(bundle, GetCurrencyInternal(), breakdown);
                result.GrantRewards(bundle.BaseInfo.Id);
                m_PurchaseTracker.MarkPurchased(bundle);
            });
            OnPurchaseCompleteInvoke(result);

            // Event terpisah untuk refresh detail panel tanpa harus tahu result
            OnPurchaseCompleteBundleInvoke(bundle);
        }

        public ShopBundleAvailability GetAvailability(ShopBundleConfig bundle)
        {
            bool canPurchase = m_PurchaseTracker.CanPurchase(bundle);
            bool isFirst = m_PurchaseTracker.IsFirstPurchase(bundle);
            double resetSeconds = m_PurchaseTracker.GetDailyResetSecondsRemaining(bundle);
            return new ShopBundleAvailability(
                canPurchase, isFirst, resetSeconds, bundle.PurchaseLimit);
        }

        public ShopCostBreakdown GetBreakdown(ShopBundleConfig bundle)
        {
            bool isFirst = m_PurchaseTracker.IsFirstPurchase(bundle);
            return m_CostResolver.CalculateBreakdown(
                bundle, GetCurrencyInternal(), isFirst);
        }

        private CollectibleResultData BuildResultInternal(ShopBundleConfig bundle)
        {
            var result = new CollectibleResultData();
            if (bundle.Entries == null) return result;
            foreach (var entry in bundle.Entries)
                result.AddEntry(entry.Collectible, entry.Amount);
            return result;
        }

        private bool ValidateBundleInternal(ShopBundleConfig bundle)
        {
            if (bundle != null) return true;
            OnPurchaseFailedInvoke("Bundle tidak valid.");
            return false;
        }

        private CurrenciesControl GetCurrencyInternal() =>
            Player.Instance.CurrencyControl;
        private void OnPurchaseRequestedInvoke(ShopConfirmData value0)
        {
            m_OnPurchaseRequested?.Invoke(value0);
        }

        private void OnPurchaseCompleteInvoke(CollectibleResultData value0)
        {
            m_OnPurchaseComplete?.Invoke(value0);
        }

        private void OnPurchaseCompleteBundleInvoke(ShopBundleConfig value0)
        {
            m_OnPurchaseCompleteBundle?.Invoke(value0);
        }

        private void OnPurchaseFailedInvoke(string value0)
        {
            m_OnPurchaseFailed?.Invoke(value0);
        }
    }
}