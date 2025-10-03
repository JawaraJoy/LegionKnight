using UnityEngine;
using UnityEngine.UI;

namespace LegionKnight
{
    public partial class ProfilePictView : UIView
    {
        [SerializeField]
        private Image m_ProfileIcon;
        [SerializeField]
        private Image m_ProfileBorder;
        [SerializeField]
        private Image m_ProfileBackground;

        private CustomProfile m_Profile;

        private CustomProfile GetCustomProfile()
        {
            if (m_Profile == null)
            {
                m_Profile = Player.Instance.CustomProfile;
            }
            return m_Profile;
        }
        private void OnEnable()
        {
            InitInternal();
        }
        private void Awake()
        {
            Player.Instance.CustomProfile.AddProfilePictView(this);
        }
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

        public void Init()
        {
            InitInternal();
        }

        private void InitInternal()
        {
            CustomImageDefinition icon = GetCustomProfile().UsedIcon;
            if (icon != null)
            {
                m_ProfileIcon.sprite = icon.Icon;
            }
            CustomImageDefinition frame = GetCustomProfile().UsedFrame;
            if (frame != null)
            {
                m_ProfileBackground.sprite = frame.Icon;
            }
        }
    }
}
