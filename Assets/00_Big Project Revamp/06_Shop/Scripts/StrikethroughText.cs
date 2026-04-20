using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Rush
{
    // Attach on the same GameObject as TextMeshProUGUI
    // Assign m_StrikethroughLine to a child Image that acts as the strike line
    [RequireComponent(typeof(TextMeshProUGUI))]
    public class StrikethroughText : MonoBehaviour
    {
        [SerializeField] private Image m_StrikethroughLine;

        [SerializeField] private float m_LineHeight = 2.5f;
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

        public void SetText(string text)
        {
            if (m_Text == null) return;
            m_Text.text = text;

            // Force TMP to rebuild mesh so textBounds is accurate before we read it
            m_Text.ForceMeshUpdate();
            RefreshLineInternal();
        }

        public void SetVisible(bool visible)
        {
            gameObject.SetActive(visible);
        }

        // Recalculate when layout changes (e.g. parent resized)
        private void OnRectTransformDimensionsChange()
        {
            if (!gameObject.activeInHierarchy) return;
            RefreshLineInternal();
        }

        private void RefreshLineInternal()
        {
            if (m_LineRect == null || m_Text == null) return;

            Bounds bounds = m_Text.textBounds;

            // If text is empty textBounds can be zero — hide the line
            if (bounds.size.x <= 0f)
            {
                m_StrikethroughLine.gameObject.SetActive(false);
                return;
            }

            m_StrikethroughLine.gameObject.SetActive(true);

            m_LineRect.anchorMin = new Vector2(0.5f, 0.5f);
            m_LineRect.anchorMax = new Vector2(0.5f, 0.5f);
            m_LineRect.pivot = new Vector2(0.5f, 0.5f);
            m_LineRect.anchoredPosition = new Vector2(
                bounds.center.x,
                bounds.center.y + m_VerticalOffset);
            m_LineRect.sizeDelta = new Vector2(bounds.size.x, m_LineHeight);
        }
    }
}