using UnityEngine;
using UnityEngine.UI;

namespace Rush
{
    public class WaveIndicatorView : SliderView
    {
        [SerializeField]
        private Image m_WaveIcon;

        public override void SetSlider(int current, int max)
        {
            base.SetSlider(current, max);
        }
        public void SetWaveIcon(Sprite icon)
        {
            m_WaveIcon.sprite = icon;
        }
        override protected void ReduceDuration(float value)
        {
            //base.ReduceDuration(value);
        }
    }
}
