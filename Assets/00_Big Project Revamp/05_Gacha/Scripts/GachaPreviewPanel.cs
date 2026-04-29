using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using LegionKnight;
using UnityEngine.Events;

namespace Rush
{
    // Previews gacha results one by one
    // If the result is a HeroUnitConfig → opens GachaHeroRevealPanel first
    // After all results previewed → opens CollectibleResultPanel
    public class GachaPreviewPanel : PanelView
    {
        [SerializeField] private GachaPreviewItemUI m_PreviewItemUI;

        [Header("Navigation")]
        [SerializeField] private Button m_NextButton;
        [SerializeField] private TextMeshProUGUI m_CounterText;   // e.g. "3 / 10"

        [Header("Pity Notice")]
        [SerializeField] private GameObject m_PityNoticeObject;
        [SerializeField]
        private UnityEvent m_OnNextClicked; // Optional event for additional logic when Next is clicked (e.g. play sound)
        private CollectibleResultData m_Result;
        private List<CollectibleResultEntry> m_Entries;
        private int m_CurrentIndex;

        // ── Lifecycle ─────────────────────────────────────────────────────────

        protected override void ShowInternal()
        {
            base.ShowInternal();
            if (m_NextButton != null) m_NextButton.onClick.AddListener(OnNextClickedInternal);
        }

        protected override void HideInternal()
        {
            if (m_NextButton != null) m_NextButton.onClick.RemoveListener(OnNextClickedInternal);
            base.HideInternal();
        }

        // ── Public ────────────────────────────────────────────────────────────

        public void Show(CollectibleResultData result)
        {
            m_Result = result;
            m_Entries = new List<CollectibleResultEntry>(result.Entries);
            m_CurrentIndex = 0;

            Show();
            ShowEntryInternal(m_CurrentIndex);
        }

        // ── Entry Display ─────────────────────────────────────────────────────

        private void ShowEntryInternal(int index)
        {
            if (m_Entries == null || index >= m_Entries.Count) return;
            m_OnNextClicked?.Invoke();
            var entry = m_Entries[index];

            // Update preview item UI with splash image
            m_PreviewItemUI?.Setup(entry);

            // Update counter
            if (m_CounterText != null)
                m_CounterText.text = $"{index + 1} / {m_Entries.Count}";

            // Pity notice only on last item
            if (m_PityNoticeObject != null)
                m_PityNoticeObject.SetActive(
                    index == m_Entries.Count - 1 && m_Result.WasSpecialDrop);

            // If this is a hero → open reveal panel first
            // Next button is blocked until reveal panel is closed
            if (entry.Collectible is HeroUnitConfig heroConfig)
            {
                SetNextButtonInteractableInternal(false);
                OpenHeroRevealInternal(heroConfig);
            }
            else
            {
                SetNextButtonInteractableInternal(true);
            }
        }

        private void OpenHeroRevealInternal(HeroUnitConfig heroConfig)
        {
            var revealPanel = CanvasManager.Instance.GetPanel<GachaHeroRevealPanel>();
            revealPanel?.Show(heroConfig, OnHeroRevealClosedInternal);
        }

        private void OnHeroRevealClosedInternal()
        {
            // Hero reveal done — re-enable next button so player can continue
            SetNextButtonInteractableInternal(true);
        }

        // ── Next ──────────────────────────────────────────────────────────────

        private void OnNextClickedInternal()
        {
            m_CurrentIndex++;

            if (m_CurrentIndex < m_Entries.Count)
            {
                ShowEntryInternal(m_CurrentIndex);
                
            }
            else
            {
                // All entries previewed — show full result panel
                FinishPreviewInternal();
            }
        }

        private void FinishPreviewInternal()
        {
            Hide();
            var resultPanel = CanvasManager.Instance.GetPanel<GachaResultPanel>();
            resultPanel?.Show(m_Result);
        }

        private void SetNextButtonInteractableInternal(bool interactable)
        {
            if (m_NextButton != null) m_NextButton.interactable = interactable;
        }
    }
}