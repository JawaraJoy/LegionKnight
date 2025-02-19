using UnityEngine;
using UnityEngine.UI;

namespace LegionKnight
{
    public partial class PlayerProfileView : UIView
    {
        [SerializeField]
        private Image m_ProfileIcon;
        [SerializeField]
        private Image m_ProfileBorder;
        [SerializeField]
        private Image m_ProfileBackground;

        public void SetProfileIcon(Sprite val)
        {
            m_ProfileIcon.sprite = val;
        }
        public void SetProfileBorder(Sprite val)
        {
            m_ProfileBorder.sprite = val;
        }
        public void SetProfileBackground(Sprite val)
        {
            m_ProfileBackground.sprite = val;
        }
    }
}
