using System;
using UnityEngine;
using LegionKnight;

namespace Rush
{
    public class IAPTabView : UIView
    {
        [SerializeField] private IAPBundlePool m_BundlePool;

        public void Populate(IAPTabConfig tabConfig,
            Action<IAPBundleConfig> onBundleClicked)
        {
            if (m_BundlePool == null || tabConfig?.Bundles == null) return;
            m_BundlePool.ReturnAll();

            var iap = UnityService.Instance.IAPManager;
            foreach (var bundle in tabConfig.Bundles)
            {
                if (bundle == null) continue;
                string localizedPrice = iap.GetLocalizedPrice(bundle);
                bool isFirst = iap.IsFirstPurchase(bundle);
                bool canPurchase = iap.CanPurchase(bundle);
                var item = m_BundlePool.Rent();
                item.Setup(bundle, localizedPrice, isFirst, canPurchase, onBundleClicked);
            }
        }

        protected override void HideInternal()
        {
            //m_BundlePool?.ReturnAll();
            base.HideInternal();
        }
    }
}