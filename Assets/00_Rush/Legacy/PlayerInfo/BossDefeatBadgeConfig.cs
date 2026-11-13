using LegionKnight;
using UnityEngine;

namespace Rush
{
    [CreateAssetMenu(menuName = "Legion Knight/Boss Defeat Badge")]
    public sealed class BossDefeatBadgeConfig : BadgeConfig
    {
        [SerializeField] private BossUnitConfig m_Boss;
        [SerializeField] private bool m_HiddenUntilDefeated;
        public bool HiddenUntilDefeated => m_HiddenUntilDefeated;
        public BossUnitConfig Boss => m_Boss;
    }
}
