using UnityEngine;

namespace Rush
{
    public abstract class DamageAbilityConfig : AbilityConfig
    {
        // some casting has concentrate point, if the concentrate point is interrupted by damage, the casting will be interupted,
        // and the skill will not be delivered,
        [SerializeField, Tooltip("The point to apply interupt enemy casting")]
        private int m_InteruptDamage = 0;
        [SerializeField]
        private bool m_CanCriticalHit = false;
        [SerializeField]
        private float m_DamageBasedTargetMaxHP = 0f;
        [SerializeField]
        private DamageType m_DamageType = DamageType.CompareWithDefense;
        [SerializeField]
        protected SpawnSetupField m_SpawningSetup;
        [SerializeField]
        protected bool m_DeliverLookAtTargetOnActivate;
        
        [SerializeField]
        protected TargetDistributeMode m_TargetDistributeMode;
        [SerializeField, Range(0f, 1f), Tooltip("Shield restored to the attacker from actual target health damage.")]
        private float m_ShieldFromDamageRate;
        public float ShieldFromDamageRate => m_ShieldFromDamageRate;

        [SerializeField, Range(0f, 1f), Tooltip("Health restored to the attacker from actual target health damage, independently of stat lifesteal.")]
        private float m_HealFromDamageRate;
        public float HealFromDamageRate => m_HealFromDamageRate;

        public int InteruptDamage => m_InteruptDamage;
        public SpawnSetupField SpawningSetup => m_SpawningSetup;
        public TargetDistributeMode TargetDistributeMode => m_TargetDistributeMode;
        public bool DeliverLookAtTargetOnActivate => m_DeliverLookAtTargetOnActivate;
        public float DamageBasedTargetMaxHP => m_DamageBasedTargetMaxHP;
        public DamageType DamageType => m_DamageType;
        public bool CanCriticalHit => m_CanCriticalHit;
        protected abstract int GetDamageInternal(IAbilityContext context);
        public virtual int GetDamage(IAbilityContext context)
        {
            return GetDamageInternal(context);
        }
    }
}
