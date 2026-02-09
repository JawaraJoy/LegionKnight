using MoreMountains.Tools;
using Rush;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace LegionKnight
{
    public class ImageView : UIView
    {
        [SerializeField]
        private Image m_Icon;
        [SerializeField]
        private Image m_SelectedImage;

        private bool m_IsSelected = false;
        [SerializeField]
        private Button m_SelectButton;
        [SerializeField]
        private Animator m_Animator;

        private CustomImageDefinition m_Definition;
        private ImageContent m_ImageContent;

        private PlayerCustomProfile m_Profile;
        public bool IsSelected => m_IsSelected;
        public CustomImageDefinition Definition => m_Definition;
        public Button SelectButton => m_SelectButton;

        [SerializeField]
        private ImageViewNoticeButton m_NoticeButton;
        public ImageViewNoticeButton NoticeButton => m_NoticeButton;
        [SerializeField]
        private UnityEvent m_OnSelected;
        [SerializeField, MMReadOnly]
        private RuntimeAnimatorController m_runtimeAnim;

        private PlayerCustomProfile GetCustomProfile()
        {
            if (m_Profile == null)
            {
                m_Profile = Player.Instance.CustomProfile;
            }
            return m_Profile;
        }
        private void Start()
        {
            m_SelectButton.onClick.RemoveAllListeners();
            m_SelectButton.onClick.AddListener(() => SelectectInternal());
        }
        public void Init(CustomImageDefinition defi)
        {
            m_Definition = defi;
            RefreshInternal();
            m_NoticeButton.SetDefinition(m_Definition);
            m_NoticeButton.NoticeCheck();
        }
        
        public void Refresh()
        {
            RefreshInternal();
        }
        private void RefreshInternal()
        {
            m_Icon.sprite = m_Definition.Icon;
            bool hasAnim = m_Definition.runtimeAnim != null;
            m_Animator.enabled = hasAnim;

            if (hasAnim)
            {
                m_runtimeAnim = m_Definition.runtimeAnim;

                m_Animator.runtimeAnimatorController = m_runtimeAnim;

                m_Animator.Play("Frame", 0, 0f);
            }
            m_Icon.color = IsOwned() ? Color.white : Color.gray;
            m_SelectButton.interactable = IsOwned();
            m_SelectedImage.gameObject.SetActive(IsSelectedInternal());
        }
        private void SelectectInternal()
        {
            ImageContent imageContent = Player.Instance.CustomProfile.GetIcon(m_Definition);

            switch (m_Definition.Type)
            {
                case CustomImageType.Frame:
                    imageContent = Player.Instance.CustomProfile.GetFrame(m_Definition);
                    break;
                case CustomImageType.Icon:
                    imageContent = Player.Instance.CustomProfile.GetIcon(m_Definition);
                    break;
            }
            imageContent.ChangeCondition(ProductCondition.NoticeUnlocked);
            m_NoticeButton.NoticeCheck();
            m_OnSelected.Invoke();
            GetCustomProfile().SetSelected(m_Definition);
            m_SelectedImage.gameObject.SetActive(true);
            
        }
        public void Select()
        {
            SelectectInternal();
        }

        public void UnSelected()
        {
            m_SelectedImage.gameObject.SetActive(false);
        }

        private bool IsOwned()
        {
            if (m_Definition == null)
            {
                return false;
            }
            bool owned = false;
            switch (m_Definition.Type)
            {
                case CustomImageType.Frame:
                    owned = GetCustomProfile().HasFrame(m_Definition, out m_ImageContent) && m_ImageContent.Owned;
                    break;
                case CustomImageType.Icon:
                    owned = GetCustomProfile().HasIcon(m_Definition, out m_ImageContent) && m_ImageContent.Owned;
                    break;
            }
            return owned;
        }

        private bool IsSelectedInternal()
        {
            switch (m_Definition.Type)
            {
                case CustomImageType.Frame:
                    m_IsSelected = GetCustomProfile().SelectedFrame == m_Definition;
                    break;
                case CustomImageType.Icon:
                    m_IsSelected = GetCustomProfile().SelectedIcon == m_Definition;
                    break;
            }
            return m_IsSelected;
        }
    }
}
