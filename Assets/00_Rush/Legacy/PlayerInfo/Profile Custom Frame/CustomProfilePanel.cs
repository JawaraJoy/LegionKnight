using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;

namespace LegionKnight
{
    public class CustomProfilePanel : PanelView
    {
        [SerializeField] private AssetReferenceGameObject m_ImageViewAsset;
        [SerializeField] private Transform m_ImageViewContainer;
        [SerializeField] private Button m_IconTabButton;
        [SerializeField] private Button m_FrameTabButton;
        [SerializeField] private Button m_UseButton;
        [SerializeField] private CustomImageType m_CurrentSelectedType = CustomImageType.Icon;
        private readonly Dictionary<CustomImageDefinition, ImageView> m_Views = new();
        private bool m_Loading;
        private PlayerCustomProfile Profile => Player.Instance.CustomProfile;

        private void Awake()
        {
            m_IconTabButton.onClick.AddListener(ShowIcon);
            m_FrameTabButton.onClick.AddListener(ShowFrame);
            m_UseButton.onClick.AddListener(EquipSelection);
        }

        private void EquipSelection()
        {
            Profile.SetUsed(m_CurrentSelectedType);
            HideInternal();
        }

        protected override void ShowInternal()
        {
            base.ShowInternal();
            Refresh();
        }

        public async void Refresh()
        {
            ApplyTab();
            if (m_Loading) return;
            m_Loading = true;
            try
            {
                await LoadEntries(Profile.Icons);
                if (this) await LoadEntries(Profile.Frames);
            }
            catch (System.Exception ex) { Debug.LogWarning($"Profile collection could not load: {ex.Message}"); }
            finally { m_Loading = false; }
            if (this) ApplyTab();
        }

        private async Task LoadEntries(ImageContent[] entries)
        {
            foreach (var entry in entries)
            {
                var definition = entry.Definition?.ProfileAppearance;
                if (definition == null || m_Views.ContainsKey(definition)) continue;
                var handle = m_ImageViewAsset.InstantiateAsync(m_ImageViewContainer, false);
                var instance = await handle.Task;
                if (!this || !instance) { if (handle.IsValid()) Addressables.ReleaseInstance(handle); return; }
                if (!instance.TryGetComponent<ImageView>(out var view)) { Addressables.ReleaseInstance(instance); continue; }
                view.Init(definition);
                m_Views.Add(definition, view);
                instance.SetActive(definition.Type == m_CurrentSelectedType);
                if (instance.activeSelf) view.Show();
            }
        }

        public void SetCurrentSelectedType(CustomImageType type) { m_CurrentSelectedType = type; ApplyTab(); }
        public void ShowIcon() => SetCurrentSelectedType(CustomImageType.Icon);
        public void ShowFrame() => SetCurrentSelectedType(CustomImageType.Frame);

        private void ApplyTab()
        {
            bool icons = m_CurrentSelectedType == CustomImageType.Icon;
            m_IconTabButton.interactable = !icons;
            m_FrameTabButton.interactable = icons;
            foreach (var pair in m_Views)
            {
                bool visible = pair.Key.Type == m_CurrentSelectedType;
                pair.Value.gameObject.SetActive(visible);
                if (visible) pair.Value.Show();
            }
            RefreshAllViews();
        }

        public void RefreshAllViews()
        {
            foreach (var view in m_Views.Values) view.Refresh();
            var profile = Profile;
            var selected = m_CurrentSelectedType == CustomImageType.Icon ? profile.SelectedIcon : profile.SelectedFrame;
            m_UseButton.interactable = profile.FindOwnedVariant(selected) != null;
        }

        private void OnDestroy()
        {
            foreach (var view in m_Views.Values) if (view) Addressables.ReleaseInstance(view.gameObject);
        }
    }
}
