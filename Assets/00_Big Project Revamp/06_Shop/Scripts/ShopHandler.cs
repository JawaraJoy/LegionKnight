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
        [SerializeField] private UnityEvent<string> m_OnPurchaseFailed;

        public ShopConfig ShopConfig => m_ShopConfig;
        public UnityEvent<ShopConfirmData> OnPurchaseRequested => m_OnPurchaseRequested;
        public UnityEvent<CollectibleResultData> OnPurchaseComplete => m_OnPurchaseComplete;
        public UnityEvent<string> OnPurchaseFailed => m_OnPurchaseFailed;

        public void RequestPurchase(ShopBundleConfig bundle)
        {
            if (!ValidateBundleInternal(bundle)) return;

            var availability = GetAvailability(bundle);
            if (!availability.CanPurchase)
            {
                m_OnPurchaseFailed?.Invoke("Bundle tidak tersedia saat ini.");
                return;
            }

            var breakdown = m_CostResolver.CalculateBreakdown(
                bundle, GetCurrencyInternal(), availability.IsFirstPurchase);

            if (!breakdown.CanAfford)
            {
                m_OnPurchaseFailed?.Invoke("Mata uang tidak cukup.");
                return;
            }

            m_OnPurchaseRequested?.Invoke(new ShopConfirmData(bundle, breakdown));
        }

        public void ExecutePurchase(ShopBundleConfig bundle)
        {
            if (!ValidateBundleInternal(bundle)) return;

            var availability = GetAvailability(bundle);
            if (!availability.CanPurchase)
            {
                m_OnPurchaseFailed?.Invoke("Bundle tidak tersedia saat ini.");
                return;
            }

            var breakdown = m_CostResolver.CalculateBreakdown(
                bundle, GetCurrencyInternal(), availability.IsFirstPurchase);

            if (!breakdown.CanAfford)
            {
                m_OnPurchaseFailed?.Invoke("Mata uang tidak cukup.");
                return;
            }

            m_CostResolver.DeductCost(bundle, GetCurrencyInternal(), breakdown);
            m_PurchaseTracker.MarkPurchased(bundle);
            m_OnPurchaseComplete?.Invoke(BuildResultInternal(bundle));
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
            m_OnPurchaseFailed?.Invoke("Bundle tidak valid.");
            return false;
        }

        private CurrenciesControl GetCurrencyInternal() =>
            Player.Instance.CurrencyControl;
    }
}