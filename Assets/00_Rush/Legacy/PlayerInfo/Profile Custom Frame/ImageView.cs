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
        [SerializeField] private Image m_RarityFrame;

        private bool m_IsSelected = false;
        [SerializeField]
        private Button m_SelectButton;
        [SerializeField]
        private Animator m_Animator;

        private CustomImageDefinition m_Definition;

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
            m_Definition = defi.ProfileAppearance;
            m_NoticeButton.SetDefinition(m_Definition);
            RefreshInternal();
        }
        
        public void Refresh()
        {
            RefreshInternal();
        }
        private void RefreshInternal()
        {
            var rarity = m_Definition.CollectibleField.RarityConfig;
            if (m_RarityFrame != null && rarity != null) m_RarityFrame.color = rarity.Color;
            m_Icon.sprite = m_Definition.CollectibleField.Icon;
            bool hasAnim = m_Definition.runtimeAnim != null;
            m_Animator.enabled = hasAnim;

            if (hasAnim)
            {
                m_runtimeAnim = m_Definition.runtimeAnim;

                m_Animator.runtimeAnimatorController = m_runtimeAnim;

                m_Animator.Play("Frame", 0, 0f);
            }
            bool owned = IsOwned();
            m_Icon.color = owned ? Color.white : Color.gray;
            m_SelectButton.interactable = owned;
            m_NoticeButton.NoticeCheck();
            m_SelectedImage.gameObject.SetActive(IsSelectedInternal());
        }
        private void SelectectInternal()
        {
            if (!IsOwned()) return;
            GetCustomProfile().AcknowledgeAppearance(m_Definition);
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

        private bool IsOwned() => GetCustomProfile().FindOwnedVariant(m_Definition) != null;

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
