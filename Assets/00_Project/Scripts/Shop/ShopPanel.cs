using System.Collections.Generic;
using UnityEngine;

namespace LegionKnight
{
    public static partial class PanelId
    {
        public const string ShopPanelId = "Shop";
    }
    public partial class ShopPanel : PanelView
    {
        [SerializeField]
        private List<ShopView> m_ShopViews = new();

        private ShopView GetShopViewInternal(string uniqueName)
        {
            return m_ShopViews.Find(shop => shop.UniqueId == uniqueName);
        }
        private void ShowShopInternal(string uniqueName)
        {
            foreach (var shopView in m_ShopViews)
            {
                shopView.Hide();
            }
            GameManager.Instance.SelectShop(uniqueName);
            GetShopViewInternal(uniqueName).Show();
        }
        public void ShowShop(string uniqueName)
        {
            ShowShopInternal(uniqueName);
        }
    }
}
