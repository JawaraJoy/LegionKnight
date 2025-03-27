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
            m_CounterSlide.value = rate;
        }
        public void SetCounterText(int counter)
        {
            m_CounterText.text = counter.ToString();
        }
    }
    public partial class BannerPanel
    {
        public void SetCounterSlideValue(float rate)
        {
            GetBinding<DrawCounterView>().SetCounterSlideValue(rate);
        }
        public void SetCounterText(int counter)
        {
            GetBinding<DrawCounterView>().SetCounterText(counter);
        }
    }
    public partial class GachaManagerAgent
    {
        public void SetCounterSlideValue(GachaBanner banner)
        {
            GetBannerPanel().SetCounterSlideValue(banner.GetDrawCountRate());
            GetBannerPanel().SetCounterText(banner.TotalDraws);
        }
    }
}
