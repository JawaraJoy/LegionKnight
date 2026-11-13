using UnityEngine;
using UnityEngine.UI;
using TMPro;
using LegionKnight;

namespace Rush
{
    public class GachaPanel : PanelView
    {
        [SerializeField] private TextMeshProUGUI m_BannerNameText;
        [SerializeField] private Image m_BannerImage;
        [SerializeField] private GachaDrawButtonUI m_DrawSingleButtonUI;
        [SerializeField] private GachaDrawButtonUI m_DrawMultiButtonUI;
        [SerializeField] private Button m_DetailButton;
        [SerializeField] private TextMeshProUGUI m_PityProgressText;
        [SerializeField] private Image m_PityFill;
        [SerializeField] private Transform m_BannerButtonContainer;
        [SerializeField] private GachaBannerButtonUI m_BannerButtonPrefab;
        

        [System.Serializable]
        private sealed class BannerNavigationField
        {
            [SerializeField] private Button m_Previous;
            [SerializeField] private Button m_Next;
            [SerializeField] private TextMeshProUGUI m_Page;
            [SerializeField] private TextMeshProUGUI m_Epithet;
            [SerializeField] private ScrollRect m_Rail;
            [SerializeField] private AspectRatioFitter m_ArtworkFrame;
            [SerializeField] private RectTransform m_ArtworkViewport;

            public Button Previous => m_Previous;
            public Button Next => m_Next;
            public TextMeshProUGUI Page => m_Page;
            public TextMeshProUGUI Epithet => m_Epithet;
            public ScrollRect Rail => m_Rail;
            public AspectRatioFitter ArtworkFrame => m_ArtworkFrame;
            public RectTransform ArtworkViewport => m_ArtworkViewport;
        }

        [SerializeField] private BannerNavigationField m_Navigation = new();
        private readonly System.Collections.Generic.List<GachaBannerButtonUI> m_BannerButtons = new();
        private bool m_Subscribed;
        private Rect? m_DefaultArtworkBounds;

        private GachaManager Manager => RushPlayer.Instance.GachaManager;

        protected override void ShowInternal()
        {
            base.ShowInternal();
            if (!IsShown) return;
            PopulateBannerButtonsInternal();
            RefreshViewInternal();
            SubscribeEventsInternal();
        }

        protected override void HideInternal()
        {
            UnsubscribeEventsInternal();
            base.HideInternal();
        }

        private void SubscribeEventsInternal()
        {
            if (m_Subscribed) return;
            m_Subscribed = true;
            if (m_Navigation.Previous != null) m_Navigation.Previous.onClick.AddListener(OnPreviousBannerInternal);
            if (m_Navigation.Next != null) m_Navigation.Next.onClick.AddListener(OnNextBannerInternal);
            Manager.OnDrawComplete.AddListener(OnDrawCompleteInternal);
            Manager.OnDrawFailed.AddListener(OnDrawFailedInternal);
            Manager.OnDrawRequested.AddListener(OnDrawRequestedInternal);

            if (m_DrawSingleButtonUI?.Button != null)
                m_DrawSingleButtonUI.Button.onClick.AddListener(OnDrawSingleClickedInternal);
            if (m_DrawMultiButtonUI?.Button != null)
                m_DrawMultiButtonUI.Button.onClick.AddListener(OnDrawMultiClickedInternal);
            if (m_DetailButton != null)
                m_DetailButton.onClick.AddListener(OnDetailClickedInternal);
        }

