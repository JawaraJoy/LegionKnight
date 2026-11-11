using MoreMountains.Tools;
using System.Collections.Generic;
using UnityEngine;
using Rush;
using UnityEngine.Events;

namespace LegionKnight
{
    public class CustomProfile : MonoBehaviour
    {
        
        [SerializeField]
        private ImageContent[] m_Icons;
        [SerializeField]
        private ImageContent[] m_Frames;
        [SerializeField]
        private CustomImageDefinition m_DefaultIcon;
        [SerializeField]
        private CustomImageDefinition m_DefaultFrame;
        [SerializeField, MMReadOnly]
        private CustomImageDefinition m_SelectedIcon;
        [SerializeField, MMReadOnly]
        private CustomImageDefinition m_SelectedFrame;
        [SerializeField, MMReadOnly]
        private CustomImageDefinition m_UsedIcon;
        [SerializeField, MMReadOnly]
        private CustomImageDefinition m_UsedFrame;
        [SerializeField, MMReadOnly]
        private List<ProfilePictView> m_ProfilePicts = new();
        public ImageContent[] Icons => m_Icons;
        public ImageContent[] Frames => m_Frames;
        public CustomImageDefinition SelectedIcon => m_SelectedIcon;
        public CustomImageDefinition UsedIcon => m_UsedIcon;
        public CustomImageDefinition SelectedFrame => m_SelectedFrame;
        public CustomImageDefinition UsedFrame => m_UsedFrame;

        private string SELECTED_ICON_KEY => $"selicon";
        private string USED_ICON_KEY => $"usedicon";
        private string SELECTED_FRAME_KEY => $"selframe";
        private string USED_FRAME_KEY => $"usedframe";

        private CustomProfilePanel m_ProfilePanel;

        private CustomProfilePanel GetProfilePanel()
        {
            if (m_ProfilePanel == null)
            {
                m_ProfilePanel = CanvasManager.Instance.GetPanel<CustomProfilePanel>();
            }
            return m_ProfilePanel;
        }

        public void AddProfilePictView(ProfilePictView pict)
        {
            if (m_ProfilePicts.Contains(pict))return;
            m_ProfilePicts.Add(pict);
        }
        public void RemoveProfilePictView(ProfilePictView pict)
        {
            if (!m_ProfilePicts.Contains(pict)) return;
            m_ProfilePicts.Remove(pict);
        }
        private ImageContent GetIconInternal(CustomImageDefinition defi)
        {
            foreach (var icon in m_Icons)
            {
                if (icon.Definition.BaseInfo.Id == defi.BaseInfo.Id)
                {
                    return icon;
                }
            }
            return null;
        }
        public ImageContent GetIcon(CustomImageDefinition defi)
        {
            return GetIconInternal(defi);
        }
        public ImageContent GetFrame(CustomImageDefinition defi)
        {
            return GetFrameInternal(defi);
        }
        private ImageContent GetIconInternal(string id)
        {
            foreach (var icon in m_Icons)
            {
                if (icon.Definition.BaseInfo.Id == id)
                {
                    return icon;
                }
            }
            return null;
        }
        private ImageContent GetFrameInternal(CustomImageDefinition defi)
        {
            foreach (var frame in m_Frames)
            {
                if (frame.Definition.BaseInfo.Id == defi.BaseInfo.Id)
                {
                    return frame;
                }
            }
            return null;
        }
        private ImageContent GetFrame(string id)
        {
            foreach (var frame in m_Frames)
            {
                if (frame.Definition.BaseInfo.Id == id)
                {
                    return frame;
                }
            }
            return null;
        }
        private bool HasIconInternal(CustomImageDefinition defi, out ImageContent content)
        {
            content = GetIconInternal(defi);
            return content != null;
        }
        private bool HasIconInternal(string id, out ImageContent content)
        {
            content = GetIconInternal(id);
            return content != null;
        }
        private bool HasFrameInternal(CustomImageDefinition defi, out ImageContent content)
        {
            content = GetFrameInternal(defi);
            return content != null;
        }
        private bool HasFrameInternal(string id, out ImageContent content)
        {
            content = GetFrame(id);
            return content != null;
        }
        public bool HasIcon(CustomImageDefinition defi, out ImageContent content)
        {
            return HasIconInternal(defi, out content);
        }
        public bool HasFrame(CustomImageDefinition defi, out ImageContent content)
        {
            return HasFrameInternal(defi, out content);
        }
        public void Init()
        {
            bool hasSelectedIcon = UnityService.Instance.HasData(SELECTED_ICON_KEY);
            bool hasSelectedFrame = UnityService.Instance.HasData(SELECTED_FRAME_KEY);
            if (hasSelectedIcon)
            {
                string selectedIconId = UnityService.Instance.GetData<string>(SELECTED_ICON_KEY);
                if (HasIconInternal(selectedIconId, out ImageContent content))
                {
                    m_SelectedIcon = content.Definition;
                }
                else
                {
                    m_SelectedIcon = m_DefaultIcon;
                }
                
            }
            else
            {
                m_SelectedIcon = m_DefaultIcon;
            }
            Debug.Log($"[profile] hasselectedicon{hasSelectedIcon}");
            if (hasSelectedFrame)
            {
                string selectedFrameId = UnityService.Instance.GetData<string>(SELECTED_FRAME_KEY);
                if (HasFrameInternal(selectedFrameId, out ImageContent content))
                {
                    m_SelectedFrame = content.Definition;
                }
                else
                {
                    m_SelectedFrame = m_DefaultFrame;
                }
            }
            else
            {
                m_SelectedFrame = m_DefaultFrame;
            }

            bool hasUsedIcon = UnityService.Instance.HasData(USED_ICON_KEY);
            bool hasUsedFrame = UnityService.Instance.HasData(USED_FRAME_KEY);
            if (hasUsedIcon)
            {
                string usedIconId = UnityService.Instance.GetData<string>(USED_ICON_KEY);
                if (HasIconInternal(usedIconId, out ImageContent content))
                {
                    m_UsedIcon = content.Definition;
                }
                else
                {
                    m_UsedIcon = m_DefaultIcon;
                }
            }
            else
            {
                m_UsedIcon = m_DefaultIcon;
            }
            if (hasUsedFrame)
            {
                string usedFrameId = UnityService.Instance.GetData<string>(USED_FRAME_KEY);
                if (HasFrameInternal(usedFrameId, out ImageContent content))
                {
                    m_UsedFrame = content.Definition;
                }
                else
                {
                    m_UsedFrame = m_DefaultFrame;
                }
            }
            else
            {
                m_UsedFrame = m_DefaultFrame;
            }

            foreach (var icon in m_Icons)
            {
                icon.Init();
            }

            foreach (var frame in m_Frames)
            {
                frame.Init();
            }
            RefreshInternal();
        }
        public void Refres()
        {
            RefreshInternal();
        }
        private void RefreshInternal()
        {
            foreach (var pp in m_ProfilePicts)
            {
                pp.Init();
            }
            GetProfilePanel().RefreshAllViews();
        }
        public void SetOwned(CustomImageDefinition defi, bool owned)
        {
            switch(defi.Type)
            {
                case CustomImageType.Frame:
                    if (HasFrameInternal(defi, out var frame))
                    {
                        frame.SetOwned(owned);
                    }
                    break;
                case CustomImageType.Icon:
                    if (HasIconInternal(defi, out var icon))
                    {
                        icon.SetOwned(owned);
                    }
                    break;
            }

        }
        public virtual void SetSelected(CustomImageDefinition defi)
        {
            
            switch(defi.Type)
            {
                case CustomImageType.Frame:
                    if (HasFrameInternal(defi, out var frame))
                    {
                        m_SelectedFrame = frame.Definition;
                        UnityService.Instance.SaveData(SELECTED_ICON_KEY, m_SelectedIcon.BaseInfo.Id);
                    }
                    break;
                case CustomImageType.Icon:
                    if (HasIconInternal(defi, out var icon))
                    {
                        m_SelectedIcon = icon.Definition;
                        UnityService.Instance.SaveData(SELECTED_FRAME_KEY, m_SelectedFrame.BaseInfo.Id);
                    }
                    break;
            }
            RefreshInternal();  
        }
        public void SetUsed(CustomImageType typ)
        {
            switch(typ)
            {
                case CustomImageType.Frame:
                    m_UsedFrame = m_SelectedFrame;
                    UnityService.Instance.SaveData(USED_FRAME_KEY, m_UsedFrame.BaseInfo.Id);
                    break;
                case CustomImageType.Icon:
                    m_UsedIcon = m_SelectedIcon;
                    UnityService.Instance.SaveData(USED_ICON_KEY, m_UsedIcon.BaseInfo.Id);
                    break;
            }
            
            RefreshInternal();
        }
    }

