using UnityEngine;
using UnityEngine.UI;
using LegionKnight;

namespace Rush
{
    public class ShopPanel : PanelView
    {
        [SerializeField] private TabGroup m_TabGroup;
        [SerializeField] private ShopTabEntry[] m_ShopTabEntries;
        [SerializeField] private Button m_CloseButton;

        private ShopManager Manager => RushPlayer.Instance.ShopManager;

        private void Awake()
        {
            if (m_CloseButton != null) m_CloseButton.onClick.AddListener(HideInternal);
            SubscribeEventsInternal();
        }
        protected override void ShowInternal()
        {
            base.ShowInternal();
            
            PopulateTabsInternal();
            
            m_TabGroup?.Show();
        }

        protected override void HideInternal()
        {
            //UnsubscribeEventsInternal();
            m_TabGroup?.Hide();
            base.HideInternal();
        }

        private void SubscribeEventsInternal()
        {
            Manager.OnPurchaseRequested.AddListener(OnPurchaseRequestedInternal);
            Manager.OnPurchaseComplete.AddListener(OnPurchaseCompleteInternal);
            Manager.OnPurchaseFailed.AddListener(OnPurchaseFailedInternal);
        }

        private void UnsubscribeEventsInternal()
        {
            Manager.OnPurchaseRequested.RemoveListener(OnPurchaseRequestedInternal);
            Manager.OnPurchaseComplete.RemoveListener(OnPurchaseCompleteInternal);
            Manager.OnPurchaseFailed.RemoveListener(OnPurchaseFailedInternal);
        }

        private void PopulateTabsInternal()
        {
            var tabs = Manager.ShopConfig?.Tabs;
            if (tabs == null || m_ShopTabEntries == null) return;

            for (int i = 0; i < m_ShopTabEntries.Length && i < tabs.Length; i++)
                m_ShopTabEntries[i].Populate(tabs[i], OnBundleClickedInternal);
        }

        // Bundle di-klik → buka detail panel
        private void OnBundleClickedInternal(ShopBundleConfig bundle)
        {
            var detailPanel = CanvasManager.Instance.GetPanel<ShopBundleDetailPanel>();
            detailPanel?.Show(bundle);
        }

        // Purchase request dari detail panel → buka confirm
        private void OnPurchaseRequestedInternal(ShopConfirmData data)
        {
            var confirmPanel = CanvasManager.Instance.GetPanel<CurrencyConfirmationPanel>();
            confirmPanel?.ShowConfirmation(data, () => Manager.ExecutePurchase(data.Bundle));
        }

        private void OnPurchaseCompleteInternal(CollectibleResultData result)
        {
            // Refresh list badge (first purchase, unavailable) di semua tab
            RefreshActiveTabInternal();

            // Refresh detail panel jika masih terbuka
            var detailPanel = CanvasManager.Instance.GetPanel<ShopBundleDetailPanel>();
            // detail panel refresh dirinya sendiri via RefreshIfShowing

            // Tampilkan result
            var resultPanel = CanvasManager.Instance.GetPanel<ShopResultPanel>();
            resultPanel?.Show(result);
        }

        private void OnPurchaseFailedInternal(string message) =>
            UnityEngine.Debug.LogWarning($"[ShopPanel] {message}");

        private void RefreshActiveTabInternal()
        {
            if (m_ShopTabEntries == null) return;
            foreach (var entry in m_ShopTabEntries)
                entry.RepopulateIfVisible(OnBundleClickedInternal);
        }
    }
}