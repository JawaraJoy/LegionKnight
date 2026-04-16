using UnityEngine;
using LegionKnight;

namespace Rush
{
    public class GachaCostResolver : MonoBehaviour
    {
        [SerializeField] private string m_DailyResetKey = "GachaDailyReset_";

        // ── Kalkulasi ──────────────────────────────────────────────────────────

        public int CalculateCost(GachaBannerConfig banner, bool isMulti, bool isDailyFirst)
        {
            int baseCost = isMulti
                ? banner.SingleDrawCost * banner.MultiDrawCount
                : banner.SingleDrawCost;

            float discount = CalculateDiscountInternal(banner, isMulti, isDailyFirst);
            return Mathf.Max(1, Mathf.RoundToInt(baseCost * (1f - discount)));
        }

        // Hitung breakdown mix currency secara detail
        // Logic:
        // 1. Coba bayar penuh dengan main → jika cukup, main saja
        // 2. Main kurang → pakai semua main yang ada, kekurangannya dari alt
        // 3. Alt juga kurang → CanAfford = false
        public GachaCostBreakdown CalculateBreakdown(GachaBannerConfig banner,
            CurrenciesControl currencyControl, bool isMulti, bool isDailyFirst)
        {
            var breakdown = new GachaCostBreakdown();
            int totalCost = CalculateCost(banner, isMulti, isDailyFirst);
            int mainHeld = currencyControl.GetCurrencyAmount(banner.DrawCostCurrency);

            if (mainHeld >= totalCost)
            {
                // bayar penuh dengan main
                breakdown.SetMain(totalCost);
                breakdown.SetAlt(0);
                breakdown.SetMixed(false);
                breakdown.SetCanAfford(true);
                return breakdown;
            }

            // main tidak cukup
            int mainUsed = mainHeld;
            int deficit = totalCost - mainUsed;

            if (banner.AltCostCurrency == null)
            {
                breakdown.SetMain(mainUsed);
                breakdown.SetAlt(0);
                breakdown.SetMixed(false);
                breakdown.SetCanAfford(false);
                return breakdown;
            }

            // konversi deficit → kebutuhan alt currency
            // rumus: altNeeded = ceil(deficit / singleDrawCost * altSingleDrawCost)
            int altNeeded = Mathf.CeilToInt(
                (float)deficit / banner.SingleDrawCost * banner.AltSingleDrawCost);
            int altHeld = currencyControl.GetCurrencyAmount(banner.AltCostCurrency);

            breakdown.SetMain(mainUsed);
            breakdown.SetAlt(altNeeded);
            breakdown.SetMixed(mainUsed > 0);
            breakdown.SetCanAfford(altHeld >= altNeeded);
            return breakdown;
        }

        public bool HasEnoughCurrency(GachaBannerConfig banner,
            CurrenciesControl currencyControl, bool isMulti, bool isDailyFirst)
        {
            return CalculateBreakdown(banner, currencyControl, isMulti, isDailyFirst).CanAfford;
        }

        public void DeductCost(GachaBannerConfig banner, CurrenciesControl currencyControl,
            GachaCostBreakdown breakdown)
        {
            if (!breakdown.CanAfford) return;

            if (breakdown.MainCurrencyAmount > 0)
                currencyControl.RemoveCurrencyAmount(banner.DrawCostCurrency,
                    breakdown.MainCurrencyAmount);

            if (breakdown.AltCurrencyAmount > 0 && banner.AltCostCurrency != null)
                currencyControl.RemoveCurrencyAmount(banner.AltCostCurrency,
                    breakdown.AltCurrencyAmount);
        }

        // ── Discount ──────────────────────────────────────────────────────────

        private float CalculateDiscountInternal(GachaBannerConfig banner,
            bool isMulti, bool isDailyFirst)
        {
            if (banner.DiscountConfig == null) return 0f;

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

        // ── Daily ──────────────────────────────────────────────────────────────

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