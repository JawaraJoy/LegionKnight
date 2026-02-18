using MoreMountains.Tools;
using UnityEngine;

namespace Rush
{
    [RequireComponent(typeof(BoxCollider2D))]
    public class PlatformAttack : MonoBehaviour, IHasAttacker, IHasAbilityContext
    {
        [SerializeField]
        private Platform2D m_MainPlatform;
        [SerializeField, MMReadOnly]
        private AttackerField m_AttackerField;
        public AttackerField AttackerField => m_AttackerField;
        private AbilityContext m_AbilityContext;
        public IAbilityContext AbilityContext => m_AbilityContext;

        public bool Initialized => m_AbilityContext.Initialized;

        public void Init(IAbilityContext abilityContext)
        {
            m_AbilityContext = new AbilityContext(abilityContext.AbilityDeliver, abilityContext.SkillContext);
            AbilityConfig damageConfig = m_AbilityContext.AbilityDeliver.Config;
            if (damageConfig is DamageAbilityConfig damageAbilityConfig) 
            {
                float attack = AbilityUltility.GetFinalPowerAmount(m_AbilityContext);
                int roundedAttack = Mathf.RoundToInt(attack);
                float damageBasedTargetMaxHp = damageAbilityConfig.DamageBasedTargetMaxHP;
                bool isTrueDamage = damageAbilityConfig.IsTrueDamage;
                bool isFatalDamage = damageAbilityConfig.IsFatalDamage;

                m_AttackerField = new(roundedAttack, damageBasedTargetMaxHp, isTrueDamage, isFatalDamage);
            }
        }
    }
}
