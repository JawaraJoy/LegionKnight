using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace LegionKnight
{
    public partial class DrawCounterView : UIView
    {
        [SerializeField]
        private Slider m_CounterSlide;
        [SerializeField]
        private TextMeshProUGUI m_CounterText;
        public void SetCounterSlideValue(float rate)
        {
            if (!m_CounterSlide.enabled) return;
            m_CounterSlide.value = rate;
        }
        public void SetCounterText(int currentDrawCount, int guaranteedDraw)
        {
            m_CounterText.text = $"{currentDrawCount}/{guaranteedDraw}";
        }
    }
    public partial class BannerPanel
    {
        public void SetCounterSlideValue(float rate)
        {
            GetBinding<DrawCounterView>().SetCounterSlideValue(rate);
        }
    }
    public partial class GachaManagerAgent
    {
        public void SetCounterSlideValue(GachaBanner banner)
        {
            GetBannerPanel().SetCounterSlideValue(banner.GetDrawCountRate());
            
        }
    }
}
