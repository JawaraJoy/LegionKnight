using UnityEngine;

namespace LegionKnight
{
    public class CounterStat
    {
        [SerializeField]
        private float m_DamageCounterMultiplier = 1.0f;
        [SerializeField]
        private int m_DamageCounterFlat = 0;

        [SerializeField]
        private float m_DamageCounterMultiplierGrowth = 0.1f;
        [SerializeField]
        private int m_DamageCounterFlatGrowth = 1;

        public float damageCounterMultiplier => m_DamageCounterMultiplier;
        public int damageCounterFlat => m_DamageCounterFlat;
        public float damageCounterMultiplierGrowth => m_DamageCounterMultiplierGrowth;
        public int damageCounterFlatGrowth => m_DamageCounterFlatGrowth;
    }
}
