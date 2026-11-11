namespace Rush
{
    public class GachaCostBreakdown
    {
        // ── Display — ditampilkan di UI tombol ────────────────────────────────
        // Berapa total yang harus dibayar (dari config, bukan dari sisa currency player)
        private int m_TotalCost;
        private int m_OriginalCost;
        private bool m_HasDiscount;

        // ── Deduction — dipakai saat benar-benar bayar ────────────────────────
        // Berapa yang diambil dari main currency player
        private int m_MainDeductAmount;
        // Berapa yang diambil dari alt currency player
        private int m_AltDeductAmount;

        private bool m_IsMixed;
        private bool m_CanAfford;

        // Display
        public int TotalCost => m_TotalCost;
        public int OriginalCost => m_OriginalCost;
        public bool HasDiscount => m_HasDiscount;

        // Deduction
        public int MainDeductAmount => m_MainDeductAmount;
        public int AltDeductAmount => m_AltDeductAmount;

        public bool IsMixed => m_IsMixed;
        public bool CanAfford => m_CanAfford;

        internal void SetTotalCost(int cost) => m_TotalCost = cost;
        internal void SetOriginalCost(int cost) => m_OriginalCost = cost;
        internal void SetHasDiscount(bool value) => m_HasDiscount = value;
        internal void SetMainDeduct(int amount) => m_MainDeductAmount = amount;
        internal void SetAltDeduct(int amount) => m_AltDeductAmount = amount;
        internal void SetMixed(bool mixed) => m_IsMixed = mixed;
        internal void SetCanAfford(bool can) => m_CanAfford = can;
    }
}