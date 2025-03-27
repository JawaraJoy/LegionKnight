using UnityEngine;
using UnityEngine.UI;

namespace LegionKnight
{
    public partial class DrawCounterView : UIView
    {
        [SerializeField]
        private Slider m_CounterSlide;

        public void SetCounterSlideValue(float rate)
        {
            m_CounterSlide.value = rate;
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
