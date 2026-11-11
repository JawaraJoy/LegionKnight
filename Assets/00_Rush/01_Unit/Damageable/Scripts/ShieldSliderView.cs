using UnityEngine;

namespace Rush
{
    public class ShieldSliderView : SliderView
    {
        override public void SetSlider(int current, int max)
        {
            base.SetSlider(current, max);
            if (current > 0)
            {
                m_Rate = Mathf.Clamp(m_Rate, 0.07f, 1f);
                m_Slider.value = m_Rate;
            }
        }
    }
}
