using TMPro;
using UnityEngine;
using UnityEngine.UI;
using LegionKnight;

namespace Rush
{
    /// <summary>
    /// One tab: a Button + label pair that controls a UIView panel.
    /// Add this as a component on each tab button GameObject.
    /// Wire m_View to the matching panel; call group.OnTabClicked(myIndex) from m_Button.onClick.
    /// </summary>
    public class TabEntry : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Button m_Button;
        [SerializeField] private TextMeshProUGUI m_Label;
        [SerializeField] private UIView m_View;

        [Header("Config")]
        [SerializeField] private string m_TabLabel;
        [SerializeField] private bool m_IsEnabled = true;

        public bool IsEnabled => m_IsEnabled;

        private void Awake()
        {
            if (m_Label != null)
                m_Label.text = m_TabLabel;

            SetStateInternal(false);
        }

        /// <summary>
        /// active = true  → disable button interaction, show highlight, show view
        /// active = false → enable button interaction, hide highlight, hide view
        /// </summary>
        public void SetState(bool active)
        {
            SetStateInternal(active);
        }

        private void SetStateInternal(bool active)
        {
            // Button interactability: active tab can't be re-clicked
            if (m_Button != null)
                m_Button.interactable = !active;

            // Panel visibility
            if (m_View != null)
            {
                if (active) m_View.Show();
                else m_View.Hide();
            }
        }
    }
}