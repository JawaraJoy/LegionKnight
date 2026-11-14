using LegionKnight;
using UnityEngine;

namespace Rush
{
    [DisallowMultipleComponent]
    public sealed class HeroProfileBadgeTracker : MonoBehaviour
    {
        [SerializeField] private HeroesCollection m_Heroes;
        [SerializeField] private BadgeManager m_Badges;
        [SerializeField] private BadgeUnlockChannel m_Notices;
        private const float c_NoticeInterval = .25f;
        private int[] m_Announced;
        private float m_NextCheck;
        private bool m_MilestonesDirty;
        [SerializeField] private PlayerProgression m_PlayerLevel;
        private bool m_Initialized;
        public event System.Action ProgressChanged;

        private void OnEnable()
        {
            m_Heroes.OnCharacterOwned.AddListener(RefreshHero);
            m_Heroes.OnCharacterStarUp.AddListener(RefreshHero);
            if (m_Notices != null)
            {
                UnitConfig.BossKilled += MarkMilestonesInternal;
                SpinWheel.SpinConsumed += MarkMilestonesInternal;
                Energy.EnergySpent += MarkMilestonesInternal;
                PlatformHandler.PerfectStreakReached += MarkMilestonesInternal;
                m_Heroes.OnCharacterLevelUp.AddListener(MarkHeroMilestonesInternal);
                m_PlayerLevel?.AddOnLevelUp(MarkMilestonesInternal);
                m_MilestonesDirty = true;
            }
            if (m_Initialized) RefreshInternal(null);
        }

        private void OnDisable()
        {
            m_Heroes.OnCharacterOwned.RemoveListener(RefreshHero);
            m_Heroes.OnCharacterStarUp.RemoveListener(RefreshHero);
            UnitConfig.BossKilled -= MarkMilestonesInternal;
            SpinWheel.SpinConsumed -= MarkMilestonesInternal;
            Energy.EnergySpent -= MarkMilestonesInternal;
            PlatformHandler.PerfectStreakReached -= MarkMilestonesInternal;
            m_Heroes.OnCharacterLevelUp.RemoveListener(MarkHeroMilestonesInternal);
            m_PlayerLevel?.RemoveOnLevelUp(MarkMilestonesInternal);
        }

        // Wired after the hero roster and saved badge claim states have both loaded.
        public void Initialize()
        {
            if (!m_Initialized && m_Notices != null)
            {
                var badges = m_Badges.GetAllBadges();
                m_Announced = new int[badges.Length];
                for (int i = 0; i < badges.Length; i++)
                    m_Announced[i] = EarnedCount(badges[i]);
            }
            m_Initialized = true;
            RefreshInternal(null);
        }

        // Existing badge sources update pending tiers through different APIs. Compare earned
        // totals so claims, asset reloads and repeated defeats cannot replay an unlock.
        private static int EarnedCount(BadgeContent badge) => badge?.Config == null ? 0 :
            (badge.IsUnlocked ? badge.CurrentUpgradeLevel + 1 : 0) + badge.UnClaimedReward;

        private void MarkMilestonesInternal() => m_MilestonesDirty = true;
        private void MarkMilestonesInternal(int value) => m_MilestonesDirty = true;
        private void MarkHeroMilestonesInternal(HeroUnitConfig hero) => m_MilestonesDirty = true;

        private void LateUpdate()
        {
            if (m_Announced == null || Time.unscaledTime < m_NextCheck) return;
            m_NextCheck = Time.unscaledTime + c_NoticeInterval;
            if (m_MilestonesDirty)
            {
                m_MilestonesDirty = false;
                m_Badges.RefreshMilestones();
            }
            var badges = m_Badges.GetAllBadges();
            for (int i = 0; i < badges.Length; i++)
            {
                var badge = badges[i];
                int earned = EarnedCount(badge);
                if (earned <= m_Announced[i]) continue;
                int previous = m_Announced[i];
                m_Announced[i] = earned;
                for (int tier = previous; tier < earned; tier++)
                    m_Notices.NotifyEarned(badge, tier);
                TenjinManager.Instance.SendEventToUnlockAchievement(badge.Config);
            }
        }

        private void RefreshHero(HeroUnitConfig hero)
        {
            m_MilestonesDirty = true;
            if (m_Initialized) RefreshInternal(hero);
        }

        private void RefreshInternal(HeroUnitConfig changedHero)
        {
            bool changed = false;
            foreach (var entry in m_Badges.GetAllBadges())
            {
                if (entry?.Config is not HeroProfileBadgeConfig badge || badge.Hero == null) continue;
                if (changedHero != null && changedHero != badge.Hero) continue;

                var hero = m_Heroes.GetHeroUnit(badge.Hero);
                int previous = entry.UnClaimedReward;
                entry.RefreshMilestones(badge.ProgressFor(hero));
                changed |= previous != entry.UnClaimedReward;
            }
            if (changed) ProgressChanged?.Invoke();
        }
    }
}

