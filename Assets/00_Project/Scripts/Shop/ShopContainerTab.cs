using System.Collections.Generic;
using UnityEngine;

namespace LegionKnight
{
    [System.Serializable]
    public partial class ShopContainerTab
    {
        [SerializeField]
        private string m_TabName = "Tab Name";
        [SerializeField]
        private List<ShopItemControl> m_ShopItemControls = new List<ShopItemControl>();

        public string TabName => m_TabName;

        public ShopItemControl GetShopItemControl(ShopItemDefinition defi)
        {
            ShopItemControl match = m_ShopItemControls.Find(item => item.ShopItem == defi);
            if (match == null)
            {
                Debug.LogError($"ShopItemControl not found for {defi.name} in {m_TabName}");
                return null;
            }
            return match;
        }

    }
}
