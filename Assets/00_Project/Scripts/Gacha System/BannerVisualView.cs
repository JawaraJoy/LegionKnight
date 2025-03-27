using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace LegionKnight
{
    public partial class BannerVisualView : UIView
    {
        [SerializeField]
        private Image m_VisualImage;
        [SerializeField]
        private TextMeshProUGUI m_BannerName;

        public void SetBannerVisual(Sprite visual, string nam)
        {
            m_VisualImage.sprite = visual;
            m_BannerName.text = nam;
        }
    }
    public partial class BannerPanel
    {
        public void SetBannerVisual(Sprite visual, string nam)
        {
            GetBinding<BannerVisualView>().SetBannerVisual(visual, nam);
        }
    }
    public partial class GachaManagerAgent
    {
        public void SetBannerVisual(GachaBanner banner)
        {
            Sprite visual = banner.VisualBanner;
            string n = banner.Definition.name;
            GetBannerPanel().SetBannerVisual(visual, n);
        }
    }
}
