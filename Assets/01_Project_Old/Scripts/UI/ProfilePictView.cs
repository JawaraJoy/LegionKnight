using MoreMountains.Tools;
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
        private Animator m_Animator;
        [SerializeField, MMReadOnly]
        private AnimationClip m_NewAnimatedFrameClip;
        [SerializeField, MMReadOnly]
        private AnimatorOverrideController m_OverrideController;
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
            var baseController = m_Animator.runtimeAnimatorController;

            if (baseController is AnimatorOverrideController aoc)
            {
                baseController = aoc.runtimeAnimatorController;
            }

            m_OverrideController = new AnimatorOverrideController(baseController);
            m_Animator.runtimeAnimatorController = m_OverrideController;
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
                m_ProfileBackground.sprite = icon.Icon;
            }

            if (frame != null)
            {
                if (frame.IconAnimationClip == null)
                {
                    m_ProfileBackground.sprite = frame.Icon;
                    m_Animator.enabled = false;
                }
                else
                {
                    m_Animator.enabled = true;
                    m_NewAnimatedFrameClip = frame.IconAnimationClip;

                    // "Frame" MUST be the original clip name in Animator
                    m_OverrideController["Kill Joy Frame Clip"] = m_NewAnimatedFrameClip;

                    m_Animator.Play("Frame", 0, 0f);
                }
            }
            if (icon != null)
            {
                
            }


            m_NoticeButton.NoticeCheck();
        }
    }
}
