using LegionKnight;
using UnityEngine;

namespace Rush
{
    [DisallowMultipleComponent]
    public sealed class HeroProfileBadgeTracker : MonoBehaviour
    {
        [SerializeField] private HeroesCollection m_Heroes;
        [SerializeField] private BadgeManager m_Badges;
        private bool m_Initialized;
        public event System.Action ProgressChanged;

        private void OnEnable()
        {
            m_Heroes.OnCharacterOwned.AddListener(RefreshHero);
            m_Heroes.OnCharacterStarUp.AddListener(RefreshHero);
            if (m_Initialized) RefreshInternal(null);
        }

        private void OnDisable()
        {
            m_Heroes.OnCharacterOwned.RemoveListener(RefreshHero);
            m_Heroes.OnCharacterStarUp.RemoveListener(RefreshHero);
        }

        // Wired after the hero roster and saved badge claim states have both loaded.
        public void Initialize()
        {
            m_Initialized = true;
            RefreshInternal(null);
        }

        private void RefreshHero(HeroUnitConfig hero)
        {
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

