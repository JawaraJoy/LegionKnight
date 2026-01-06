using TMPro;
using UnityEngine;

namespace LegionKnight
{
    public class StatView : UIView
    {
        [SerializeField]
        private TextMeshProUGUI m_CurrentText;
        [SerializeField]
        private TextMeshProUGUI m_NextText;
        [SerializeField]
        private GameObject m_NextContent;

        public void SetCurrentValue(int value)
        {
            if (m_CurrentText != null)
            {
                m_CurrentText.text = value.ToString();
            }
        }
        public void SetNextValue(int value)
        {
            if (m_NextText != null)
            {
                m_NextText.text = value.ToString();
            }
        }
        public void ShowNextValue()
        {
            if (m_NextContent != null)
            {
                m_NextContent.SetActive(true);
            }
        }
        public void HideNextValue()
        {
            if (m_NextContent != null)
            {
                m_NextContent.SetActive(false);
            }
        }
    }
}
