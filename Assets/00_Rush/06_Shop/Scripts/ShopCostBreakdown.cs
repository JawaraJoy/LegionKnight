namespace Rush
{
    public class ShopCostBreakdown
    {
        private int m_MainCurrencyAmount;
        private int m_OriginalPrice;
        private bool m_CanAfford;
        private bool m_IsFree;
        private bool m_IsFirstPurchaseDiscount;
        private bool m_HasDiscount;

        public int MainCurrencyAmount => m_MainCurrencyAmount;
        public int OriginalPrice => m_OriginalPrice;
        public bool CanAfford => m_CanAfford;
        public bool IsFree => m_IsFree;
        public bool IsFirstPurchaseDiscount => m_IsFirstPurchaseDiscount;
        public bool HasDiscount => m_HasDiscount;

        public string DiscountLabel => m_OriginalPrice > 0
            ? $"-{(1f - (float)m_MainCurrencyAmount / m_OriginalPrice) * 100f:F0}%"
            : string.Empty;

        internal void SetMain(int amount) => m_MainCurrencyAmount = amount;
        internal void SetOriginalPrice(int price) => m_OriginalPrice = price;
        internal void SetCanAfford(bool can) => m_CanAfford = can;
        internal void SetFree(bool free) => m_IsFree = free;
        internal void SetFirstPurchaseDiscount(bool value) => m_IsFirstPurchaseDiscount = value;
        internal void SetHasDiscount(bool value) => m_HasDiscount = value;
    }
}
