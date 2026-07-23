using UnityEngine;

namespace Rush
{
    [CreateAssetMenu(fileName = "Defensive", menuName = "Rush/Combat/StatusEff/Defensive", order = 1)]
    public class DefensiveStatusEffectConfig : StatusEffectConfig
    {
        [SerializeField]
        private int m_RebornCount;

        [SerializeField]
        private DefensiveStatusField m_Base;
        [SerializeField]
        private DefensiveStatusField m_GrowthPerLevel;

        [Header("States")]
        [SerializeField]
        private bool m_Immortality;
        [SerializeField]
        private bool m_Invisibility;

        [SerializeField] 
        protected SkillConfig[] m_InfectorSkillToOnEffectDone;
        public override void OnEffectStarted(StatusEffectContext context)
        {
            if (context.Infected.HasBind(out Damageable damageable))
            {
                if (m_Invisibility)
                {
                    damageable.SetInvisible(true);
                }
                if (m_Immortality)
                {
                    damageable.SetImmortal(true);
                }
            }
        }
        public override void OnEffectEnded(StatusEffectContext context)
        {
            if (context.Infected.HasBind(out Damageable damageable))
            {
                if (m_Invisibility)
                {
                    damageable.SetInvisible(false);
                }
                if (m_Immortality)
                {
                    damageable.SetImmortal(false);
                }
            }
            Unit owner = context.AbilityContext.SkillContext.ModuleContext.Unit;
            if (owner.HasBind(out SkillController skillController))
            {
                skillController.ForceActives(m_InfectorSkillToOnEffectDone);
            }
        }

        public override void OnStackAdded(StatusEffectContext context)
        {
            if (context.Infected.HasBind(out Damageable damageable))
            {
                int statuslevel = context.AbilityContext.SkillContext.Skill.Progression.Level;

                damageable.AddRemainingRebornCount(m_RebornCount, true);
                damageable.AddShield(GetFinalValue(m_Base.Shield, m_GrowthPerLevel.Shield, statuslevel), true);
                damageable.AddShieldBasedOnDefendRate(GetFinalValue(m_Base.ShieldBasedDefendRate, m_GrowthPerLevel.ShieldBasedDefendRate, statuslevel), true);
                damageable.AddBarrier(GetFinalValue(m_Base.Barrier, m_GrowthPerLevel.Barrier, statuslevel), true);
                damageable.AddDamageReductionRate(GetFinalValue(m_Base.DamageReductionRate, m_GrowthPerLevel.DamageReductionRate, statuslevel));
            }
        }
        public override void OnStackRemoved(StatusEffectContext context)
        {
            if (context.Infected.HasBind(out Damageable damageable))
            {
                int statuslevel = context.AbilityContext.SkillContext.Skill.Progression.Level;

                damageable.AddRemainingRebornCount(-m_RebornCount, false);
                damageable.AddShield(-GetFinalValue(m_Base.Shield, m_GrowthPerLevel.Shield, statuslevel), false);
                damageable.AddShieldBasedOnDefendRate(-GetFinalValue(m_Base.ShieldBasedDefendRate, m_GrowthPerLevel.ShieldBasedDefendRate, statuslevel), false);
                damageable.AddBarrier(-GetFinalValue(m_Base.Barrier, m_GrowthPerLevel.Barrier, statuslevel), false);
                damageable.AddDamageReductionRate(-GetFinalValue(m_Base.DamageReductionRate, m_GrowthPerLevel.DamageReductionRate, statuslevel));
            }
        }

        private int GetFinalValue(int baseValue, int growthPerLevel, int level)
        {
            return baseValue + growthPerLevel * (level - 1);
        }
        private float GetFinalValue(float baseValue, float growthPerLevel, int level)
        {
            return baseValue + growthPerLevel * (level - 1);
        }
    }
}
