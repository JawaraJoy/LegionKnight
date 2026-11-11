using UnityEngine;

namespace Rush
{
    [CreateAssetMenu(fileName = "ShopTab_", menuName = "Rush/Shop/Tab")]
    public class ShopTabConfig : ScriptableObject
    {
        [SerializeField] private string m_TabLabel;
        [SerializeField] private ShopBundleConfig[] m_Bundles;

        public string TabLabel => m_TabLabel;
        public ShopBundleConfig[] Bundles => m_Bundles;
    }
}