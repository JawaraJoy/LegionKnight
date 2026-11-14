using System;
using System.Collections.Generic;
using LegionKnight;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Rush
{
    public partial class PatchNotePanel : PanelView
    {
        [SerializeField] private TextMeshProUGUI m_TitlePatchText;
        [SerializeField] private TextMeshProUGUI m_DescriptionPatchText;

        [Serializable]
        private sealed class WelcomeField
        {
            public RectTransform Board;
            public ScrollRect Scroll;
            public PatchNoteRow Template;
            public Button Continue;
            public WelcomePanel Welcome;
        }

        [SerializeField] private WelcomeField m_Welcome;
        private readonly List<PatchNoteRow> m_Rows = new();

        protected override void ShowInternal()
        {
            base.ShowInternal();
            if (IsShown) RefreshInternal();
        }

        public void Refresh() => RefreshInternal();

        private void RefreshInternal()
        {
            var manager = GameManager.Instance?.PatchNoteManager;
            var note = manager != null ? manager.CurrentPatchNote : null;
            string title = note != null ? note.Label ?? string.Empty : string.Empty;
            string body = note != null ? note.Description : string.Empty;
            m_TitlePatchText.text = title;
            if (m_Welcome?.Template == null)
            {
                if (m_DescriptionPatchText != null) m_DescriptionPatchText.text = body;
                return;
            }

            FitInternal();
            m_Welcome.Continue.interactable = true;
            var scroll = m_Welcome.Scroll;
            Canvas.ForceUpdateCanvases();
            float width = scroll.content.rect.width;
            int count = 0;
            foreach (var block in PatchNoteText.Read(title, body))
                DisplayRowInternal(count++, block, width);
            if (count == 0)
                DisplayRowInternal(count++, new PatchNoteBlock(PatchNoteKind.Paragraph, string.Empty, "No patch notes available."), width);
            for (int i = count; i < m_Rows.Count; i++) m_Rows[i].gameObject.SetActive(false);
            LayoutRebuilder.ForceRebuildLayoutImmediate(scroll.content);
            Canvas.ForceUpdateCanvases();
            scroll.StopMovement();
            scroll.verticalNormalizedPosition = 1f;
            // The open tween starts at zero scale. Reset in the viewport's local space, too.
            scroll.content.anchoredPosition = Vector2.zero;
        }

        private void DisplayRowInternal(int index, PatchNoteBlock block, float width)
        {
            if (index == m_Rows.Count)
                m_Rows.Add(Instantiate(m_Welcome.Template, m_Welcome.Scroll.content, false));
            var row = m_Rows[index];
            row.gameObject.SetActive(true);
            row.Display(block, width);
        }

        private void OnRectTransformDimensionsChange() => FitInternal();

        private void FitInternal()
        {
            if (m_Welcome?.Board == null) return;
            var board = m_Welcome.Board;
            var area = ((RectTransform)board.parent).rect.size;
            float scale = Mathf.Min(area.x / board.rect.width, area.y / board.rect.height);
            if (scale > 0) board.localScale = Vector3.one * scale;
        }

        // The tween may send completion more than once. Advance the welcome flow only once.
        public void FinishClose()
        {
            if (m_Welcome.Welcome.IsShown && !m_Welcome.Continue.interactable)
                m_Welcome.Welcome.Hide();
        }
    }
}
