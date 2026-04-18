using System;
using UnityEngine;
using LegionKnight;

namespace Rush
{
    public class ShopTabView : UIView
    {
        [SerializeField] private ShopBundlePool m_BundlePool;

        public void Populate(ShopTabConfig tabConfig,
            Action<ShopBundleConfig> onBuyClicked)
        {
            if (m_BundlePool == null || tabConfig?.Bundles == null) return;
            m_BundlePool.ReturnAll();

            var manager = RushPlayer.Instance.ShopManager;
            foreach (var bundle in tabConfig.Bundles)
            {
                var breakdown = manager.GetBreakdown(bundle);
                var availability = manager.GetAvailability(bundle);
                var item = m_BundlePool.Rent();
                item.Setup(bundle, breakdown, availability);
                item.SetBuyListener(onBuyClicked);
            }
        }

        protected override void HideInternal()
        {
            m_BundlePool?.ReturnAll();
            base.HideInternal();
        }
    }
}