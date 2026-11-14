using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Rush
{
    public sealed class PatchNoteRow : MonoBehaviour
    {
        [SerializeField] private TMP_Text m_Heading, m_Body;
        [SerializeField] private Image m_Paper, m_Wheel, m_Highlight, m_Divider;
        [SerializeField] private LayoutElement m_Layout;

        internal void Display(PatchNoteBlock block, float width)
        {
            bool section = block.Kind == PatchNoteKind.Section;
            bool featured = block.Kind == PatchNoteKind.Entry &&
                string.Equals(block.Heading, "Spin Wheel", System.StringComparison.OrdinalIgnoreCase);
            m_Paper.enabled = section;
            m_Wheel.enabled = m_Highlight.enabled = featured;
            m_Divider.enabled = block.Kind == PatchNoteKind.Entry && !featured;
            float x = featured ? 72f : section ? 26f : 2f;
            float textWidth = Mathf.Max(1f, width - x - (featured ? 10f : 5f));
            float top = featured ? 12f : 1f;
            if (block.Heading.Length > 0)
            {
                m_Heading.gameObject.SetActive(true);
                m_Heading.fontSize = section ? 20 : 17;
                top += Place(m_Heading, block.Heading, x, top, textWidth) + (section ? 0f : 4f);
            }
            else m_Heading.gameObject.SetActive(false);
            m_Body.gameObject.SetActive(block.Body.Length > 0);
            if (block.Body.Length > 0) top += Place(m_Body, block.Body, x, top, textWidth);
            float height = featured ? Mathf.Max(72f, top + 12f) : top + (section ? 6f : 9f);
            m_Layout.preferredHeight = Mathf.Ceil(height);
        }

        private float Place(TMP_Text label, string text, float x, float y, float width)
        {
            label.text = text;
            float height = Mathf.Ceil(label.GetPreferredValues(text, width, Mathf.Infinity).y) + 2f;
            label.rectTransform.sizeDelta = new Vector2(width, height);
            label.rectTransform.anchoredPosition = new Vector2(x, -y);
            return height;
        }
    }
}
