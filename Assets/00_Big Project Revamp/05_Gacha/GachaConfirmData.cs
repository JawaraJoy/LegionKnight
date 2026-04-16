namespace Rush
{
    // Dikirim via OnDrawRequested event → diterima CurrencyConfirmationPanel
    public class GachaConfirmData
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
    }
}