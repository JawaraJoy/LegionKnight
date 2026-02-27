using UnityEngine;
using LegionKnight;
using UnityEngine.UI;
using TMPro;

namespace Rush
{
    public class HealthSliderView : UIView
    {
        [SerializeField]
        private Slider m_HealthSlider;
        [SerializeField]
        private TextMeshProUGUI m_HealthText;

        public void SetHealth(int current, int max)
        {
            m_HealthText.text = $"{current}/{max}";
            float rate = (float) current / max ;

            m_HealthSlider.value = rate ;
        }
    }
}
