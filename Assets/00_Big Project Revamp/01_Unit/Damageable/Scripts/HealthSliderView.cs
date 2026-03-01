using UnityEngine;
using UnityEngine.UI;

namespace Rush
{
    public class HealthSliderView : SliderView
    {
        [SerializeField]
        private Image m_HealthFillImage;
        override public void SetSlider(int current, int max)
        {
            base.SetSlider(current, max);
            Color healthColor = RushGameManager.Instance.GameConfig.HealthColorConfig.GetHealthColor(m_Rate);
            m_HealthFillImage.color = healthColor;
        }
    }
}
