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
            foreach (var shopView in m_ShopViews)
            {
                shopView.Hide();
            }
            GetDiamondShopView().Show();
            GetDiamondShopView().ShowTab(showTab);
        }
    }

    public partial class CanvasManager
    {
        public void ShowDiamondShop(string showTab)
        {

            GetPanelInternal<ShopPanel>().ShowDiamondShop(showTab);
        }
    }
}
