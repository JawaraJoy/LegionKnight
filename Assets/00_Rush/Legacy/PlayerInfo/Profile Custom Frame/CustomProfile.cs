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
        public CustomImageDefinition SelectedIcon => m_SelectedIcon != null ? m_SelectedIcon.ProfileAppearance : null;
        public CustomImageDefinition UsedIcon => m_UsedIcon != null ? m_UsedIcon.ProfileAppearance : null;
        public CustomImageDefinition SelectedFrame => m_SelectedFrame != null ? m_SelectedFrame.ProfileAppearance : null;
        public CustomImageDefinition UsedFrame => m_UsedFrame != null ? m_UsedFrame.ProfileAppearance : null;

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
            foreach (var icon in m_Icons) icon.Init();
            foreach (var frame in m_Frames) frame.Init();

            var save = UnityService.Instance;
            m_UsedIcon = OwnedOrDefault(GetIconInternal(save.GetData<string>(USED_ICON_KEY)), m_DefaultIcon);
            m_UsedFrame = OwnedOrDefault(GetFrame(save.GetData<string>(USED_FRAME_KEY)), m_DefaultFrame);
            m_SelectedIcon = OwnedOrDefault(GetIconInternal(save.GetData<string>(SELECTED_ICON_KEY)), m_UsedIcon);
            m_SelectedFrame = OwnedOrDefault(GetFrame(save.GetData<string>(SELECTED_FRAME_KEY)), m_UsedFrame);
            RefreshInternal();
        }
        private static CustomImageDefinition OwnedOrDefault(ImageContent entry, CustomImageDefinition fallback) =>
            entry != null && entry.Owned ? entry.Definition : fallback;

        public void Refres()
        {
            RefreshInternal();
        }
        private void RefreshInternal()
        {
            m_ProfilePicts.RemoveAll(view => view == null);
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
        // Ownership and save IDs belong to the original item. Multiple owned items can share one visible tile.
        public ImageContent FindOwnedVariant(CustomImageDefinition definition)
        {
            if (definition == null) return null;
            var entries = definition.Type == CustomImageType.Icon ? m_Icons : m_Frames;
            ImageContent alternative = null;
            foreach (var entry in entries)
            {
                if (!entry.Owned || entry.Definition == null || entry.Definition.ProfileAppearance.BaseInfo.Id != definition.ProfileAppearance.BaseInfo.Id) continue;
                if (entry.Definition.BaseInfo.Id == definition.BaseInfo.Id) return entry;
                if (alternative == null) alternative = entry;
            }
            return alternative;
        }

        public bool HasNewAppearance(CustomImageDefinition definition)
        {
            if (definition == null) return false;
            var entries = definition.Type == CustomImageType.Icon ? m_Icons : m_Frames;
            foreach (var entry in entries)
            {
                if (entry.Owned && entry.Condition == ProductCondition.NewUnlocked &&
                    entry.Definition != null && entry.Definition.ProfileAppearance.BaseInfo.Id == definition.ProfileAppearance.BaseInfo.Id) return true;
            }
            return false;
        }

        public void AcknowledgeAppearance(CustomImageDefinition definition)
        {
            var entries = definition.Type == CustomImageType.Icon ? m_Icons : m_Frames;
            foreach (var entry in entries)
            {
                if (entry.Owned && entry.Condition == ProductCondition.NewUnlocked &&
                    entry.Definition != null && entry.Definition.ProfileAppearance.BaseInfo.Id == definition.ProfileAppearance.BaseInfo.Id)
                    entry.ChangeCondition(ProductCondition.NoticeUnlocked);
            }
        }

        public virtual void SetSelected(CustomImageDefinition definition)
        {
            var owned = FindOwnedVariant(definition);
            if (owned == null) return;
            bool icon = definition.Type == CustomImageType.Icon;
            var previous = icon ? m_SelectedIcon : m_SelectedFrame;
            // Keep the exact variant across reopening and equipping the same tile.
            if (previous != null && definition == definition.ProfileAppearance && previous.ProfileAppearance == definition.ProfileAppearance)
            {
                var previousEntry = icon ? GetIconInternal(previous) : GetFrameInternal(previous);
                if (previousEntry != null && previousEntry.Owned) owned = previousEntry;
            }
            if (icon) m_SelectedIcon = owned.Definition;
            else m_SelectedFrame = owned.Definition;
            UnityService.Instance.SaveData(icon ? SELECTED_ICON_KEY : SELECTED_FRAME_KEY, owned.Definition.BaseInfo.Id);
            RefreshInternal();
        }
        public void SetUsed(CustomImageType typ)
        {
            switch(typ)
            {
                case CustomImageType.Frame:
                    if (m_SelectedFrame == null || GetFrameInternal(m_SelectedFrame)?.Owned != true) return;
                    m_UsedFrame = m_SelectedFrame;
                    UnityService.Instance.SaveData(USED_FRAME_KEY, m_UsedFrame.BaseInfo.Id);
                    break;
                case CustomImageType.Icon:
                    if (m_SelectedIcon == null || GetIconInternal(m_SelectedIcon)?.Owned != true) return;
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
            // Both Valkyrie variants shared the static ID in older saves.
            var save = UnityService.Instance;
            const string migration = "Profile.ValkyrieSplit.v1";
            if (m_Definition.BaseInfo.Id == "frvalky_animated" && !save.HasData(migration))
            {
                save.LocalSave.ExecuteTransaction(() =>
                {
                    if (!save.HasData(OWNED_KEY) && save.HasData("ownedfrvalky"))
                    {
                        save.SaveData(OWNED_KEY, save.GetData<bool>("ownedfrvalky"));
                        if (save.HasData("confrvalky")) save.SaveData(CONDITION_KEY, save.GetData<ProductCondition>("confrvalky"));
                    }
                    save.SaveData(migration, true);
                });
            }
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
                CanvasManager.Instance.GetPanel<NewUnlockedPopUpPanel>().ShowPopUp(m_Definition.ProfileAppearance);
                ChangeConditionInternal(ProductCondition.NewUnlocked);
            }
            m_Owned = owned;
            UnityService.Instance.SaveData(OWNED_KEY, m_Owned);
            Player.Instance.CustomProfile.Refres();
            
        }
    }
}
