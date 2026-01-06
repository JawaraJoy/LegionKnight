// CLEAN REWRITE – WheelItemUI
// Minimal, clean, modular. Uses m_ prefix. Handles icon, label, button, and canvas group.

using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace LegionKnight
{
    public class WheelItemUI : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Image m_Icon;
        [SerializeField] private TextMeshProUGUI m_Label;
        [SerializeField] private Button m_Button;

        public RectTransform Rect { get; private set; }
        public CanvasGroup CanvasGroup { get; private set; }
        public Button Button => m_Button;

        private void Awake()
        {
            Rect = GetComponent<RectTransform>();

            // Auto-create CanvasGroup if not present
            CanvasGroup = GetComponent<CanvasGroup>();
            if (CanvasGroup == null)
                CanvasGroup = gameObject.AddComponent<CanvasGroup>();
        }

        public void Setup(WheelItemDefinition def)
        {
            if (def == null)
                return;

            if (m_Icon != null)
                m_Icon.sprite = def.Icon;

            if (m_Label != null)
                m_Label.text = def.Label;
        }
    }
}