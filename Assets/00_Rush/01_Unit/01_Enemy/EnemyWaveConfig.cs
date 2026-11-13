using UnityEngine;

namespace Rush
{
    [System.Serializable]
    public sealed class EnemyWaveSpawnPolicyField
    {
        [SerializeField, Min(0)] private int m_MaxConcurrentMinions;
        [SerializeField, Min(0)] private int m_MaxConcurrentPriorityMinions;
        [SerializeField] private MinionUnitConfig[] m_PriorityMinions = new MinionUnitConfig[0];
        [SerializeField] private bool m_ClearMinionsBeforeBoss;

        public int MaxConcurrentMinions => m_MaxConcurrentMinions;
        public int MaxConcurrentPriorityMinions => m_MaxConcurrentPriorityMinions;
        public MinionUnitConfig[] PriorityMinions => m_PriorityMinions;
        public bool ClearMinionsBeforeBoss => m_ClearMinionsBeforeBoss;
    }

    [CreateAssetMenu(fileName = "EnemyWave", menuName = "Rush/Level/EnemyWave")]
    public class EnemyWaveConfig : Configuration, IHasIcon
    {
        [SerializeField]
        private Sprite m_Icon;
        [SerializeField]
        private int m_RestThreshold;
        [SerializeField]
        private BossWaveField m_BossWaveField;
        [SerializeField]
        private MinionWaveField[] m_MinionWaveFields;
        [SerializeField] private EnemyWaveSpawnPolicyField m_SpawnPolicy = new();
        public EnemyWaveSpawnPolicyField SpawnPolicy => m_SpawnPolicy;
        public int RestThreshold => m_RestThreshold;
        public BossWaveField BossWaveField => m_BossWaveField;
        public MinionWaveField[] MinionWaveFields => m_MinionWaveFields;

        public Sprite Icon => m_Icon;
    }
}