    [System.Serializable]
    public class ImageContent : IProductHasCondition
    {
        [SerializeField]
        private CustomImageDefinition m_Definition;
        [SerializeField]
        private bool m_Owned;
        [SerializeField, MMReadOnly]
        private ProductCondition m_Condition = ProductCondition.Locked;
        private UnityEvent<ProductCondition> m_OnConditionChanged = new();
        public CustomImageDefinition Definition => m_Definition;
        public bool Owned => m_Owned;

        public ProductCondition Condition => m_Condition;

        public UnityEvent<ProductCondition> OnConditionChanged => m_OnConditionChanged;

        private string OWNED_KEY => $"owned{m_Definition.BaseInfo.Id}";
        private string CONDITION_KEY => $"con{m_Definition.BaseInfo.Id}";

        public void ChangeCondition(ProductCondition condition)
        {
            ChangeConditionInternal(condition);
        }
        private void ChangeConditionInternal(ProductCondition condition)
        {
            m_Condition = condition;
            UnityService.Instance.SaveData(CONDITION_KEY, m_Condition);
            m_OnConditionChanged?.Invoke(condition);
        }

        public void Init()
        {
            bool hasOwnedData = UnityService.Instance.HasData(OWNED_KEY);
            bool hasConditionData = UnityService.Instance.HasData(CONDITION_KEY);
            if (hasOwnedData)
            {
                m_Owned = UnityService.Instance.GetData<bool>(OWNED_KEY);
            }
            if (hasConditionData)
            {
                m_Condition = UnityService.Instance.GetData<ProductCondition>(CONDITION_KEY);
            }
        }
        public void SetOwned(bool owned)
        {
            if (owned == true && m_Owned == false)
            {
                CanvasManager.Instance.GetPanel<NewUnlockedPopUpPanel>().ShowPopUp(m_Definition);
                ChangeConditionInternal(ProductCondition.NewUnlocked);
                Player.Instance.CustomProfile.Refres();
            }
            m_Owned = owned;
            UnityService.Instance.SaveData(OWNED_KEY, m_Owned);
            
        }
    }
}
