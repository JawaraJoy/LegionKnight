using UnityEngine;

namespace Rush
{
    [CreateAssetMenu(fileName = "Boss_", menuName = "Rush/Unit/Boss Enemy", order = 2)]

    public class BossUnitConfig : EnemyUnitConfig, IHasPlatform
    {
        [SerializeField]
        private PlatformConfig[] m_UniquePlatforms;
        public PlatformConfig[] UniquePlatforms => m_UniquePlatforms;
    }
}
