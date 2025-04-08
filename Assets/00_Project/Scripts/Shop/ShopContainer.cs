using System.Collections.Generic;
using UnityEngine;

namespace LegionKnight
{
    [CreateAssetMenu(fileName = "New Shop Content", menuName = "Legion Knight/Shop Content", order = 1)]
    public partial class ShopContainer : ScriptableObject
    {
        [SerializeField]
        private ShopBannerField m_ShopBannerField;
        [SerializeField]
        private List<ShopContainerTab> m_ShopContainerTabs = new List<ShopContainerTab>();
    }
}
