using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace LegionKnight
{
    public class BosHealthBarView : UIView
    {
        [SerializeField]
        private TextMeshProUGUI m_BosName;
        [SerializeField]
        private Slider m_Slider;
        [SerializeField]
        private Image m_Fill;

        public void SetBosName(BosDefinition defi)
        {
            if (m_BosName != null)
            {
                m_BosName.text = defi.Label;
            }
            else
            {
                Debug.LogWarning("BosName TextMeshProUGUI component is not assigned.");
            }
            
        }
        public void SetHealth(float rate)
        {
            m_Slider.value = rate;
            m_Fill.fillAmount = rate;
        }
    }
}
