using UnityEngine;

namespace Rush
{
    public abstract class DamageAbilityConfig : AbilityConfig
    {
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
