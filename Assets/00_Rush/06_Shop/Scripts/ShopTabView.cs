using System;
using UnityEngine;
using LegionKnight;

namespace Rush
{
    public class ShopTabView : UIView
    {
        [SerializeField] private ShopBundlePool m_BundlePool;

        [SerializeField] private TMPro.TextMeshProUGUI m_CountText;
        private int m_Count;

        protected override void ShowInternal()
        {
            base.ShowInternal();
            if (m_CountText != null) m_CountText.text = $"{m_Count} bundles";
        }

        public void Populate(ShopTabConfig tabConfig,
            Action<ShopBundleConfig> onBundleClicked)
        {
            if (m_BundlePool == null) return;
            m_BundlePool.ReturnAll();

            m_Count = 0;
            if (tabConfig?.Bundles == null) return;
            var manager = RushPlayer.Instance.ShopManager;
            foreach (var bundle in tabConfig.Bundles)
            {
                if (bundle == null) continue;
                m_Count++;
                var breakdown = manager.GetBreakdown(bundle);
                var availability = manager.GetAvailability(bundle);
                var item = m_BundlePool.Rent();
                item.transform.SetAsLastSibling();
                item.Setup(bundle, breakdown, availability, onBundleClicked);
            }
            if (IsShown && m_CountText != null) m_CountText.text = $"{m_Count} bundles";
        }
    }
}
