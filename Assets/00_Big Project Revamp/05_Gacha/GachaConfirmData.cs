using UnityEngine;

namespace Rush
{
    public class GachaConfirmData : IConfirmData
    {
        private readonly GachaBannerConfig m_Banner;
        private readonly GachaCostBreakdown m_Breakdown;
        private readonly bool m_IsMulti;

        public GachaBannerConfig Banner => m_Banner;
        public GachaCostBreakdown Breakdown => m_Breakdown;
        public bool IsMulti => m_IsMulti;

        public GachaConfirmData(GachaBannerConfig banner,
            GachaCostBreakdown breakdown, bool isMulti)
        {
            m_Banner = banner;
            m_Breakdown = breakdown;
            m_IsMulti = isMulti;
        }

        // ── IConfirmData ──────────────────────────────────────────────────────

        public string DescriptionText =>
            m_IsMulti ? $"Draw {m_Banner.MultiDrawCount}x?" : "Draw 1x?";

        public int MainCurrencyAmount => m_Breakdown.MainCurrencyAmount;
        public string MainCurrencyName => m_Banner.DrawCostCurrency?.BaseInfo.Name;
        public Sprite MainCurrencyIcon => m_Banner.DrawCostCurrency?.CollectibleField?.Icon;
        public bool IsFree => false;

        public bool HasAltCurrency =>
            m_Breakdown.IsMixed && m_Breakdown.AltCurrencyAmount > 0;
        public int AltCurrencyAmount => m_Breakdown.AltCurrencyAmount;
        public string AltCurrencyName => m_Banner.AltCostCurrency?.BaseInfo.Name;
    }
}