using UnityEngine;

namespace Rush
{
    [System.Serializable]
    public class PlatformAttackField : AttackerField, IHasAbilityContext
    {

        private IAbilityContext m_AbilityContext;

        public PlatformAttackField(int attack, float damageBasedTargetMaxHP, DamageType damageType) : base(attack, damageBasedTargetMaxHP, damageType)
        {
        }

        public IAbilityContext AbilityContext => m_AbilityContext;

        public bool Initialized => m_AbilityContext.Initialized;

        public void Init(IAbilityContext abilityContext)
        {
            m_AbilityContext = abilityContext;
            AbilityConfig abilityConfig = m_AbilityContext.AbilityDeliver.AbilityConfig;
            if (abilityConfig is DamageAbilityConfig damageAbilityConfig)
            {
                m_Attack = damageAbilityConfig.GetDamage(m_AbilityContext);
                m_DamageBasedTargetMaxHP = damageAbilityConfig.DamageBasedTargetMaxHP;
                m_Type = damageAbilityConfig.DamageType;
            }
        }
    }
}
