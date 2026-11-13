using LegionKnight;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Rush
{
    public class GachaPreviewPanel : PanelView
    {
        [SerializeField] private GachaPreviewItemUI m_PreviewItemUI;
        [SerializeField] private Button m_NextButton;
        [SerializeField] private Button m_SkipButton;
        [SerializeField] private TextMeshProUGUI m_CounterText;
        [SerializeField] private TextMeshProUGUI m_BannerText;
        [SerializeField] private GameObject m_PityNoticeObject;
        [SerializeField] private UnityEvent m_OnNextClicked;

        private CollectibleResultData m_Result;
        private GachaBannerConfig m_Banner;
        private GachaHeroRevealPanel m_Reveal;
        private int m_CurrentIndex;

        public void Show(CollectibleResultData result)
        {
            if (result == null || result.Entries.Count == 0) return;
            if (IsShown) HideInternal();
            m_Result = result;
            m_Banner = RushPlayer.Instance.GachaManager.ResolveResultBanner(result.Source);
            m_CurrentIndex = 0;
            ShowInternal();
            if (IsShown) ShowEntryInternal();
        }

        protected override void ShowInternal()
        {
            base.ShowInternal();
            if (!IsShown) return;
            m_NextButton.onClick.RemoveListener(OnNextClickedInternal);
            m_NextButton.onClick.AddListener(OnNextClickedInternal);
            m_SkipButton.onClick.RemoveListener(SkipToResultsInternal);
            m_SkipButton.onClick.AddListener(SkipToResultsInternal);
        }

        protected override void HideInternal()
        {
            m_NextButton.onClick.RemoveListener(OnNextClickedInternal);
            m_SkipButton.onClick.RemoveListener(SkipToResultsInternal);
            m_Result = null;
            if (m_Reveal != null && m_Reveal.IsShown) m_Reveal.Hide();
            m_Reveal = null;
            base.HideInternal();
        }

        private void ShowEntryInternal()
        {
            var entry = m_Result.Entries[m_CurrentIndex];
            m_BannerText.text = m_Banner != null ? m_Banner.BaseInfo.Name : string.Empty;
            m_CounterText.text = $"{m_CurrentIndex + 1} / {m_Result.Entries.Count}";
            m_PityNoticeObject.SetActive(m_CurrentIndex == m_Result.Entries.Count - 1 && m_Result.WasSpecialDrop);
            m_NextButton.interactable = true;
            if (entry.Collectible is HeroUnitConfig hero)
            {
                m_PreviewItemUI.PrepareForReveal(entry);
                m_Reveal = CanvasManager.Instance.GetPanel<GachaHeroRevealPanel>();
                if (m_Reveal != null)
                {
                    m_NextButton.interactable = false;
                    m_Reveal.Show(hero, m_Banner, OnHeroRevealClosedInternal, SkipToResultsInternal);
                    if (!m_Reveal.IsShown) OnHeroRevealClosedInternal();
                }
                else m_PreviewItemUI.Setup(entry);
            }
            else m_PreviewItemUI.Setup(entry);
            OnNextClickedInvoke();
        }

        private void OnHeroRevealClosedInternal()
        {
            m_Reveal = null;
            if (m_Result == null || !IsShown) return;
            m_PreviewItemUI.Setup(m_Result.Entries[m_CurrentIndex]);
            m_NextButton.interactable = true;
        }

        public void Advance() => OnNextClickedInternal();

        private void OnNextClickedInternal()
        {
            if (m_Result == null || !m_NextButton.interactable) return;
            if (++m_CurrentIndex < m_Result.Entries.Count)
            {
                ShowEntryInternal();
                return;
            }

            FinishPreviewInternal(false);
        }

        private void SkipToResultsInternal() => FinishPreviewInternal(true);

        private void FinishPreviewInternal(bool showAllAtOnce)
        {
            if (m_Result == null || !IsShown) return;
            var summary = CanvasManager.Instance.GetPanel<GachaResultPanel>();
            if (summary == null) return;
            var result = m_Result;
            HideInternal();
            if (showAllAtOnce) summary.ShowAll(result);
            else summary.Show(result);
        }

        private void OnNextClickedInvoke() => m_OnNextClicked?.Invoke();
    }
}
