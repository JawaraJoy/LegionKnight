using UnityEngine;
using UnityEngine.UI;

namespace LegionKnight
{
    public partial class DrawRewardView : UIView
    {
        [SerializeField]
        private Image m_Icon;

        public void SetIcon(Sprite set)
        {
            m_Icon.sprite = set;
        }
    }
    public partial class DrawCounterView
    {
        [SerializeField]
        private DrawRewardView m_MainDrawRewardView;
        public void SetIcon(Sprite set)
        {
            m_MainDrawRewardView.SetIcon(set);
        }
    }
    public partial class BannerPanel
    {
        public void SetMainDrawRewardIcon(Sprite set)
        {
            GetBinding<DrawCounterView>().SetIcon(set);
        }
    }
    public partial class GachaManagerAgent
    {
        public void SetMainDrawRewardIcon(GachaBanner banner)
        {
            GetBannerPanel().SetMainDrawRewardIcon(banner.SmallVisualBanner);
        }
    }
}
