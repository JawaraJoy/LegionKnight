using UnityEngine;

namespace Rush
{
    // One tab in the IAP panel
    [CreateAssetMenu(fileName = "IAPTab_", menuName = "Rush/IAP/Tab")]
    public class IAPTabConfig : ScriptableObject
    {
        [SerializeField] private string m_TabLabel;
        [SerializeField] private IAPBundleConfig[] m_Bundles;

        public string TabLabel => m_TabLabel;
        public IAPBundleConfig[] Bundles => m_Bundles;
    }

    
}