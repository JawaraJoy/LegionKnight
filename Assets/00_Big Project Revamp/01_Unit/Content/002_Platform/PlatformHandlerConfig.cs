using UnityEngine;

namespace Rush
{
    [CreateAssetMenu(fileName = "PlatformHandler", menuName = "Rush/Handler/PlatformHandler")]
    public class PlatformHandlerConfig : Configuration
    {
        [SerializeField]
        private PlatformConfig[] m_InitialPlatformConfigs;
        [SerializeField]
        private LayerMask m_FailLayer;
        [SerializeField]
        private float m_SpawnHorizontalDistanceFromPost = 15f;
        [SerializeField, Range(0f, 2f)]
        private float m_OffSiteReachHorizontalPost = 1.0f;
        [SerializeField]
        private float m_InitialSpawnDelay = 1f;
        [SerializeField]
        private float m_GlobalSpawnDelay = 1f;
        [SerializeField]
        private float m_MinGlobalSpeedRate = 1f;
        [SerializeField]
        private float m_MaxGlobalSpeedRate = 1f;
        [SerializeField]
        private float m_GlobalPerfectTouchRange = 0.3f;
        [SerializeField]
        private int m_MaxStackedPlatforms = 15;
        [SerializeField]
        private SkillActivatorConfig[] m_GlobalSkillForPlayerOnTouchDown;
        public PlatformConfig[] InitialPlatformConfigs => m_InitialPlatformConfigs;
        public int MaxStackedPlatforms => m_MaxStackedPlatforms;
        public float MaxGlobalSpeedRate => m_MaxGlobalSpeedRate;
        public float MinGlobalSpeedRate => m_MinGlobalSpeedRate;
        public float InitialSpawnDelay => m_InitialSpawnDelay;
        public float GlobalSpawnDelay => m_GlobalSpawnDelay;
        public float GlobalPerfectTouchRange => m_GlobalPerfectTouchRange;
        public float SpawnHorizontalDistanceFromPost => m_SpawnHorizontalDistanceFromPost;
        public float OffSiteReachHorizontalPost => m_OffSiteReachHorizontalPost;
        public SkillActivatorConfig[] GlobalSkillForPlayerOnTouchDown => m_GlobalSkillForPlayerOnTouchDown;
        public LayerMask FailLayer => m_FailLayer;
    }
}
