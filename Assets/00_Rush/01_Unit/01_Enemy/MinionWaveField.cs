using UnityEngine;

namespace Rush
{
    [System.Serializable]
    public class MinionWaveField : IHasIcon
    {
        [SerializeField]
        private Sprite m_Icon;
        [SerializeField]
        private int m_ThresholdToSpawn;
        [SerializeField]
        private SpawnShapeConfig m_SpawnShapeConfig;
        [SerializeField]
        private MinionUnitConfig[] m_MinionConfigs;
        public int ThresholdToSpawn => m_ThresholdToSpawn;
        public MinionUnitConfig[] MinionConfigs => m_MinionConfigs;
        public SpawnShapeConfig SpawnShapeConfig => m_SpawnShapeConfig;

        public Sprite Icon => m_Icon;
    }
}