        private void UnsubscribeEventsInternal()
        {
            if (!m_Subscribed) return;
            m_Subscribed = false;
            if (m_Navigation.Previous != null) m_Navigation.Previous.onClick.RemoveListener(OnPreviousBannerInternal);
            if (m_Navigation.Next != null) m_Navigation.Next.onClick.RemoveListener(OnNextBannerInternal);
            Manager.OnDrawComplete.RemoveListener(OnDrawCompleteInternal);
            Manager.OnDrawFailed.RemoveListener(OnDrawFailedInternal);
            Manager.OnDrawRequested.RemoveListener(OnDrawRequestedInternal);

            if (m_DrawSingleButtonUI?.Button != null)
                m_DrawSingleButtonUI.Button.onClick.RemoveListener(OnDrawSingleClickedInternal);
            if (m_DrawMultiButtonUI?.Button != null)
                m_DrawMultiButtonUI.Button.onClick.RemoveListener(OnDrawMultiClickedInternal);
            if (m_DetailButton != null)
                m_DetailButton.onClick.RemoveListener(OnDetailClickedInternal);
        }
        public void RefreshView() => RefreshViewInternal();
        private void RefreshViewInternal()
        {
            var banner = Manager.ActiveBanner;
            if (banner == null) return;
            RefreshPresentationInternal(banner);

            if (m_PityProgressText != null)
                m_PityProgressText.text =
                    $"{Manager.PityTracker.FinalPityCounter}/{banner.FinalPityCount}";

            if (m_PityFill != null)
                m_PityFill.fillAmount = banner.FinalPityCount > 0
                    ? Mathf.Clamp01((float)Manager.PityTracker.FinalPityCounter / banner.FinalPityCount)
                    : 0f;

            int mainHeld = Player.Instance.CurrencyControl
                .GetCurrencyAmount(banner.DrawCostCurrency);

            RefreshDrawButtonInternal(m_DrawSingleButtonUI, banner, false, mainHeld);
            RefreshDrawButtonInternal(m_DrawMultiButtonUI, banner, true, mainHeld);
        }

        private void RefreshDrawButtonInternal(GachaDrawButtonUI buttonUI,
            GachaBannerConfig banner, bool isMulti, int mainHeld)
        {
            if (buttonUI == null) return;
            var breakdown = Manager.GetBreakdown(isMulti);
            buttonUI.Refresh(breakdown, banner, mainHeld);
        }

        private void PopulateBannerButtonsInternal()
        {
            if (m_BannerButtonContainer == null || m_BannerButtonPrefab == null) return;
            int count = 0;
            foreach (var banner in Manager.Banners)
            {
                if (banner == null) continue;
                if (count == m_BannerButtons.Count)
                    m_BannerButtons.Add(Instantiate(m_BannerButtonPrefab, m_BannerButtonContainer));
                m_BannerButtons[count].Setup(banner, OnBannerSelectedInternal);
                count++;
            }
            for (int i = m_BannerButtons.Count - 1; i >= count; i--)
            {
                var excess = m_BannerButtons[i];
                excess.gameObject.SetActive(false);
                Destroy(excess.gameObject);
                m_BannerButtons.RemoveAt(i);
            }
        }

        private void OnBannerSelectedInternal(GachaBannerConfig banner)
        {
            if (banner != Manager.ActiveBanner)
                Manager.SelectBanner(banner);
            RefreshViewInternal();
        }

        private void OnPreviousBannerInternal() => SelectAdjacentInternal(-1);
        private void OnNextBannerInternal() => SelectAdjacentInternal(1);

        private void SelectAdjacentInternal(int direction)
        {
            int count = m_BannerButtons.Count;
            if (count < 2) return;
            int index = m_BannerButtons.FindIndex(item => item.Banner == Manager.ActiveBanner);
            int next = (index + direction + count) % count;
            OnBannerSelectedInternal(m_BannerButtons[next].Banner);
        }

