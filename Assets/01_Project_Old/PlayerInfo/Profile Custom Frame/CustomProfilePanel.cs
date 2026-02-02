using MoreMountains.Tools;
using Rush;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;

namespace LegionKnight
{
    public class CustomProfilePanel : PanelView
    {
        [SerializeField]
        private AssetReferenceGameObject m_ImageViewAsset;
        [SerializeField]
        private Transform m_ImageViewContainer;
        [SerializeField]
        private Button m_IconTabButton;
        [SerializeField]
        private Button m_FrameTabButton;
        [SerializeField]
        private Button m_UseButton;

        private readonly List<ImageView> m_SpawnedImageViews = new();

        [SerializeField, MMReadOnly]
        private CustomImageType m_CurrentSelectedType = CustomImageType.Icon;

        private PlayerCustomProfile m_CustomProfile;
        private PlayerCustomProfile GetCustomProfile()
        {
            if (m_CustomProfile == null)
            {
                m_CustomProfile = Player.Instance.CustomProfile;
            }
            return m_CustomProfile;
        }

        private void Start()
        {
            
            m_IconTabButton.onClick.RemoveAllListeners();
            m_IconTabButton.onClick.AddListener(ShowIcon);

            m_FrameTabButton.onClick.RemoveAllListeners();
            m_FrameTabButton.onClick.AddListener(ShowFrame);

            m_UseButton.onClick.RemoveAllListeners();
            m_UseButton.onClick.AddListener(() => SetUsed());
        }
        private void SetCurrentSelectedTypeInternal(CustomImageType type)
        {
            m_CurrentSelectedType = type;
        }
        public void SetCurrentSelectedType(CustomImageType type)
        {
            SetCurrentSelectedTypeInternal(type);
        }

        protected override void ShowInternal()
        {
            base.ShowInternal();
            RefreshInternal();
        }
        public void Refresh()
        {
            RefreshInternal();
        }
        private void RefreshInternal()
        {
            StartCoroutine(RefreshingCustomImageViews());
        }
        private void ShowIconInternal()
        {
            CloseAllView();
            GetTypeImageViews(CustomImageType.Icon).ForEach(x => x.Show());

            m_FrameTabButton.image.color = Color.gray;
            m_IconTabButton.image.color = Color.white;
            m_IconTabButton.interactable = false;
            m_FrameTabButton.interactable = true;

            SetCurrentSelectedTypeInternal(CustomImageType.Icon);
        }
        public void ShowIcon()
        {
            ShowIconInternal();
        }
        public void ShowFrame()
        {
            CloseAllView();
            GetTypeImageViews(CustomImageType.Frame).ForEach(x => x.Show());

            m_FrameTabButton.image.color = Color.white;
            m_IconTabButton.image.color = Color.gray;
            m_IconTabButton.interactable = true;
            m_FrameTabButton.interactable = false;

            SetCurrentSelectedTypeInternal(CustomImageType.Frame);
        }

        private ImageView GetImageView(CustomImageDefinition defi)
        {
            return m_SpawnedImageViews.Find(x => x.Definition == defi);
        }

        private bool HasImageView(CustomImageDefinition defi, out ImageView view)
        {
            view = GetImageView(defi);
            return GetImageView(defi) != null;
        }

        private List<ImageView> GetTypeImageViews(CustomImageType typ)
        {
            return m_SpawnedImageViews.FindAll(x => x.Definition.Type == typ);
        }

        private void CloseAllView()
        {
            m_SpawnedImageViews.ForEach(x => x.Hide());
        }

        private void SetUsed()
        {
            GetCustomProfile().SetUsed(m_CurrentSelectedType);
        }
        private IEnumerator RefreshingCustomImageViews()
        {
            ImageContent[] icons = GetCustomProfile().Icons;
            ImageContent[] frames = GetCustomProfile().Frames;

            for (int i = 0; i < icons.Length; i++)
            {
                CustomImageDefinition defi = icons[i].Definition;
                if (HasImageView(defi, out var view))
                {
                    view.Init(defi);
                }
                else
                {
                    yield return StartCoroutine(SpawningCustomImageView(defi));
                }
            }
            yield return new WaitForEndOfFrame();
            for (int i = 0; i < frames.Length; i++)
            {
                CustomImageDefinition defi = frames[i].Definition;
                if (HasImageView(defi, out var view))
                {
                    view.Init(defi);
                }
                else
                {
                    yield return StartCoroutine(SpawningCustomImageView(defi));
                }
            }
            
            yield return new WaitForEndOfFrame();
            ShowIconInternal();
        }
        private IEnumerator SpawningCustomImageView(CustomImageDefinition defi)
        {
            var handle = m_ImageViewAsset.InstantiateAsync(m_ImageViewContainer, false);
            yield return handle;
            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                GameObject result = handle.Result;
                if (result.TryGetComponent(out ImageView view))
                {
                    view.Init(defi);
                    m_SpawnedImageViews.Add(view);
                    /*ImageViewNoticeButton noticeButton = view.NoticeButton;
                    if (noticeButton != null)
                    {
                        noticeButton.SetDefinition(defi);
                        noticeButton.NoticeCheck();
                    }*/
                }
            }
            
        }
        private void UnSelectectAllViews()
        {
            foreach(var view in m_SpawnedImageViews)
            {
                view.UnSelected();
            }
        }
        private void RefreshAllViewsInternal()
        {
            foreach(var view in m_SpawnedImageViews)
            {
                view.Refresh();
            }
        }
        public void RefreshAllViews()
        {
            RefreshAllViewsInternal();
        }
    }
}
