namespace Rush
{
    // Hasil query availability — dikirim ke UI untuk update state tombol
    public class ShopBundleAvailability
    {
        private readonly bool m_CanPurchase;
        private readonly bool m_IsFirstPurchase;
        private readonly double m_ResetSecondsRemaining;
        private readonly ShopBundlePurchaseLimit m_LimitType;

        public bool CanPurchase => m_CanPurchase;
        public bool IsFirstPurchase => m_IsFirstPurchase;
        public double ResetSecondsRemaining => m_ResetSecondsRemaining;
        public ShopBundlePurchaseLimit LimitType => m_LimitType;
        public bool IsDaily =>
            m_LimitType == ShopBundlePurchaseLimit.Daily;

        public ShopBundleAvailability(bool canPurchase, bool isFirstPurchase,
            double resetSeconds, ShopBundlePurchaseLimit limitType)
        {
            m_CanPurchase = canPurchase;
            m_IsFirstPurchase = isFirstPurchase;
            m_ResetSecondsRemaining = resetSeconds;
            m_LimitType = limitType;
        }
    }
}