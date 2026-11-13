using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace LegionKnight
{
    public class AchievementBadgeView : UIView
    {
        [SerializeField] private Transform m_BadgeContainer;
        [SerializeField] private AssetReferenceGameObject m_BadgeAsset;
        private readonly Dictionary<BadgeConfig, BadgeView> m_Views = new();
        private bool m_Loading;

        protected override void ShowInternal()
        {
            base.ShowInternal();
            RefreshBadges();
        }

        private async void RefreshBadges()
        {
            var manager = Player.Instance.BadgeManager;
            manager.RefreshMilestones();
            foreach (var pair in m_Views) pair.Value.Init(pair.Key);
            if (m_Loading) return;
            m_Loading = true;
            try
            {
                foreach (var entry in manager.GetAllBadges())
                {
                    var config = entry.Config;
                    if (config != null && config.Goal == ProfileBadgeGoal.Event && !entry.IsUnlocked && !entry.CanClaim) continue;
                    if (config == null || config.Upgrade.Length == 0 || m_Views.ContainsKey(config)) continue;
                    var handle = m_BadgeAsset.InstantiateAsync(m_BadgeContainer, false);
                    var row = await handle.Task;
                    if (!this || !row) { if (handle.IsValid()) Addressables.ReleaseInstance(handle); return; }
                    if (!row.TryGetComponent<BadgeView>(out var view)) { Addressables.ReleaseInstance(row); continue; }
                    view.Init(config);
                    m_Views.Add(config, view);
                }
            }
            catch (System.Exception ex) { Debug.LogWarning($"Profile badges could not load: {ex.Message}"); }
            finally { m_Loading = false; }
        }
        private void OnDestroy()
        {
            foreach (var view in m_Views.Values) if (view) Addressables.ReleaseInstance(view.gameObject);
        }
    }
}
