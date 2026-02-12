using UnityEngine;

namespace Rush
{
    [System.Serializable]
    public class PlatformDamageField : IAttacker
    {
        [SerializeField]
        private int m_BaseDamage = 10;
        [SerializeField]
        private float m_DamageBasedTargetMaxHP = 1.0f;
        [SerializeField]
        private bool m_TrueDamage = true;
        [SerializeField]
        private bool m_FatalDamage = true;

        public int BaseDamage => m_BaseDamage;
        public float DamageBasedTargetMaxHP => m_DamageBasedTargetMaxHP;
        public bool TrueDamage => m_TrueDamage;
        public bool FatalDamage => m_FatalDamage;

        public int GetFinalDamage(Damageable damageable)
        {
            float damageBaseMaxHp = damageable.MaxHealth * m_DamageBasedTargetMaxHP;
            int damageRounded = m_BaseDamage + Mathf.RoundToInt(damageBaseMaxHp);
            return damageRounded;
        }
    }
}
