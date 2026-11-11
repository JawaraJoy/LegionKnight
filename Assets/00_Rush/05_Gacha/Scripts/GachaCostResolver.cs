using UnityEngine;
using LegionKnight;

namespace Rush
{
    public class GachaCostResolver : MonoBehaviour
    {
        [SerializeField] private string m_DailyResetKey = "GachaDailyReset_";

        // ── Public ────────────────────────────────────────────────────────────

        public int CalculateCost(GachaBannerConfig banner, bool isMulti, bool isDailyFirst)
        {
            int baseCost = isMulti
                ? banner.SingleDrawCost * banner.MultiDrawCount
                : banner.SingleDrawCost;

            float discount = CalculateDiscountInternal(banner, isMulti, isDailyFirst, baseCost);
            return Mathf.Max(1, Mathf.RoundToInt(baseCost * (1f - discount)));
        }

        public GachaCostBreakdown CalculateBreakdown(GachaBannerConfig banner,
            CurrenciesControl currencyControl, bool isMulti, bool isDailyFirst)
        {
            var breakdown = new GachaCostBreakdown();

            int baseCost = isMulti
                ? banner.SingleDrawCost * banner.MultiDrawCount
                : banner.SingleDrawCost;

            // Discount only applies if baseCost exceeds the minimum threshold
            float discount = CalculateDiscountInternal(banner, isMulti, isDailyFirst, baseCost);
            bool hasDiscount = discount > 0f;
            int totalCost = Mathf.Max(1, Mathf.RoundToInt(baseCost * (1f - discount)));

            breakdown.SetOriginalCost(baseCost);
            breakdown.SetTotalCost(totalCost);
            breakdown.SetHasDiscount(hasDiscount);

            int mainHeld = currencyControl.GetCurrencyAmount(banner.DrawCostCurrency);

            if (mainHeld >= totalCost)
            {
                // Main is sufficient — pay entirely from main
                breakdown.SetMainDeduct(totalCost);
                breakdown.SetAltDeduct(0);
                breakdown.SetMixed(false);
                breakdown.SetCanAfford(true);
                return breakdown;
            }

            if (banner.AltCostCurrency == null)
            {
                breakdown.SetMainDeduct(mainHeld);
                breakdown.SetAltDeduct(0);
                breakdown.SetMixed(false);
                breakdown.SetCanAfford(false);
                return breakdown;
            }

            // Main insufficient — use all available main, cover deficit with alt
            int mainUsed = mainHeld;
            int deficit = totalCost - mainUsed;

            // Alt conversion is based on the discounted single draw cost so alt
            // proportionally reflects the same discount as main
            // discountedSingleCost = SingleDrawCost * (1 - discount)
            float discountedSingleCost = banner.SingleDrawCost * (1f - discount);
            int altNeeded = discountedSingleCost > 0
                ? Mathf.CeilToInt(deficit / discountedSingleCost * banner.AltSingleDrawCost)
                : Mathf.CeilToInt((float)deficit / banner.SingleDrawCost * banner.AltSingleDrawCost);

            int altHeld = currencyControl.GetCurrencyAmount(banner.AltCostCurrency);

            breakdown.SetMainDeduct(mainUsed);
            breakdown.SetAltDeduct(altNeeded);
            breakdown.SetMixed(mainUsed > 0);
            breakdown.SetCanAfford(altHeld >= altNeeded);
            return breakdown;
        }

        public void DeductCost(GachaBannerConfig banner,
            CurrenciesControl currencyControl, GachaCostBreakdown breakdown)
        {
            if (!breakdown.CanAfford) return;

            if (breakdown.MainDeductAmount > 0)
                currencyControl.RemoveCurrencyAmount(
                    banner.DrawCostCurrency, breakdown.MainDeductAmount);

            if (breakdown.AltDeductAmount > 0 && banner.AltCostCurrency != null)
                currencyControl.RemoveCurrencyAmount(
                    banner.AltCostCurrency, breakdown.AltDeductAmount);
        }

        // ── Discount ──────────────────────────────────────────────────────────

        private float CalculateDiscountInternal(GachaBannerConfig banner,
            bool isMulti, bool isDailyFirst, int baseCost)
        {
            if (banner.DiscountConfig == null) return 0f;

            // Discount does not apply if base cost is at or below the minimum threshold
            if (baseCost <= banner.DiscountConfig.MinimumPriceForDiscount) return 0f;

            var d = banner.DiscountConfig;
            float discount = d.GeneralDiscount;

            if (isMulti)
            {
                discount = Mathf.Max(discount, d.MultiDrawDiscount);
                if (isDailyFirst) discount = Mathf.Max(discount, d.MultiDrawDailyDiscount);
            }
            else
            {
                if (isDailyFirst) discount = Mathf.Max(discount, d.SingleDrawDailyDiscount);
            }

            return Mathf.Clamp01(discount);
        }

        // ── Daily ─────────────────────────────────────────────────────────────

        public bool IsDailyFirstDraw(GachaBannerConfig banner, bool isMulti)
        {
            string key = m_DailyResetKey + banner.BaseInfo.Id + (isMulti ? "_multi" : "_single");
            if (!UnityService.Instance.HasData(key)) return true;
            return UnityService.Instance.GetData<string>(key)
                   != System.DateTime.Today.ToString("yyyyMMdd");
        }

        public void MarkDailyDrawUsed(GachaBannerConfig banner, bool isMulti)
        {
            string key = m_DailyResetKey + banner.BaseInfo.Id + (isMulti ? "_multi" : "_single");
            UnityService.Instance.SaveData(key, System.DateTime.Today.ToString("yyyyMMdd"));
        }
    }
}