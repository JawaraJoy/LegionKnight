using LegionKnight;
using UnityEngine;
using UnityEngine.UI;

namespace Rush
{
    public class ShopPanel : PanelView
    {
        [SerializeField] private TabGroup m_TabGroup;
        [SerializeField] private ShopTabEntry[] m_ShopTabEntries;
        [SerializeField] private Button m_CloseButton;

        private ShopManager m_Manager;

        private void Start() => BindManagerInternal();

        private void BindManagerInternal()
        {
            if (m_Manager != null) return;
            m_Manager = RushPlayer.Instance.ShopManager;
            if (m_Manager == null) return;
            m_Manager.OnPurchaseRequested.AddListener(OnPurchaseRequestedInternal);
            m_Manager.OnPurchaseComplete.AddListener(OnPurchaseCompleteInternal);
            m_Manager.OnPurchaseFailed.AddListener(OnPurchaseFailedInternal);
            if (m_CloseButton != null) m_CloseButton.onClick.AddListener(HideInternal);
        }

        private void OnDestroy()
        {
            if (m_Manager == null) return;
            m_Manager.OnPurchaseRequested.RemoveListener(OnPurchaseRequestedInternal);
            m_Manager.OnPurchaseComplete.RemoveListener(OnPurchaseCompleteInternal);
            m_Manager.OnPurchaseFailed.RemoveListener(OnPurchaseFailedInternal);
        }

        protected override void ShowInternal()
        {
            BindManagerInternal();
            base.ShowInternal();
            if (!IsShown || m_Manager == null) return;
            var tabs = m_Manager.ShopConfig?.Tabs;
            if (tabs == null || m_ShopTabEntries == null) return;
            for (int i = 0; i < m_ShopTabEntries.Length && i < tabs.Length; i++)
                m_ShopTabEntries[i]?.Populate(tabs[i], OnBundleClickedInternal);
            m_TabGroup?.Show();
        }

        protected override void HideInternal()
        {
            m_TabGroup?.Hide();
            base.HideInternal();
        }

        public void ShowTab(ShopTabConfig tab)
        {
            ShowInternal();
            if (!IsShown || tab == null || m_TabGroup == null) return;
            var tabs = m_Manager?.ShopConfig?.Tabs;
            if (tabs == null) return;
            int index = System.Array.IndexOf(tabs, tab);
            if (index >= 0) m_TabGroup.OnTabClicked(index);
        }

        private void OnBundleClickedInternal(ShopBundleConfig bundle) =>
            CanvasManager.Instance.GetPanel<ShopBundleDetailPanel>()?.Show(bundle);

        private void OnPurchaseRequestedInternal(ShopConfirmData offer)
        {
            var confirm = CanvasManager.Instance.GetPanel<CurrencyConfirmationPanel>();
            if (confirm != null) confirm.ShowConfirmation(offer, () => m_Manager.ConfirmPurchase(offer));
        }

        private void OnPurchaseCompleteInternal(CollectibleResultData result)
        {
            if (m_ShopTabEntries != null)
                foreach (var entry in m_ShopTabEntries) entry?.RepopulateIfVisible(OnBundleClickedInternal);
            CanvasManager.Instance.GetPanel<ShopBundleDetailPanel>()?.RefreshOfferInternal();
            CanvasManager.Instance.GetPanel<ShopResultPanel>()?.Show(result);
        }

        private void OnPurchaseFailedInternal(string message)
        {
            CanvasManager.Instance.GetPanel<ShopBundleDetailPanel>()?.ShowFailureInternal(message);
            Debug.LogWarning($"[ShopPanel] {message}", this);
        }
    }
}
