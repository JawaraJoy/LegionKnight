using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace LegionKnight
{
    public partial class ShopView : UIView
    {
        [SerializeField]
        private List<ShopTabView> m_Tabs = new();

        private ShopTabView GetShopTabViewInternal(string uniqueName)
        {
            return m_Tabs.Find(tab => tab.UniqueId == uniqueName);
        }
        public void ShowTab(string uniqueName)
        {
            foreach (var tab in m_Tabs)
            {
                tab.Hide();
            }
            GetShopTabViewInternal(uniqueName).Show();
        }
    }
}
