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

        [SerializeField, Min(0f), Tooltip("Linear growth per skill upgrade for the level-one shield and damage reduction.")]
        private float m_LevelOnePowerGrowth;
        public DefensiveStatusField BasePower => m_Base;
        public DefensiveStatusField GrowthPerLevel => m_GrowthPerLevel;
        public float LevelOnePowerGrowth => m_LevelOnePowerGrowth;
        public int RebornCount => m_RebornCount;
        public bool Immortality => m_Immortality;
        public bool Invisibility => m_Invisibility;
        public SkillConfig[] SkillsOnEffectDone => m_InfectorSkillToOnEffectDone ?? System.Array.Empty<SkillConfig>();
        public override bool TracksStackPower => m_LevelOnePowerGrowth > 0f;

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
                if (TracksStackPower)
                {
                    float factor = 1f + m_LevelOnePowerGrowth * (statuslevel - 1);
                    int levelOneShield = m_Base.Shield + Mathf.RoundToInt(damageable.Defense * m_Base.ShieldBasedDefendRate);
                    int shield = Mathf.RoundToInt(levelOneShield * factor);
                    int before = damageable.Shield;
                    if (shield != 0) damageable.AddShield(shield, true);
                    context.AppliedShield = damageable.Shield - before;
                    context.AppliedReductionRate = m_Base.DamageReductionRate * factor;
                    damageable.AddStatusDamageReductionRate(context.AppliedReductionRate);
                }
                else
                {
                    damageable.AddShield(GetFinalValue(m_Base.Shield, m_GrowthPerLevel.Shield, statuslevel), true);
                    damageable.AddShieldBasedOnDefendRate(GetFinalValue(m_Base.ShieldBasedDefendRate, m_GrowthPerLevel.ShieldBasedDefendRate, statuslevel), true);
                    damageable.AddDamageReductionRate(GetFinalValue(m_Base.DamageReductionRate, m_GrowthPerLevel.DamageReductionRate, statuslevel));
                }
                damageable.AddBarrier(GetFinalValue(m_Base.Barrier, m_GrowthPerLevel.Barrier, statuslevel), true);
            }
        }
        public override void OnStackRemoved(StatusEffectContext context)
        {
            if (context.Infected.HasBind(out Damageable damageable))
            {
                int statuslevel = context.AbilityContext.SkillContext.Skill.Progression.Level;

                damageable.AddRemainingRebornCount(-m_RebornCount, false);
                if (TracksStackPower)
                {
                    if (context.AppliedShield != 0) damageable.AddShield(-context.AppliedShield, false);
                    damageable.AddStatusDamageReductionRate(-context.AppliedReductionRate);
                }
                else
                {
                    damageable.AddShield(-GetFinalValue(m_Base.Shield, m_GrowthPerLevel.Shield, statuslevel), false);
                    damageable.AddShieldBasedOnDefendRate(-GetFinalValue(m_Base.ShieldBasedDefendRate, m_GrowthPerLevel.ShieldBasedDefendRate, statuslevel), false);
                    damageable.AddDamageReductionRate(-GetFinalValue(m_Base.DamageReductionRate, m_GrowthPerLevel.DamageReductionRate, statuslevel));
                }
                damageable.AddBarrier(-GetFinalValue(m_Base.Barrier, m_GrowthPerLevel.Barrier, statuslevel), false);
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
