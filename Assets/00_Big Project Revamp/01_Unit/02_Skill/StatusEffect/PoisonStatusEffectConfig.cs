using UnityEngine;

namespace Rush
{
    [CreateAssetMenu(fileName = "Poison", menuName = "Rush/Combat/StatusEff/Poison", order = 2)]
    public class PoisonStatusEffectConfig : StatusEffectConfig
    {
        [Tooltip("you have to add skill configuration with this ability on Infector Skills")]
        [SerializeField]
        private AbilityConfig m_AbilityOnAppliedToInfected;
        public override void ApplyEffect(StatusEffectContext context)
        {
            TakePoisonDamage(context);
        }
        private void TakePoisonDamage(StatusEffectContext context)
        {
            if (HasInfectorSkillController(context, out SkillController controller))
            {
                if (controller.HasAbility(m_AbilityOnAppliedToInfected, out AbilityDeliver abilityDeliver))
                {
                    Damageable infectedDamageable = GetInfectedDamageable(context);
                    if (infectedDamageable != null)
                    {
                        IAbilityContext abilityContext = context.AbilityContext;
                        infectedDamageable.TakeDamage(abilityContext);
                    }
                }
            }
        }
        private Damageable GetInfectedDamageable(StatusEffectContext context)
        {
            if (context.Infected.HasBind(out Damageable damageable))
            {
                return damageable;
            }
            return null;
        }
        private SkillController GetInfectorStatController(StatusEffectContext context)
        {
            Unit infector = context.AbilityContext.SkillContext.ModuleContext.Unit;
            if (infector.HasBind(out SkillController controller))
            {
                return controller;
            }
            return null;
        }

        private bool HasInfectorSkillController(StatusEffectContext context, out SkillController controller)
        {
            controller = GetInfectorStatController(context);
            return controller != null;
        }

        public override void DoneEffect(StatusEffectContext context)
        {
            //throw new System.NotImplementedException();
        }

        public override void OnStackAdded(StatusEffectContext context)
        {
            //throw new System.NotImplementedException();
        }

        public override void OnStackRemoved(StatusEffectContext context)
        {
            TakePoisonDamage(context);
        }
    }
}
