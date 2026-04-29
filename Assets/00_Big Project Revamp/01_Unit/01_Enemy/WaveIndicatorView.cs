using UnityEngine;
using UnityEngine.UI;

namespace Rush
{
    public class WaveIndicatorView : SliderView
    {
        [SerializeField]
        private Image m_WaveIcon;

        public void SetWaveIcon(Sprite icon)
        {
            m_WaveIcon.sprite = icon;
        }
    }
}
