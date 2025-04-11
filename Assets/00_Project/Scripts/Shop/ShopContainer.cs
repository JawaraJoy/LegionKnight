using System.Collections.Generic;
using UnityEngine;

namespace LegionKnight
{
    [System.Serializable]
    public partial class ShopContainer
    {
        [SerializeField]
        private ShopBannerField m_ShopBannerField;
        [SerializeField]
        private Currency m_CurrencyUsed;
        [SerializeField]
        private List<ShopContainerTab> m_ShopContainerTabs = new();
        public string ShopName => m_ShopBannerField.BannerName;
        public Sprite ShopMainImage => m_ShopBannerField.BannerMainImage;
        public List<ShopContainerTab> ShopContainerTabs => m_ShopContainerTabs;
        public string BannerName => m_ShopBannerField.BannerName;
        public Sprite BannerMainImage => m_ShopBannerField.BannerMainImage;

        public void Init()
        {
            int playerCurrencyAmount = Player.Instance.GetCurrencyAmount(m_CurrencyUsed.CurrencyDefinition);
            m_CurrencyUsed.SetAmount(playerCurrencyAmount);
        }
        public Currency GetCurrencyUsed()
        {
            return m_CurrencyUsed;
        }
        private ShopContainerTab GetShopContainerTabInternal(string name)
        {
            foreach (var tab in m_ShopContainerTabs)
            {
                if (tab.TabName == name)
                {
                    return tab;
                }
            }
            Debug.LogError($"Shop container tab with name {name} not found");
            return null;
        }
        public ShopItemControl GetShopItemControl(ShopItemDefinition defi)
        {
            var tab = GetShopContainerTabInternal(defi.TabName);
            if (tab == null)
            {
                Debug.LogError($"Tab {defi.TabName} not found in shop container {ShopName}");
                return null;
            }
            return tab.GetShopItemControl(defi);
        }
    }
}
