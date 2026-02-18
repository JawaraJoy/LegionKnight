using UnityEngine;

namespace Rush
{
    public abstract class DamageAbilityConfig : AbilityConfig
    {
        [SerializeField]
        private float m_DamageBasedTargetMaxHP = 0f;
        [SerializeField]
        private bool m_IsTrueDamage = false;
        [SerializeField]
        private bool m_IsFatalDamage = false;
        public bool IsTrueDamage => m_IsTrueDamage;
        public bool IsFatalDamage => m_IsFatalDamage;
        public float DamageBasedTargetMaxHP => m_DamageBasedTargetMaxHP;
        protected abstract int GetDamage(AbilityContext context);
    }
}
