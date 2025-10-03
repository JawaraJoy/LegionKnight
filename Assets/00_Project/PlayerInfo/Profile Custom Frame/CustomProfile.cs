using MoreMountains.Tools;
using System.Collections.Generic;
using Unity.Services.Core;
using UnityEngine;
using UnityEngine.InputSystem;

namespace LegionKnight
{
    public class CustomProfile : MonoBehaviour
    {
        
        [SerializeField]
        private ImageContent[] m_Icons;
        [SerializeField]
        private ImageContent[] m_Frames;
        [SerializeField, MMReadOnly]
        private List<ProfilePictView> m_ProfilePicts = new();
        [SerializeField, MMReadOnly]
        private CustomImageDefinition m_SelectedIcon;
        [SerializeField, MMReadOnly]
        private CustomImageDefinition m_SelectedFrame;
        [SerializeField, MMReadOnly]
        private CustomImageDefinition m_UsedIcon;
        [SerializeField, MMReadOnly]
        private CustomImageDefinition m_UsedFrame;
        public ImageContent[] Icons => m_Icons;
        public ImageContent[] Frames => m_Frames;
        public CustomImageDefinition SelectedIcon => m_SelectedIcon;
        public CustomImageDefinition UsedIcon => m_UsedIcon;
        public CustomImageDefinition SelectedFrame => m_SelectedFrame;
        public CustomImageDefinition UsedFrame => m_UsedFrame;

        public void AddProfilePictView(ProfilePictView pict)
        {
            if (m_ProfilePicts.Contains(pict))return;
            m_ProfilePicts.Add(pict);
        }
        private ImageContent GetIcon(CustomImageDefinition defi)
        {
            foreach (var icon in m_Icons)
            {
                if (icon.Definition == defi)
                {
                    return icon;
                }
            }
            return null;
        }
        private ImageContent GetFrame(CustomImageDefinition defi)
        {
            foreach (var frame in m_Frames)
            {
                if (frame.Definition == defi)
                {
                    return frame;
                }
            }
            return null;
        }
        private bool HasIconInternal(CustomImageDefinition defi, out ImageContent content)
        {
            content = GetIcon(defi);
            return content != null;
        }
        private bool HasFrameInternal(CustomImageDefinition defi, out ImageContent content)
        {
            content = GetFrame(defi);
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
            foreach (var icon in m_Icons)
            {
                icon.Init();
                if (icon.Selected)
                {
                    m_SelectedIcon = icon.Definition;
                }
                if (icon.Used)
                {
                    m_UsedIcon = icon.Definition;
                }
            }

            foreach (var frame in m_Frames)
            {
                frame.Init();
                if (frame.Selected)
                {
                    m_SelectedFrame = frame.Definition;
                }
                if (frame.Used)
                {
                    m_UsedFrame = frame.Definition;
                }
            }
            Refresh();
        }
        private void Refresh()
        {
            foreach (var pp in m_ProfilePicts)
            {
                pp.Init();
            }
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
        public virtual void SetSelected(CustomImageDefinition defi, bool selected)
        {
            switch(defi.Type)
            {
                case CustomImageType.Frame:
                    UnSelectAllFrame();
                    if (HasFrameInternal(defi, out var frame))
                    {
                        frame.SetSelected(selected);
                        m_SelectedFrame = frame.Definition;
                    }
                    break;
                case CustomImageType.Icon:
                    UnSelectAllIcon();
                    if (HasIconInternal(defi, out var icon))
                    {
                        icon.SetSelected(selected);
                        m_SelectedIcon = icon.Definition;
                    }
                    break;
            }

        }
        public void SetUsed(CustomImageDefinition defi, bool used)
        {
            switch(defi.Type)
            {
                case CustomImageType.Frame:
                    UnUsedAllFrame();
                    if (HasFrameInternal(defi, out var frame))
                    {
                        frame.SetUsed(used);
                        m_UsedFrame = frame.Definition;
                    }
                    break;
                case CustomImageType.Icon:
                    UnUsedAllIcon();
                    if (HasIconInternal(defi, out var icon))
                    {
                        icon.SetUsed(used);
                        m_UsedIcon = icon.Definition;
                    }
                    break;
            }
            Refresh();
        }
        public void SetUsed(CustomImageType typ, bool used)
        {
            switch(typ)
            {
                case CustomImageType.Frame:
                    UnUsedAllFrame();
                    m_SelectedFrame.SetUsed(used);
                    m_UsedFrame = m_SelectedFrame;
                    break;
                case CustomImageType.Icon:
                    UnUsedAllIcon();
                    m_SelectedIcon.SetUsed(used);
                    m_UsedIcon = m_SelectedIcon;
                    break;
            }
            Refresh();
        }

        private void UnSelectAllIcon()
        {
            foreach (var icon in m_Icons)
            {
                icon.SetSelected(false);
            }
        }
        private void UnSelectAllFrame()
        {
            foreach (var frame in m_Frames)
            {
                frame.SetSelected(false);
            }
        }
        private void UnUsedAllIcon()
        {
            foreach (var icon in m_Icons)
            {
                icon.SetUsed(false);
            }
        }
        private void UnUsedAllFrame()
        {
            foreach (var frame in m_Frames)
            {
                frame.SetUsed(false);
            }
        }
    }

    [System.Serializable]
    public class ImageContent
    {
        [SerializeField]
        private CustomImageDefinition m_Definition;
        [SerializeField]
        private bool m_Owned;
        [SerializeField]
        private bool m_Seleted;
        [SerializeField]
        private bool m_Used;
        public CustomImageDefinition Definition => m_Definition;
        public bool Owned => m_Owned;
        public bool Selected => m_Seleted;
        public bool Used => m_Used;

        private string OWNED_KEY => $"owned{m_Definition.Id}";
        private string SELECTED_KEY => $"selected{m_Definition.Id}";
        private string USED_KEY => $"used{m_Definition.Id}";
        public void Init()
        {
            bool hasOwnedData = UnityService.Instance.HasData(m_Definition.Id);
            bool hasSelectedData = UnityService.Instance.HasData(SELECTED_KEY);
            bool hasUsedData = UnityService.Instance.HasData(USED_KEY);
            if (hasOwnedData)
            {
                m_Owned = UnityService.Instance.GetData<bool>(m_Definition.Id);
            }
            if (hasSelectedData)
            {
                m_Seleted = UnityService.Instance.GetData<bool>(SELECTED_KEY);
            }
            if (hasUsedData)
            {
                m_Used = UnityService.Instance.GetData<bool>(USED_KEY);
            }
        }
        public void SetOwned(bool owned)
        {
            m_Owned = owned;
            UnityService.Instance.SaveData(OWNED_KEY, m_Owned);
        }
        public void SetSelected(bool selected)
        {
            m_Seleted = selected;
            UnityService.Instance.SaveData(SELECTED_KEY, m_Seleted);
        }
        public void SetUsed(bool used)
        {
            m_Used = used;
            UnityService.Instance.SaveData(USED_KEY, m_Used);
        }
    }
}
