using UnityEngine;

namespace LegionKnight
{
    public partial class DiamondShopView : ShopView
    {
        
    }

    public partial class ShopPanel
    {
        private DiamondShopView GetDiamondShopView()
        {
            return GetBinding<DiamondShopView>();
        }
        public void ShowDiamondShop(string showTab)
        {
            ShowInternal();
            GetDiamondShopView().Show();
            GetDiamondShopView().ShowTab(showTab);
        }
    }

    public partial class GameManager
    {
        public void ShowDiamondShop(string showTab)
        {
            GetPanelInternal<ShopPanel>().ShowDiamondShop(showTab);
        }
    }
}
