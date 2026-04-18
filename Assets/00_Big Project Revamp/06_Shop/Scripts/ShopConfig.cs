using UnityEngine;

namespace Rush
{
    [CreateAssetMenu(fileName = "ShopConfig_", menuName = "Rush/Shop/Shop Config")]
    public class ShopConfig : Configuration
    {
        [SerializeField] private ShopTabConfig[] m_Tabs;
        public ShopTabConfig[] Tabs => m_Tabs;
    }
}