        private void RefreshPresentationInternal(GachaBannerConfig banner)
        {
            if (m_BannerNameText != null)
                m_BannerNameText.text = banner.HasPanelHeroName
                    ? banner.PanelHeroName.ToUpperInvariant() + "  <size=30%><cspace=0.1em>BANNER</cspace></size>"
                    : banner.BaseInfo.Name;
            if (m_Navigation.Epithet != null)
                m_Navigation.Epithet.text = banner.PanelEpithet?.ToUpperInvariant();

            RefreshArtworkInternal(banner);

            int activeIndex = m_BannerButtons.FindIndex(item => item.Banner == banner);
            foreach (var item in m_BannerButtons)
                item.SetActiveBanner(banner);
            if (m_Navigation.Page != null)
                m_Navigation.Page.text = $"{activeIndex + 1:00} / {m_BannerButtons.Count:00}";
            if (m_Navigation.Previous != null) m_Navigation.Previous.interactable = m_BannerButtons.Count > 1;
            if (m_Navigation.Next != null) m_Navigation.Next.interactable = m_BannerButtons.Count > 1;
            if (activeIndex >= 0) RevealBannerInternal(m_BannerButtons[activeIndex]);
        }

        private void RefreshArtworkInternal(GachaBannerConfig banner)
        {
            if (m_BannerImage == null) return;
            var artwork = banner.PanelArtwork;
            m_BannerImage.sprite = artwork;
            if (artwork == null || m_Navigation.ArtworkFrame == null) return;

            var viewport = m_Navigation.ArtworkViewport;
            if (viewport != null)
            {
                m_DefaultArtworkBounds ??= new Rect(viewport.anchorMin, viewport.anchorMax - viewport.anchorMin);
                Rect bounds = banner.PanelArtworkBounds;
                if (bounds.width <= 0f || bounds.height <= 0f) bounds = m_DefaultArtworkBounds.Value;
                viewport.anchorMin = bounds.min;
                viewport.anchorMax = bounds.max;
            }

            float visible = 1f - Mathf.Clamp(banner.PanelBottomCrop, 0f, 0.9f);
            m_Navigation.ArtworkFrame.aspectRatio = artwork.rect.width / (artwork.rect.height * visible);
            m_BannerImage.rectTransform.anchorMin = new Vector2(0f, 1f - 1f / visible);
        }

        private void RevealBannerInternal(GachaBannerButtonUI item)
        {
            var rail = m_Navigation.Rail;
            if (rail == null || !rail.isActiveAndEnabled) return;
            LayoutRebuilder.ForceRebuildLayoutImmediate(rail.content);
            var rect = (RectTransform)item.transform;
            float viewportHeight = rail.viewport.rect.height;
            float overflow = rail.content.rect.height - viewportHeight;
            if (overflow <= 0f) return;
            float top = -rect.anchoredPosition.y - rect.rect.height * (1f - rect.pivot.y);
            float offset = (1f - rail.verticalNormalizedPosition) * overflow;
            float target = Mathf.Clamp(offset, top + rect.rect.height - viewportHeight, top);
            if (!Mathf.Approximately(offset, target))
            {
                rail.StopMovement();
                rail.verticalNormalizedPosition = 1f - Mathf.Clamp01(target / overflow);
            }
        }

        private void OnDrawSingleClickedInternal() => Manager.RequestDrawSingle();
        private void OnDrawMultiClickedInternal() => Manager.RequestDrawMulti();

        private void OnDrawRequestedInternal(GachaConfirmData data)
        {
            var confirmPanel = CanvasManager.Instance.GetPanel<CurrencyConfirmationPanel>();
            confirmPanel?.ShowConfirmation(data, () => Manager.ConfirmDraw(data));
        }

        private void OnDetailClickedInternal()
        {
            var detailPanel = CanvasManager.Instance.GetPanel<BannerDetailPanel>();
            detailPanel?.Show(Manager.ActiveBanner);
        }

        private void OnDrawCompleteInternal(CollectibleResultData result)
        {
            RefreshViewInternal();
            var previewPanel = CanvasManager.Instance.GetPanel<GachaPreviewPanel>();
            previewPanel?.Show(result);
        }

        private void OnDrawFailedInternal(string message) =>
            Debug.LogWarning($"[GachaPanel] {message}");
    }
}