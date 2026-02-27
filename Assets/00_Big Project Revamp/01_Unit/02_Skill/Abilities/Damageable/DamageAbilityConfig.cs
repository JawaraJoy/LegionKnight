using UnityEngine;

namespace Rush
{
    public abstract class DamageAbilityConfig : AbilityConfig
    {
        [SerializeField]
        private float m_DamageBasedTargetMaxHP = 0f;
        [SerializeField]
        private DamageType m_DamageType = DamageType.CompareWithDefense;
        public float DamageBasedTargetMaxHP => m_DamageBasedTargetMaxHP;
        public DamageType DamageType => m_DamageType;
        protected abstract int GetDamageInternal(IAbilityContext context);
        public virtual int GetDamage(IAbilityContext context)
        {
            return GetDamageInternal(context);
        }
    }
}
