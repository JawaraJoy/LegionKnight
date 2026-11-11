using UnityEngine;
using LegionKnight;

namespace Rush
{
    public class ShopCostResolver : MonoBehaviour
    {
        public int CalculateFinalPrice(ShopBundleConfig bundle, bool isFirstPurchase)
        {
            float discount = CalculateDiscountInternal(bundle, isFirstPurchase);
            return Mathf.Max(0, Mathf.RoundToInt(bundle.BasePrice * (1f - discount)));
        }

        public ShopCostBreakdown CalculateBreakdown(ShopBundleConfig bundle,
            CurrenciesControl currencyControl, bool isFirstPurchase)
        {
            var breakdown = new ShopCostBreakdown();

            float discount = CalculateDiscountInternal(bundle, isFirstPurchase);
            bool hasDiscount = discount > 0f;
            int finalPrice = Mathf.Max(0, Mathf.RoundToInt(bundle.BasePrice * (1f - discount)));

            breakdown.SetOriginalPrice(bundle.BasePrice);
            breakdown.SetHasDiscount(hasDiscount);
            breakdown.SetFirstPurchaseDiscount(
                isFirstPurchase && bundle.FirstPurchaseDiscount > 0f
                && bundle.BasePrice > bundle.MinimumPriceForDiscount);

            if (finalPrice <= 0)
            {
                breakdown.SetFree(true);
                breakdown.SetMain(0);
                breakdown.SetCanAfford(true);
                return breakdown;
            }

            breakdown.SetFree(false);

            int held = bundle.CostCurrency != null
                ? currencyControl.GetCurrencyAmount(bundle.CostCurrency) : 0;

            breakdown.SetMain(finalPrice);
            breakdown.SetCanAfford(held >= finalPrice);
            return breakdown;
        }

        public void DeductCost(ShopBundleConfig bundle,
            CurrenciesControl currencyControl, ShopCostBreakdown breakdown)
        {
            if (!breakdown.CanAfford || breakdown.IsFree) return;
            if (bundle.CostCurrency == null) return;
            currencyControl.RemoveCurrencyAmount(bundle.CostCurrency, breakdown.MainCurrencyAmount);
        }

        // ── Discount ──────────────────────────────────────────────────────────

        private float CalculateDiscountInternal(ShopBundleConfig bundle, bool isFirstPurchase)
        {
            // Jika BasePrice sama atau kurang dari threshold → tidak ada discount
            if (bundle.BasePrice <= bundle.MinimumPriceForDiscount) return 0f;

            float discount = isFirstPurchase
                ? Mathf.Max(bundle.FirstPurchaseDiscount, bundle.MainDiscount)
                : bundle.MainDiscount;

            return Mathf.Clamp01(discount);
        }
    }
}