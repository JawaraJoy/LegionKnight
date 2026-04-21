using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Purchasing;

namespace Rush
{
    [CreateAssetMenu(fileName = "IAPBundle_", menuName = "Rush/IAP/Bundle")]
    public class IAPBundleConfig : Configuration, ICart
    {
        [SerializeField]
        private CartItem[] m_CartItems;
        [Header("Main Items")]
        [SerializeField] private IAPBundleEntry[] m_Entries;

        [Header("First Purchase Bonus")]
        [SerializeField] private IAPBundleEntry[] m_FirstPurchaseBonusEntries;

        [Header("Display")]
        [SerializeField] private Sprite m_BundleSprite;

        public string ProductId => m_BaseInfo.Id;
        public IAPBundleEntry[] Entries => m_Entries;
        public IAPBundleEntry[] FirstPurchaseBonusEntries => m_FirstPurchaseBonusEntries;
        public Sprite BundleSprite => m_BundleSprite;

        public bool HasFirstPurchaseBonus =>
            m_FirstPurchaseBonusEntries is { Length: > 0 };

        public IReadOnlyList<CartItem> Items()
        {
            throw new System.NotImplementedException();
        }
    }
}