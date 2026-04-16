namespace Rush
{
    // Menyimpan rincian berapa main dan alt currency yang akan dipakai
    // Dibuat sebelum konfirmasi agar UI bisa display breakdown ke user
    public class GachaCostBreakdown
    {
        private int m_MainCurrencyAmount;
        private int m_AltCurrencyAmount;
        private bool m_IsMixed;
        private bool m_CanAfford;

        public int MainCurrencyAmount => m_MainCurrencyAmount;
        public int AltCurrencyAmount => m_AltCurrencyAmount;
        public bool IsMixed => m_IsMixed;
        public bool CanAfford => m_CanAfford;

        internal void SetMain(int amount) => m_MainCurrencyAmount = amount;
        internal void SetAlt(int amount) => m_AltCurrencyAmount = amount;
        internal void SetMixed(bool mixed) => m_IsMixed = mixed;
        internal void SetCanAfford(bool can) => m_CanAfford = can;
    }
}