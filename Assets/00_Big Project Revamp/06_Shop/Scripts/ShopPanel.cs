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

        protected override void ShowInternal()
        {
            base.ShowInternal();
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
                m_ShopTabEntries[i].Populate(tabs[i], OnBuyClickedInternal);
        }

        private void OnBuyClickedInternal(ShopBundleConfig bundle) =>
            Manager.RequestPurchase(bundle);

        private void OnPurchaseRequestedInternal(ShopConfirmData data)
        {
            var confirmPanel = CanvasManager.Instance.GetPanel<CurrencyConfirmationPanel>();
            confirmPanel?.ShowConfirmation(data, () => Manager.ExecutePurchase(data.Bundle));
        }

        private void OnPurchaseCompleteInternal(CollectibleResultData result)
        {
            RefreshActiveTabInternal();
            var resultPanel = CanvasManager.Instance.GetPanel<ShopResultPanel>();
            resultPanel?.Show(result);
        }

        private void OnPurchaseFailedInternal(string message) =>
            UnityEngine.Debug.LogWarning($"[ShopPanel] {message}");

        private void RefreshActiveTabInternal()
        {
            if (m_ShopTabEntries == null) return;
            foreach (var entry in m_ShopTabEntries)
                entry.RepopulateIfVisible(OnBuyClickedInternal);
        }
    }
}