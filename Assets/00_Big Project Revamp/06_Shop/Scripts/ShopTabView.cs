using System;
using UnityEngine;
using LegionKnight;

namespace Rush
{
    public class ShopTabView : UIView
    {
        [SerializeField] private ShopBundlePool m_BundlePool;

        public void Populate(ShopTabConfig tabConfig,
            Action<ShopBundleConfig> onBundleClicked)
        {
            if (m_BundlePool == null || tabConfig?.Bundles == null) return;
            m_BundlePool.ReturnAll();

            var manager = RushPlayer.Instance.ShopManager;
            foreach (var bundle in tabConfig.Bundles)
            {
                var breakdown = manager.GetBreakdown(bundle);
                var availability = manager.GetAvailability(bundle);
                var item = m_BundlePool.Rent();
                item.Setup(bundle, breakdown, availability, onBundleClicked);
            }
        }

        protected override void HideInternal()
        {
            m_BundlePool?.ReturnAll();
            base.HideInternal();
        }
    }
}   