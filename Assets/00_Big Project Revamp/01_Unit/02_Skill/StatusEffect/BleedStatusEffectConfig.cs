using System.Collections.Generic;
using UnityEngine;

namespace Rush
{
    // the status to apply additional damage to the infected when stack added or on done
    [CreateAssetMenu(fileName = "Bleed", menuName = "Rush/Combat/StatusEff/Bleed", order = 2)]
    public class BleedStatusEffectConfig : StatusEffectConfig
    {
        [Tooltip("you have to add skill configuration with this ability on Infector Skills")]
        [SerializeField]
        private DirectDamageAbilityConfig m_DamageOnStackAddedToInfected;
        [SerializeField]
        private DirectDamageAbilityConfig m_DamageOnDoneToInfected;
        public override void OnEffectStarted(StatusEffectContext context)
        {
            
        }
        private void TakeBleedDamage(StatusEffectContext context, DirectDamageAbilityConfig directDamageAbility)
        {
            if (HasInfectorSkillController(context, out SkillController controller))
            {
                if (controller.HasAbility(directDamageAbility, out AbilityDeliver abilityDeliver))
                {
                    Damageable infectedDamageable = GetInfectedDamageable(context);
                    if (abilityDeliver is DirectDamager damager)
                    {
                        damager.ActiveOverrideTarget(new List<ITargetable>() { infectedDamageable });
                    }
                    /*if (infectedDamageable != null)
                    {
                        IAbilityContext abilityContext = abilityDeliver.AbilityContext;
                        infectedDamageable.TakeDamage(abilityContext);
                    }*/
                    
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

        public override void OnEffectEnded(StatusEffectContext context)
        {
            TakeBleedDamage(context, m_DamageOnDoneToInfected);
        }

        public override void OnStackAdded(StatusEffectContext context)
        {
            TakeBleedDamage(context, m_DamageOnStackAddedToInfected);
        }

        public override void OnStackRemoved(StatusEffectContext context)
        {
            //TakePoisonDamage(context);
        }
    }
}
