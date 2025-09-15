using UnityEngine;
using UnityEngine.UI;

namespace LegionKnight
{
    public partial class HeroUniquePlatformView : UIView
    {
        [SerializeField]
        private Image m_Icon;

        public void Init()
        {
            Sprite icon = Player.Instance.GetHeroStandbyPlatform().Icon;         
            m_Icon.sprite = icon;
        }
    }
}
