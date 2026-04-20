using UnityEngine;

namespace Rush
{
    [System.Serializable]
    public class ShopBundleEntry
    {
        [SerializeField] private CollectibleConfig m_Collectible;
        [SerializeField] private int m_Amount = 1;

        public CollectibleConfig Collectible => m_Collectible;
        public int Amount => m_Amount;
    }

    [CreateAssetMenu(fileName = "ShopBundle_", menuName = "Rush/Shop/Bundle")]
    public class ShopBundleConfig : Configuration
    {
        [Header("Contents")]
        [SerializeField] private ShopBundleEntry[] m_Entries;

        [Header("Price")]
        [SerializeField] private ItemConfig m_CostCurrency;
        [SerializeField] private int m_BasePrice;

        [Header("Discounts")]
        [SerializeField, Range(0f, 1f)] private float m_MainDiscount = 0f;
        [SerializeField, Range(0f, 1f)] private float m_FirstPurchaseDiscount = 0f;

        [Tooltip("Discount tidak berlaku jika BasePrice sama atau kurang dari nilai ini")]
        [SerializeField] private int m_MinimumPriceForDiscount = 0;

        [Header("Purchase Limit")]
        [SerializeField] private ShopBundlePurchaseLimit m_PurchaseLimit = ShopBundlePurchaseLimit.Unlimited;

        [Header("Display")]
        [SerializeField] private Sprite m_BundleSprite;

        public ShopBundleEntry[] Entries => m_Entries;
        public ItemConfig CostCurrency => m_CostCurrency;
        public int BasePrice => m_BasePrice;
        public float MainDiscount => m_MainDiscount;
        public float FirstPurchaseDiscount => m_FirstPurchaseDiscount;
        public int MinimumPriceForDiscount => m_MinimumPriceForDiscount;
        public ShopBundlePurchaseLimit PurchaseLimit => m_PurchaseLimit;
        public Sprite BundleSprite => m_BundleSprite;
    }
}