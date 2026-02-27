using UnityEngine;

namespace Rush
{
    [System.Serializable]
    public class AttackerField : IHasAttack
    {
        [SerializeField]
        protected int m_Attack;
        [SerializeField]
        protected float m_DamageBasedTargetMaxHP;
        [SerializeField]
        protected DamageType m_Type = DamageType.CompareWithDefense;
        public int Attack => m_Attack;
        public DamageType Type => m_Type;
        public float DamageBasedTargetMaxHP => m_DamageBasedTargetMaxHP;
        public AttackerField(int attack, float damageBasedTargetMaxHP, DamageType damageType)
        {
            m_Attack = attack;
            m_DamageBasedTargetMaxHP = damageBasedTargetMaxHP;
            m_Type = damageType;
        }
    }
    
}
