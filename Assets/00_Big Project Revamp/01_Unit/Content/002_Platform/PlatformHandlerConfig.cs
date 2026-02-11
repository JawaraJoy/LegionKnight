using UnityEngine;

namespace Rush
{
    [CreateAssetMenu(fileName = "PlatformHandler", menuName = "Rush/Handler/PlatformHandler")]
    public class PlatformHandlerConfig : Configuration
    {
        [SerializeField]
        private PlatformConfig[] m_InitialPlatformConfigs;
        [SerializeField]
        private float m_SpawnHorizontalDistanceFromPost = 15f;
        [SerializeField]
        private float m_InitialSpawnDelay = 1f;
        [SerializeField]
        private float m_GlobalSpawnDelay = 1f;
        [SerializeField]
        private float m_GlobalSpeedRate = 1f;
        [SerializeField]
        private float m_GlobalPerfectTouchRange = 0.3f;
        [SerializeField]
        private SkillActivatorConfig[] m_GlobalSkillForPlayerOnTouchDown;
        public PlatformConfig[] InitialPlatformConfigs => m_InitialPlatformConfigs;
        public float GlobalSpeedRate => m_GlobalSpeedRate;
        public float InitialSpawnDelay => m_InitialSpawnDelay;
        public float GlobalSpawnDelay => m_GlobalSpawnDelay;
        public float GlobalPerfectTouchRange => m_GlobalPerfectTouchRange;
        public SkillActivatorConfig[] GlobalSkillForPlayerOnTouchDown => m_GlobalSkillForPlayerOnTouchDown;
    }
}
