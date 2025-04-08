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
        private List<ShopItemDefinition> m_ShopItemDefinitions = new List<ShopItemDefinition>();
    }
}
