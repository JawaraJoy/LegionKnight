using UnityEngine;

namespace LegionKnight
{
    public class ShopPanelAgent : MonoBehaviour
    {
        private ShopPanel GetShopPanel()
        {
            return GameManager.Instance.GetPanel<ShopPanel>();
        }
        public void ShowShopPage(string pageName)
        {
            GetShopPanel().Show();
            GetShopPanel().ShowShop(pageName);
        }
    }
}
