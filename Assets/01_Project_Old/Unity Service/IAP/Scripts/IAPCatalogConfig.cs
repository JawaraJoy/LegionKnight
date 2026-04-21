using UnityEngine;

namespace Rush
{
    // Root catalog — assign to IAPManager
    [CreateAssetMenu(fileName = "IAPCatalog_", menuName = "Rush/IAP/Catalog")]
    public class IAPCatalogConfig : Configuration
    {
        [SerializeField] private IAPTabConfig[] m_Tabs;
        public IAPTabConfig[] Tabs => m_Tabs;
    }
}
