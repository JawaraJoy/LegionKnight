using UnityEngine;
using UnityEngine.UI;
using TMPro;
using LegionKnight;

namespace Rush
{
    public class GachaPanel : PanelView
    {
        [SerializeField] private Image m_BannerImage;
        [SerializeField] private Button m_DrawSingleButton;
        [SerializeField] private Button m_DrawMultiButton;
        [SerializeField] private Button m_DetailButton;
        [SerializeField] private TextMeshProUGUI m_SingleCostText;
        [SerializeField] private TextMeshProUGUI m_MultiCostText;
        [SerializeField] private TextMeshProUGUI m_PityProgressText;
        [SerializeField] private Transform m_BannerButtonContainer;
        [SerializeField] private GachaBannerButtonUI m_BannerButtonPrefab;

        private GachaManager Manager => RushPlayer.Instance.GachaManager;

        protected override void ShowInternal()
        {
            base.ShowInternal();
            RefreshViewInternal();
            PopulateBannerButtonsInternal();
            SubscribeEventsInternal();
        }

        protected override void HideInternal()
        {
            UnsubscribeEventsInternal();
            base.HideInternal();
        }

        private void SubscribeEventsInternal()
        {
            Manager.OnDrawComplete.AddListener(OnDrawCompleteInternal);
            Manager.OnDrawFailed.AddListener(OnDrawFailedInternal);
            Manager.OnDrawRequested.AddListener(OnDrawRequestedInternal);
            if (m_DrawSingleButton != null) m_DrawSingleButton.onClick.AddListener(OnDrawSingleClickedInternal);
            if (m_DrawMultiButton != null) m_DrawMultiButton.onClick.AddListener(OnDrawMultiClickedInternal);
            if (m_DetailButton != null) m_DetailButton.onClick.AddListener(OnDetailClickedInternal);
        }

        private void UnsubscribeEventsInternal()
        {
            Manager.OnDrawComplete.RemoveListener(OnDrawCompleteInternal);
            Manager.OnDrawFailed.RemoveListener(OnDrawFailedInternal);
            Manager.OnDrawRequested.RemoveListener(OnDrawRequestedInternal);
            if (m_DrawSingleButton != null) m_DrawSingleButton.onClick.RemoveListener(OnDrawSingleClickedInternal);
            if (m_DrawMultiButton != null) m_DrawMultiButton.onClick.RemoveListener(OnDrawMultiClickedInternal);
            if (m_DetailButton != null) m_DetailButton.onClick.RemoveListener(OnDetailClickedInternal);
        }

        private void RefreshViewInternal()
        {
            var banner = Manager.ActiveBanner;
            if (banner == null) return;

            if (m_BannerImage != null) m_BannerImage.sprite = banner.BannerSplashSprite;

            var singleBreakdown = Manager.GetBreakdown(false);
            var multiBreakdown = Manager.GetBreakdown(true);

            if (m_SingleCostText != null)
                m_SingleCostText.text = singleBreakdown.MainCurrencyAmount.ToString();
            if (m_MultiCostText != null)
                m_MultiCostText.text = multiBreakdown.MainCurrencyAmount.ToString();
            if (m_PityProgressText != null)
                m_PityProgressText.text =
                    $"{Manager.PityTracker.FinalPityCounter}/{banner.FinalPityCount}";

            if (m_DrawSingleButton != null) m_DrawSingleButton.interactable = Manager.CanAffordDraw(false);
            if (m_DrawMultiButton != null) m_DrawMultiButton.interactable = Manager.CanAffordDraw(true);
        }

        private void PopulateBannerButtonsInternal()
        {
            if (m_BannerButtonContainer == null || m_BannerButtonPrefab == null) return;
            foreach (Transform child in m_BannerButtonContainer) Destroy(child.gameObject);
            foreach (var banner in Manager.Banners)
            {
                var btn = Instantiate(m_BannerButtonPrefab, m_BannerButtonContainer);
                btn.Setup(banner, OnBannerSelectedInternal);
            }
        }

        private void OnBannerSelectedInternal(GachaBannerConfig banner)
        {
            Manager.SelectBanner(banner);
            RefreshViewInternal();
        }

        // Tombol → request (pre-confirm)
        private void OnDrawSingleClickedInternal() => Manager.RequestDrawSingle();
        private void OnDrawMultiClickedInternal() => Manager.RequestDrawMulti();

        // Manager memberi tahu ada request → buka confirm panel dengan breakdown
        private void OnDrawRequestedInternal(GachaConfirmData data)
        {
            var confirmPanel = CanvasManager.Instance.GetPanel<CurrencyConfirmationPanel>();
            System.Action executeAction = data.IsMulti
                ? Manager.ExecuteDrawMulti
                : Manager.ExecuteDrawSingle;
            confirmPanel?.ShowConfirmation(data, executeAction);
        }

        private void OnDetailClickedInternal()
        {
            var detailPanel = CanvasManager.Instance.GetPanel<BannerDetailPanel>();
            detailPanel?.Show(Manager.ActiveBanner);
        }

        private void OnDrawCompleteInternal(GachaDrawResult result)
        {
            RefreshViewInternal();
            var resultPanel = CanvasManager.Instance.GetPanel<GachaResultPanel>();
            resultPanel?.Show(result);
        }

        private void OnDrawFailedInternal(string message) =>
            Debug.LogWarning($"[GachaPanel] {message}");
    }
}