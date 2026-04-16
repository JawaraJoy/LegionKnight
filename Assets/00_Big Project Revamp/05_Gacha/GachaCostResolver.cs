using UnityEngine;
using LegionKnight;

namespace Rush
{
    public class GachaCostResolver : MonoBehaviour
    {
        [SerializeField] private string m_DailyResetKey = "GachaDailyReset_";

        public int CalculateCost(GachaBannerConfig banner, bool isMulti, bool isDailyFirst)
        {
            int baseCost = isMulti
                ? banner.SingleDrawCost * banner.MultiDrawCount
                : banner.SingleDrawCost;

            float discount = CalculateDiscountInternal(banner, isMulti, isDailyFirst);
            return Mathf.Max(1, Mathf.RoundToInt(baseCost * (1f - discount)));
        }

        private float CalculateDiscountInternal(GachaBannerConfig banner, bool isMulti, bool isDailyFirst)
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

        public bool HasEnoughCurrency(GachaBannerConfig banner, CurrenciesControl currencyControl,
            bool isMulti, bool isDailyFirst)
        {
            int cost = CalculateCost(banner, isMulti, isDailyFirst);
            int mainAmount = currencyControl.GetCurrencyAmount(banner.DrawCostCurrency);
            if (mainAmount >= cost) return true;
            if (banner.AltCostCurrency == null) return false;

            int altAmount = currencyControl.GetCurrencyAmount(banner.AltCostCurrency);
            int deficit = cost - mainAmount;
            // konversi deficit ke alt currency
            int altNeeded = Mathf.CeilToInt((float)deficit / banner.SingleDrawCost * banner.AltSingleDrawCost);
            return altAmount >= altNeeded;
        }

        public void DeductCost(GachaBannerConfig banner, CurrenciesControl currencyControl,
            bool isMulti, bool isDailyFirst)
        {
            int cost = CalculateCost(banner, isMulti, isDailyFirst);
            int mainAmount = currencyControl.GetCurrencyAmount(banner.DrawCostCurrency);

            if (mainAmount >= cost)
            {
                currencyControl.RemoveCurrencyAmount(banner.DrawCostCurrency, cost);
                return;
            }

            currencyControl.RemoveCurrencyAmount(banner.DrawCostCurrency, mainAmount);

            if (banner.AltCostCurrency != null)
            {
                int deficit = cost - mainAmount;
                int altNeeded = Mathf.CeilToInt((float)deficit / banner.SingleDrawCost * banner.AltSingleDrawCost);
                currencyControl.RemoveCurrencyAmount(banner.AltCostCurrency, altNeeded);
            }
        }

        public bool IsDailyFirstDraw(GachaBannerConfig banner, bool isMulti)
        {
            string key = m_DailyResetKey + banner.BaseInfo.Id + (isMulti ? "_multi" : "_single");
            if (!UnityService.Instance.HasData(key)) return true;
            return UnityService.Instance.GetData<string>(key) != System.DateTime.Today.ToString("yyyyMMdd");
        }

        public void MarkDailyDrawUsed(GachaBannerConfig banner, bool isMulti)
        {
            string key = m_DailyResetKey + banner.BaseInfo.Id + (isMulti ? "_multi" : "_single");
            UnityService.Instance.SaveData(key, System.DateTime.Today.ToString("yyyyMMdd"));
        }
    }
}