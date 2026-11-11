using UnityEngine;

namespace Rush
{
    [System.Serializable]
    public class DefensiveStatusField
    {
        [SerializeField]
        private int m_Shield;
        [SerializeField]
        private float m_ShieldBasedDefendRate;
        [SerializeField]
        private float m_ShieldBasedMaxHpRate;
        [SerializeField]
        private int m_Barrier;

        [Header("Stats")]
        [SerializeField]
        private float m_DamageReductionRate;
        public int Shield => m_Shield;
        public float ShieldBasedDefendRate => m_ShieldBasedDefendRate;
        public float ShieldBasedMaxHpRate => m_ShieldBasedMaxHpRate;
        public int Barrier => m_Barrier;
        public float DamageReductionRate => m_DamageReductionRate;

        
        public DefensiveStatusField(int shield, float shieldBasedDefendRate, int barrier, float damageReductionRate)
        {
            m_Shield = shield;
            m_ShieldBasedDefendRate = shieldBasedDefendRate;
            m_Barrier = barrier;
            m_DamageReductionRate = damageReductionRate;
        }
    }
}
