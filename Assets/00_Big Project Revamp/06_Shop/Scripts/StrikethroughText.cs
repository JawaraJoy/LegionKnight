using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Rush
{
    // Attach on the same GameObject as the TMP text
    // Child hierarchy expected:
    // OriginalPriceGroup
    // ├── [this component + TextMeshProUGUI]
    // └── StrikethroughLine (Image)
    [RequireComponent(typeof(TextMeshProUGUI))]
    public class StrikethroughText : MonoBehaviour
    {
        [SerializeField] private Image m_StrikethroughLine;

        // Line thickness in pixels
        [SerializeField] private float m_LineHeight = 2.5f;

        // Vertical offset from text center — positive moves line up
        [SerializeField] private float m_VerticalOffset = 0f;

        private TextMeshProUGUI m_Text;
        private RectTransform m_LineRect;

        private void Awake()
        {
            m_Text = GetComponent<TextMeshProUGUI>();
            m_LineRect = m_StrikethroughLine != null
                ? m_StrikethroughLine.GetComponent<RectTransform>()
                : null;
        }

        private void OnEnable()
        {
            // TMPro fires this after text + layout is updated
            TMPro_EventManager.TEXT_CHANGED_EVENT.Add(OnTextChangedInternal);
        }

        private void OnDisable()
        {
            TMPro_EventManager.TEXT_CHANGED_EVENT.Remove(OnTextChangedInternal);
        }

        public void SetText(string text)
        {
            if (m_Text != null) m_Text.text = text;
            // Force layout so bounds are immediately correct
            Canvas.ForceUpdateCanvases();
            RefreshLineInternal();
        }

        public void SetVisible(bool visible)
        {
            gameObject.SetActive(visible);
        }

        private void OnTextChangedInternal(Object obj)
        {
            if (obj != m_Text) return;
            RefreshLineInternal();
        }

        private void RefreshLineInternal()
        {
            if (m_LineRect == null || m_Text == null) return;

            // Get the rendered text bounds in local space
            m_Text.ForceMeshUpdate();
            Bounds bounds = m_Text.textBounds;

            // Width matches text width exactly
            float width = bounds.size.x;

            // Center of text vertically in local space + optional offset
            float centerY = bounds.center.y + m_VerticalOffset;

            // Apply to line rect
            m_LineRect.anchorMin = new Vector2(0.5f, 0.5f);
            m_LineRect.anchorMax = new Vector2(0.5f, 0.5f);
            m_LineRect.pivot = new Vector2(0.5f, 0.5f);
            m_LineRect.anchoredPosition = new Vector2(bounds.center.x, centerY);
            m_LineRect.sizeDelta = new Vector2(width, m_LineHeight);
        }
    }
}