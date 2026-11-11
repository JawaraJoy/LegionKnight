using UnityEngine;

namespace Rush
{
    public class ShopConfirmData : IConfirmData
    {
        private readonly ShopBundleConfig m_Bundle;
        private readonly ShopCostBreakdown m_Breakdown;

        public ShopBundleConfig Bundle => m_Bundle;
        public ShopCostBreakdown Breakdown => m_Breakdown;

        public ShopConfirmData(ShopBundleConfig bundle, ShopCostBreakdown breakdown)
        {
            m_Bundle = bundle;
            m_Breakdown = breakdown;
        }

        // ── IConfirmData ──────────────────────────────────────────────────────

        public string DescriptionText => $"Buy {m_Bundle.BaseInfo.Name}?";
        public int MainCurrencyAmount => m_Breakdown.MainCurrencyAmount;
        public int OriginalCost => m_Breakdown.OriginalPrice;
        public bool HasDiscount => m_Breakdown.IsFirstPurchaseDiscount
                                            || m_Breakdown.MainCurrencyAmount < m_Breakdown.OriginalPrice;
        public string MainCurrencyName => m_Bundle.CostCurrency?.BaseInfo.Name;
        public Sprite MainCurrencyIcon => m_Bundle.CostCurrency?.CollectibleField?.Icon;
        public bool IsFree => m_Breakdown.IsFree;

        // Shop tidak pakai alt currency
        public bool HasAltCurrency => false;
        public int AltCurrencyAmount => 0;
        public string AltCurrencyName => string.Empty;
        public Sprite AltCurrencyIcon => null;
    }
}   