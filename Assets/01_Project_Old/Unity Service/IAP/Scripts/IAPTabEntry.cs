using System;
using UnityEngine;

namespace Rush
{
    public class IAPTabEntry : MonoBehaviour
    {
        [SerializeField] private TabEntry m_TabEntry;
        [SerializeField] private IAPTabView m_IAPTabView;
        [SerializeField] private IAPTabConfig m_TabConfig;

        public void Populate(IAPTabConfig config,
            Action<IAPBundleConfig> onBundleClicked)
        {
            m_TabConfig = config;
            m_IAPTabView?.Populate(config, onBundleClicked);
        }

        public void RepopulateIfVisible(Action<IAPBundleConfig> onBundleClicked)
        {
            if (m_IAPTabView == null || !m_IAPTabView.IsShow) return;
            m_IAPTabView.Populate(m_TabConfig, onBundleClicked);
        }
    }
}