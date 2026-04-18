using UnityEngine;

namespace Rush
{
    // Gabungkan TabEntry + ShopTabView dalam satu komponen
    // Assign ke tiap tab button di ShopPanel
    public class ShopTabEntry : MonoBehaviour
    {
        [SerializeField] private TabEntry m_TabEntry;
        [SerializeField] private ShopTabView m_ShopTabView;
        [SerializeField] private ShopTabConfig m_TabConfig;

        public void Populate(ShopTabConfig config,
            System.Action<ShopBundleConfig> onBuyClicked)
        {
            m_TabConfig = config;
            m_ShopTabView?.Populate(config, onBuyClicked);
        }

        public void RepopulateIfVisible(System.Action<ShopBundleConfig> onBuyClicked)
        {
            if (m_ShopTabView == null || !m_ShopTabView.IsShow) return;
            m_ShopTabView.Populate(m_TabConfig, onBuyClicked);
        }
    }
}