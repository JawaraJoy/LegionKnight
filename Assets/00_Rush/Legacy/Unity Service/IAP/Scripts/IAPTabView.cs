using System;
using LegionKnight;
using TMPro;
using UnityEngine;

namespace Rush
{
    public class IAPTabView : UIView
    {
        [SerializeField] private IAPBundlePool m_BundlePool;
        [SerializeField] private TMP_Text m_Count;

        public void Populate(IAPTabConfig tab, Action<IAPBundleConfig> onSelect) =>
            PopulateInternal(tab, onSelect);

        private void PopulateInternal(IAPTabConfig tab, Action<IAPBundleConfig> onSelect)
        {
            if (m_BundlePool == null || tab?.Bundles == null) return;
            m_BundlePool.ReturnAll();
            var store = UnityService.Instance.IAPManager;
            int count = 0;
            foreach (var bundle in tab.Bundles)
            {
                if (bundle == null) continue;
                var item = m_BundlePool.Rent();
                item.transform.SetAsLastSibling();
                item.Setup(bundle, store.GetLocalizedPrice(bundle), store.IsFirstPurchase(bundle),
                    store.CanPurchase(bundle), onSelect);
                count++;
            }
            if (m_Count != null) m_Count.text = count == 1 ? "1 pack" : $"{count} packs";
        }
    }
}
