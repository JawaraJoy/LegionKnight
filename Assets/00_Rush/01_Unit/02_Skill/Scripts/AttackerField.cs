using UnityEngine;

namespace Rush
{
    [System.Serializable]
    public class AttackerField : IHasAttack
    {
        [SerializeField]
        private bool m_Enabled = true;

        [SerializeField]
        protected int m_Attack;

        [SerializeField]
        protected float m_DamageBasedTargetMaxHP;

        [SerializeField]
        protected DamageType m_Type = DamageType.CompareWithDefense;

        [Header("Critical")]
        [SerializeField]
        private bool m_IsCritical;

        [SerializeField]
        private float m_CriticalDamageFlat;

        [SerializeField]
        private float m_CriticalDamageRate;

        public int Attack => m_Attack;
        public DamageType Type => m_Type;
        public float DamageBasedTargetMaxHP => m_DamageBasedTargetMaxHP;
        public bool Enabled => m_Enabled;

        public bool IsCritical => m_IsCritical;
        public float CriticalDamageFlat => m_CriticalDamageFlat;
        public float CriticalDamageRate => m_CriticalDamageRate;

        public AttackerField(
            int attack,
            float damageBasedTargetMaxHP,
            DamageType damageType,
            bool isCritical = false,
            float criticalDamageFlat = 0f,
            float criticalDamageRate = 0f)
        {
            m_Attack = attack;
            m_DamageBasedTargetMaxHP = damageBasedTargetMaxHP;
            m_Type = damageType;
            m_Enabled = true;

            m_IsCritical = isCritical;
            m_CriticalDamageFlat = criticalDamageFlat;
            m_CriticalDamageRate = criticalDamageRate;
        }

        public void SetAttack(int attack)
        {
            m_Attack = attack;
        }

        public void SetEnabled(bool enabled)
        {
            m_Enabled = enabled;
        }

        public void SetCritical(bool isCritical, float criticalDamageFlat, float criticalDamageRate)
        {
            m_IsCritical = isCritical;
            m_CriticalDamageFlat = criticalDamageFlat;
            m_CriticalDamageRate = criticalDamageRate;
        }
    }
}