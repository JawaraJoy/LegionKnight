using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace LegionKnight
{
    public class AchievementBadgeView : UIView
    {
        private readonly List<BadgeView> m_BadgeViews = new();
        [SerializeField]
        private Transform m_BadgeContainer;
        [SerializeField]
        private AssetReferenceGameObject m_BadgeAsset;

        private BadgeManager m_BadgeManager;

        private BadgeManager GetBadgeManager()
        {
            if (m_BadgeManager == null)
            {
                m_BadgeManager = Player.Instance.BadgeManager;
            }
            return m_BadgeManager;
        }

        private BadgeView GetBadgeView(BadgeDefinition defi)
        {
            return m_BadgeViews.Find(b => b.Definition == defi);
        }
        private bool HasBadgeView(BadgeDefinition defi, out BadgeView view)
        {
            view = GetBadgeView(defi);
            return view != null;
        }
        private IEnumerator SpawningBadge(BadgeDefinition defi)
        {
            var handle = m_BadgeAsset.InstantiateAsync(m_BadgeContainer);
            yield return handle;
            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                var go = handle.Result;
                if (go.TryGetComponent(out BadgeView view))
                {
                    view.Init(defi);
                    m_BadgeViews.Add(view);
                }
            }
            else
            {
                Debug.LogError($"Failed to load badge asset: {handle.OperationException}");
            }
        }


        private IEnumerator RefreshBadge(BadgeContent[] contents)
        {
            for (int i = 0; i < contents.Length; i++)
            {
                BadgeDefinition defi = contents[i].Definition;
                if (HasBadgeView(defi, out var view))
                {
                    view.Init(defi);
                }
                else
                {
                    yield return StartCoroutine(SpawningBadge(defi));
                }
            }
        }

        protected override void ShowInternal()
        {
            base.ShowInternal();
            BadgeContent[] contents = GetBadgeManager().GetAllBadges();
            StartCoroutine(RefreshBadge(contents));
        }
    }
}
