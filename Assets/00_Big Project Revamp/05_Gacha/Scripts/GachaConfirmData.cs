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

        public string DescriptionText => m_IsMulti
            ? $"Draw {m_Banner.MultiDrawCount}x?"
            : "Draw 1x?";

        // Berapa yang benar-benar diambil dari main wallet player
        // bukan total cost — agar sesuai dengan yang akan di-deduct
        public int MainCurrencyAmount => m_Breakdown.MainDeductAmount;
        public int OriginalCost => m_Breakdown.OriginalCost;
        public bool HasDiscount => m_Breakdown.HasDiscount;
        public string MainCurrencyName => m_Banner.DrawCostCurrency?.BaseInfo.Name;
        public Sprite MainCurrencyIcon => m_Banner.DrawCostCurrency?.CollectibleField?.Icon;
        public bool IsFree => false;

        // Alt tampil hanya jika ada yang benar-benar perlu diambil dari alt
        public bool HasAltCurrency => m_Breakdown.AltDeductAmount > 0;
        public int AltCurrencyAmount => m_Breakdown.AltDeductAmount;
        public string AltCurrencyName => m_Banner.AltCostCurrency?.BaseInfo.Name;
        public Sprite AltCurrencyIcon => m_Banner.AltCostCurrency?.CollectibleField?.Icon;
    }
}