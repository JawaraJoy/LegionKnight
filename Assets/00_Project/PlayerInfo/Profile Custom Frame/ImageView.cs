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

        private CustomImageDefinition m_Definition;
        private ImageContent m_ImageContent;

        private PlayerCustomProfile m_Profile;
        public bool IsSelected => m_IsSelected;
        public CustomImageDefinition Definition => m_Definition;

        [SerializeField]
        private UnityEvent m_OnSelected;

        public void AddListenToOnSelected(UnityAction action)
        {
            m_OnSelected.AddListener(action);
        }
        public void ClearOnSelected()
        {
            m_OnSelected.RemoveAllListeners();
        }
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
            m_SelectButton.onClick.AddListener(() => SetSelectectedInternal(true));
        }
        public void Init(CustomImageDefinition defi)
        {
            m_Definition = defi;
            m_Icon.sprite = defi.Icon;
            RefreshInternal();
        }
        public void Refresh()
        {
            RefreshInternal();
        }
        private void RefreshInternal()
        {
            m_Icon.color = IsOwned() ? Color.white : Color.gray;
            m_SelectButton.interactable = IsOwned();
            m_SelectedImage.gameObject.SetActive(IsSelectedInternal());
        }
        private void SetSelectectedInternal(bool selected)
        {
            m_OnSelected.Invoke();
            GetCustomProfile().SetSelected(m_Definition, selected);
            m_SelectedImage.gameObject.SetActive(IsSelectedInternal());
        }
        public void SetSelected(bool selected)
        {
            SetSelectectedInternal(selected);
        }

        public void UnSelected()
        {
            GetCustomProfile().SetSelected(m_Definition, false);
            m_SelectedImage.gameObject.SetActive(IsSelectedInternal());
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
                    m_IsSelected = GetCustomProfile().HasFrame(m_Definition, out m_ImageContent) && m_ImageContent.Selected;
                    break;
                case CustomImageType.Icon:
                    m_IsSelected = GetCustomProfile().HasIcon(m_Definition, out m_ImageContent) && m_ImageContent.Selected;
                    break;
            }
            return m_IsSelected;
        }
    }
}
