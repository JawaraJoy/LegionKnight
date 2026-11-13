using LegionKnight;
using UnityEngine;

namespace Rush
{
    [CreateAssetMenu(menuName = "Legion Knight/Hero Profile Badge")]
    public sealed class HeroProfileBadgeConfig : BadgeConfig
    {
        [SerializeField] private HeroUnitConfig m_Hero;

        public HeroUnitConfig Hero => m_Hero;

        // The badge system counts milestones from one. Recruitment is the first, at zero stars.
        public int ProgressFor(HeroUnit hero) =>
            hero != null && hero.Owned && hero.HeroConfig == m_Hero ? 1 + hero.Star : 0;
    }
}

