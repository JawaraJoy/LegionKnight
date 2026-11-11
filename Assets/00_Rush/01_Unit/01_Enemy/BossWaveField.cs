using UnityEngine;

namespace Rush
{
    [System.Serializable]
    public class BossWaveField : IHasIcon
    {
        [SerializeField]
        private Sprite m_Icon;
        [SerializeField]
        private int m_ThresholdToSpawn;
        [SerializeField]
        private BossUnitConfig m_BossConfig;
        public int ThresholdToSpawn => m_ThresholdToSpawn;
        public BossUnitConfig BossConfig => m_BossConfig;

        public Sprite Icon => m_Icon;
    }
}
