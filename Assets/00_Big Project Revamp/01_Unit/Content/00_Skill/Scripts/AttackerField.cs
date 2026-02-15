using UnityEngine;

namespace Rush
{
    [System.Serializable]
    public class AttackerField : IHasAttack
    {
        [SerializeField]
        private int m_Attack;
        [SerializeField]
        private float m_DamageBasedTargetMaxHP;
        [SerializeField]
        private DamageType m_Type = DamageType.CompareWithDefense;
        [SerializeField]
        private bool m_IsTrueDamage = false;
        [SerializeField]
        private bool m_IsFatalDamage = false;
        public int Attack => m_Attack;
        public DamageType Type => m_Type;
        public bool IsTrueDamage => m_IsTrueDamage;
        public bool IsFatalDamage => m_IsFatalDamage;
        public float DamageBasedTargetMaxHP => m_DamageBasedTargetMaxHP;
        public AttackerField(int attack, float damageBasedTargetMaxHP, bool isTrueDamage, bool fatalDamage)
        {
            m_Attack = attack;
            m_DamageBasedTargetMaxHP = damageBasedTargetMaxHP;
            m_IsTrueDamage = isTrueDamage;
            m_IsFatalDamage = fatalDamage;
        }
    }
    
}
