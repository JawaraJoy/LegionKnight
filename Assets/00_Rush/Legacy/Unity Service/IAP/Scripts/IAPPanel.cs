using UnityEngine;
using UnityEngine.UI;
using LegionKnight;

namespace Rush
{
    public class IAPPanel : PanelView
    {
        [SerializeField] private TabGroup m_TabGroup;
        [SerializeField] private IAPTabEntry[] m_IAPTabEntries;
        [SerializeField] private Button m_CloseButton;

        [SerializeField] private RechargeStoreView m_StoreView;
        private IAPManager IAPManager => UnityService.Instance.IAPManager;

        protected override void ShowInternal()
        {
            if (IsShown) return;
            base.ShowInternal();
            if (!IsShown) return;
            m_StoreView.Bind();
            SubscribeEventsInternal();
            PopulateTabsInternal();
            if (m_CloseButton != null) m_CloseButton.onClick.AddListener(Hide);
            m_TabGroup?.Show();
        }

        protected override void HideInternal()
        {
            UnsubscribeEventsInternal();
            if (m_CloseButton != null) m_CloseButton.onClick.RemoveListener(Hide);
            m_TabGroup?.Hide();
            base.HideInternal();
        }

        private void SubscribeEventsInternal()
        {
            m_StoreView.Changed += RefreshActiveTabInternal;
            IAPManager.OnPurchaseComplete.AddListener(OnPurchaseCompleteInternal);
            IAPManager.OnPurchaseFailed.AddListener(OnPurchaseFailedInternal);
        }

        private void UnsubscribeEventsInternal()
        {
            m_StoreView.Changed -= RefreshActiveTabInternal;
            IAPManager.OnPurchaseComplete.RemoveListener(OnPurchaseCompleteInternal);
            IAPManager.OnPurchaseFailed.RemoveListener(OnPurchaseFailedInternal);
        }

        private void PopulateTabsInternal()
        {
            var tabs = IAPManager.Catalog?.Tabs;
            if (tabs == null || m_IAPTabEntries == null) return;

            for (int i = 0; i < m_IAPTabEntries.Length && i < tabs.Length; i++)
                m_IAPTabEntries[i].Populate(tabs[i], OnBundleClickedInternal);
        }

        // Bundle clicked - open detail panel (no confirmation panel for IAP)
        private void OnBundleClickedInternal(IAPBundleConfig bundle)
        {
            var detailPanel = CanvasManager.Instance.GetPanel<IAPBundleDetailPanel>();
            detailPanel?.Show(bundle);
        }

        private void OnPurchaseCompleteInternal(CollectibleResultData result)
        {
            CanvasManager.Instance.GetPanel<IAPBundleDetailPanel>()?.Hide();

            var resultPanel = CanvasManager.Instance.GetPanel<ShopResultPanel>();
            resultPanel?.Show(result);
        }

        private void OnPurchaseFailedInternal(IAPBundleConfig bundle, string reason) =>
            Debug.LogWarning($"[IAPPanel] Purchase failed - {bundle?.BaseInfo.Name}: {reason}");

        private void RefreshActiveTabInternal()
        {
            if (m_IAPTabEntries == null) return;
            foreach (var entry in m_IAPTabEntries)
                entry?.RefreshOffers(OnBundleClickedInternal);
        }
    }
}
