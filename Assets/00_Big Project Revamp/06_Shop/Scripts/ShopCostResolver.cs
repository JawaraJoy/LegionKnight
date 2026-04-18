using UnityEngine;
using LegionKnight;

namespace Rush
{
    public class ShopCostResolver : MonoBehaviour
    {
        public int CalculateFinalPrice(ShopBundleConfig bundle, bool isFirstPurchase)
        {
            float discount = isFirstPurchase
                ? Mathf.Max(bundle.FirstPurchaseDiscount, bundle.MainDiscount)
                : bundle.MainDiscount;

            return Mathf.Max(0, Mathf.RoundToInt(bundle.BasePrice * (1f - discount)));
        }

        public ShopCostBreakdown CalculateBreakdown(ShopBundleConfig bundle,
            CurrenciesControl currencyControl, bool isFirstPurchase)
        {
            var breakdown = new ShopCostBreakdown();
            int finalPrice = CalculateFinalPrice(bundle, isFirstPurchase);

            breakdown.SetOriginalPrice(bundle.BasePrice);
            breakdown.SetFirstPurchaseDiscount(
                isFirstPurchase && bundle.FirstPurchaseDiscount > 0f);

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
    }
}