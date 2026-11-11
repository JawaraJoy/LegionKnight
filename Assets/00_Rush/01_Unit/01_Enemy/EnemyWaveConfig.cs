using UnityEngine;

namespace Rush
{
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
        public int RestThreshold => m_RestThreshold;
        public BossWaveField BossWaveField => m_BossWaveField;
        public MinionWaveField[] MinionWaveFields => m_MinionWaveFields;

        public Sprite Icon => m_Icon;
    }
}
