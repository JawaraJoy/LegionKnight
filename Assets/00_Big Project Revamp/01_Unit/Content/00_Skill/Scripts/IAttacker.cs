using MoreMountains.Tools;
using UnityEngine;

namespace Rush
{
    public interface IAttacker 
    {
        AttackerField AttackerField { get; }
    }
    [System.Serializable]
    public class AttackerField
    {
        [SerializeField]
        private int m_Damage;
        [SerializeField]
        private float m_DamageBasedTargetMaxHP;
        [SerializeField]
        private DamageType m_Type = DamageType.CompareWithDefense;
        [SerializeField]
        private bool m_IsTrueDamage = false;
        [SerializeField]
        private bool m_FatalDamage = false;
        public int Damage => m_Damage;
        public DamageType Type => m_Type;

        public float DamageBasedTargetMaxHP => m_DamageBasedTargetMaxHP;
        public AttackerField(int damage, float damageBasedTargetMaxHP, bool isTrueDamage, bool fatalDamage)
        {
            m_Damage = damage;
            m_DamageBasedTargetMaxHP = damageBasedTargetMaxHP;
            m_IsTrueDamage = isTrueDamage;
            m_FatalDamage = fatalDamage;
        }
    }
    public enum DamageType
    {
        CompareWithDefense = 0,
        TrueDamage = 1,
        FatalDamage = 2,
    }
}
