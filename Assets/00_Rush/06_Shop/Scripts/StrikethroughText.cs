using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Rush
{
    [RequireComponent(typeof(TextMeshProUGUI))]
    public class StrikethroughText : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI m_Text;
        [SerializeField] private Image m_StrikethroughLine;

        public void SetText(string text)
        {
            if (m_Text != null) m_Text.text = text;
        }

        public void SetVisible(bool visible)
        {
            gameObject.SetActive(visible);
        }

        public void SetLineVisible(bool visible)
        {
            if (m_StrikethroughLine != null)
                m_StrikethroughLine.gameObject.SetActive(visible);
        }
    }
}