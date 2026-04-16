using UnityEngine;
using UnityEngine.UI;
using TMPro;
using LegionKnight;

namespace Rush
{
    public class GachaPanel : PanelView
    {
        // Sub-view dalam satu panel ini → pakai m_Bindings
        // Panel lain (Detail, Confirm, Result) → akses via CanvasManager
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

        // ── Lifecycle ─────────────────────────────────────────────────────────
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
            if (m_DrawSingleButton != null) m_DrawSingleButton.onClick.AddListener(OnDrawSingleClickedInternal);
            if (m_DrawMultiButton != null) m_DrawMultiButton.onClick.AddListener(OnDrawMultiClickedInternal);
            if (m_DetailButton != null) m_DetailButton.onClick.AddListener(OnDetailClickedInternal);
        }

        private void UnsubscribeEventsInternal()
        {
            Manager.OnDrawComplete.RemoveListener(OnDrawCompleteInternal);
            Manager.OnDrawFailed.RemoveListener(OnDrawFailedInternal);
            if (m_DrawSingleButton != null) m_DrawSingleButton.onClick.RemoveListener(OnDrawSingleClickedInternal);
            if (m_DrawMultiButton != null) m_DrawMultiButton.onClick.RemoveListener(OnDrawMultiClickedInternal);
            if (m_DetailButton != null) m_DetailButton.onClick.RemoveListener(OnDetailClickedInternal);
        }

        // ── View ─────────────────────────────────────────────────────────────
        private void RefreshViewInternal()
        {
            var banner = Manager.ActiveBanner;
            if (banner == null) return;

            if (m_BannerImage != null) m_BannerImage.sprite = banner.BannerSplashSprite;
            if (m_SingleCostText != null) m_SingleCostText.text = Manager.GetDrawCost(false).ToString();
            if (m_MultiCostText != null) m_MultiCostText.text = Manager.GetDrawCost(true).ToString();
            if (m_PityProgressText != null)
                m_PityProgressText.text =
                    $"{Manager.PityTracker.FinalPityCounter}/{banner.FinalPityCount}";

            if (m_DrawSingleButton != null) m_DrawSingleButton.interactable = Manager.CanAffordDraw(false);
            if (m_DrawMultiButton != null) m_DrawMultiButton.interactable = Manager.CanAffordDraw(true);
        }

        private void PopulateBannerButtonsInternal()
        {
            if (m_BannerButtonContainer == null || m_BannerButtonPrefab == null) return;

            foreach (Transform child in m_BannerButtonContainer)
                Destroy(child.gameObject);

            foreach (var banner in Manager.Banners)
            {
                var btn = Instantiate(m_BannerButtonPrefab, m_BannerButtonContainer);
                btn.Setup(banner, OnBannerSelectedInternal);
            }
        }

        // ── Callbacks ────────────────────────────────────────────────────────
        private void OnBannerSelectedInternal(GachaBannerConfig banner)
        {
            Manager.SelectBanner(banner);
            RefreshViewInternal();
        }

        private void OnDrawSingleClickedInternal()
        {
            int cost = Manager.GetDrawCost(false);
            var confirmPanel = CanvasManager.Instance.GetPanel<CurrencyConfirmationPanel>();
            confirmPanel?.ShowConfirmation(
                "Draw 1x?", cost, Manager.ActiveBanner.DrawCostCurrency,
                () => Manager.DrawSingle());
        }

        private void OnDrawMultiClickedInternal()
        {
            int cost = Manager.GetDrawCost(true);
            var confirmPanel = CanvasManager.Instance.GetPanel<CurrencyConfirmationPanel>();
            confirmPanel?.ShowConfirmation(
                $"Draw {Manager.ActiveBanner.MultiDrawCount}x?", cost,
                Manager.ActiveBanner.DrawCostCurrency,
                () => Manager.DrawMulti());
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

        private void OnDrawFailedInternal(string message)
        {
            Debug.LogWarning($"[GachaPanel] {message}");
        }
    }
}