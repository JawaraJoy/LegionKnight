using UnityEngine;

namespace Rush
{
    public class PoisonStatusEffectConfig : StatusEffectConfig
    {
        [SerializeField]
        private ScalingWithStat m_ScalingWithStat = ScalingWithStat.Attack;
        [SerializeField]
        private PowerField m_DamagOnApplied;
        [SerializeField]
        private PowerField m_DamagePerUpdateStack;
        [SerializeField]
        private SkillConfig m_SkillOnAppliedToInfected;
        public override void ApplyEffect(StatusEffectContext context)
        {
            
        }

        private void ApplyDamageToInfected(StatusEffectContext context, PowerField power)
        {
            if (HasInfectorStatController(context, out StatController controller))
            {
                int damage = GetDamageByScaling(controller, power);
                if (context.Infected.HasBind(out Damageable damageable))
                {
                    damageable.TakeDamage;
                }
            }
        }
        private int GetDamageByScaling(StatController controller, PowerField power)
        {
            int damage = PowerField.GetFinalPowerByStatScaling(m_ScalingWithStat, controller);
            float initialDamage = power.InitialAmount;
            float multiplierDamage = power.MultiplierAmount;

            float finalDamage = initialDamage + (damage * multiplierDamage);
            return Mathf.RoundToInt(finalDamage);
        }
        private StatController GetInfectorStatController(StatusEffectContext context)
        {
            Unit infector = context.AbilityContext.SkillContext.ModuleContext.Unit;
            if (infector.HasBind(out StatController controller))
            {
                return controller;
            }
            return null;
        }

        private bool HasInfectorStatController(StatusEffectContext context, out StatController controller)
        {
            controller = GetInfectorStatController(context);
            return controller != null;
        }

        public override void DoneEffect(StatusEffectContext context)
        {
            throw new System.NotImplementedException();
        }

        public override void OnStackAdded(StatusEffectContext context)
        {
            throw new System.NotImplementedException();
        }

        public override void OnStackRemoved(StatusEffectContext context)
        {
            throw new System.NotImplementedException();
        }
    }
}
