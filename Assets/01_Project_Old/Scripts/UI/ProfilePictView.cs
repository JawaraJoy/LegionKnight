using Rush;
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
        [SerializeField]
        private Animator m_AnimatorIcon;
        [SerializeField]
        private Animator m_AnimatorFrame;
        private CustomProfile m_Profile;
        [SerializeField]
        private ProfileViewNoticeButton m_NoticeButton;
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
        private void OnDestroy()
        {
            Player.Instance.CustomProfile.RemoveProfilePictView(this);
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
            CustomImageDefinition frame = GetCustomProfile().UsedFrame;
            if (icon != null)
            {
                if (icon.runtimeAnim == null)
                {
                    m_ProfileIcon.sprite = icon.CollectibleField.Icon;
                    m_AnimatorIcon.enabled = false;
                }
                else
                {
                    m_AnimatorIcon.enabled = true;
                    m_AnimatorIcon.runtimeAnimatorController = icon.runtimeAnim;
                    m_AnimatorIcon.Play("Frame", 0, 0f);
                }
            }

            if (frame != null)
            {
                if (frame.runtimeAnim == null)
                {
                    m_ProfileBackground.sprite = frame.CollectibleField.Icon;
                    m_AnimatorFrame.enabled = false;
                }
                else
                {
                    m_AnimatorFrame.enabled = true;
                    m_AnimatorFrame.runtimeAnimatorController = frame.runtimeAnim;
                    m_AnimatorFrame.Play("Frame", 0, 0f);
                }
            }


            m_NoticeButton.NoticeCheck();
        }
    }
}
