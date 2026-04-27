using UnityEngine;

namespace Rush
{
    [CreateAssetMenu(fileName = "IAPBundle_", menuName = "Rush/IAP/Bundle")]
    public class IAPBundleConfig : Configuration
    {
        [Header("Main Items")]
        [SerializeField] private IAPBundleEntry[] m_Entries;

        [Header("First Purchase Bonus")]
        [SerializeField] private IAPBundleEntry[] m_FirstPurchaseBonusEntries;

        [Header("Purchase Limit")]
        [SerializeField] private ShopBundlePurchaseLimit m_PurchaseLimit = ShopBundlePurchaseLimit.Unlimited;

        [Header("Display")]
        [SerializeField] private Sprite m_BundleSprite;

        // ProductId reuses BaseInfo.Id — no separate field needed
        public string ProductId => m_BaseInfo.Id;
        public IAPBundleEntry[] Entries => m_Entries;
        public IAPBundleEntry[] FirstPurchaseBonusEntries => m_FirstPurchaseBonusEntries;
        public ShopBundlePurchaseLimit PurchaseLimit => m_PurchaseLimit;
        public Sprite BundleSprite => m_BundleSprite;

        public bool HasFirstPurchaseBonus =>
            m_FirstPurchaseBonusEntries is { Length: > 0 };
    }
}