using System.Collections.Generic;
using UnityEngine;

namespace Rush
{
    // the status to apply additional damage to the infected when stack added or on done
    [CreateAssetMenu(fileName = "StatusEffect_", menuName = "Rush/Combat/StatusEff/All In", order = 2)]
    public class AllStatusEffectConfig : StatusEffectConfig
    {
        [Header("Defense Effect")]
        [SerializeField]
        private int m_RebornCount;
        [SerializeField]
        private DefensiveStatusField m_Base;
        [SerializeField]
        private DefensiveStatusField m_GrowthPerLevel;
        [SerializeField]
        private bool m_Immortality;
        [SerializeField]
        private bool m_Invisibility;

        [Header("Silence Effect")]
        [SerializeField]
        private SkillConfig[] m_SpecificSkillsToSilence;
        [SerializeField]
        private SkillCategoryConfig[] m_CategoriesSkillToSilence;


        [Header("Skill Events")]
        [SerializeField]
        protected StatusEffecSkill[] m_OnStartActivateSkills;
        [SerializeField]
        protected StatusEffecSkill[] m_OnDoneActivateSkills;
        [SerializeField]
        private bool m_RequireFullStackForDoneSkills;
        [SerializeField]
        protected StatusEffecSkill[] m_OnStackAddedActivateSkills;
        [SerializeField]
        protected StatusEffecSkill[] m_OnStackRemovedActivateSkills;
        public DefensiveStatusField BasePower => m_Base;
        public DefensiveStatusField GrowthPerLevel => m_GrowthPerLevel;
        public int RebornCount => m_RebornCount;
        public bool Immortality => m_Immortality;
        public bool Invisibility => m_Invisibility;
        public StatusEffecSkill[] SkillsOnStart => m_OnStartActivateSkills ?? System.Array.Empty<StatusEffecSkill>();
        public StatusEffecSkill[] SkillsOnDone => m_OnDoneActivateSkills ?? System.Array.Empty<StatusEffecSkill>();
        public StatusEffecSkill[] SkillsOnStackAdded => m_OnStackAddedActivateSkills ?? System.Array.Empty<StatusEffecSkill>();
        public StatusEffecSkill[] SkillsOnStackRemoved => m_OnStackRemovedActivateSkills ?? System.Array.Empty<StatusEffecSkill>();
        public override void OnEffectStarted(StatusEffectContext context)
        {
            HandleStartDefensive(context);

            HandleSilenceBySpecific(context, true);
            HandleSilenceByCategory(context, true);

            HandleSkillEvents(context, m_OnStartActivateSkills);
        }

        private void HandleSkillEvents(StatusEffectContext context, StatusEffecSkill[] statusEffecSkills)
        {
            foreach(StatusEffecSkill statusEffecSkill in statusEffecSkills)
            {
                statusEffecSkill.Execute(context);
            }
        }

        public override void OnEffectEnded(StatusEffectContext context)
        {
            HandleEndDefensive(context);


            HandleSilenceBySpecific(context, false);
            HandleSilenceByCategory(context, false);

            if (!m_RequireFullStackForDoneSkills || context.CurrentStack >= MaxStackCount)
                HandleSkillEvents(context, m_OnDoneActivateSkills);
        }

        public override void OnStackAdded(StatusEffectContext context)
        {
            HandleSkillEvents(context, m_OnStackAddedActivateSkills);
            HandleStackAddedDefensive(context);
        }

        public override void OnStackRemoved(StatusEffectContext context)
        {
            HandleSkillEvents(context, m_OnStackRemovedActivateSkills);
            HandleStackRemovedDefensive(context);
        }
        private void HandleSilenceBySpecific(StatusEffectContext context, bool silence)
        {
            if (context.Infected.HasBind(out SkillController skill))
            {
                if (m_SpecificSkillsToSilence.Length <= 0) return;
                foreach (SkillConfig config in m_SpecificSkillsToSilence)
                {
                    if (skill.HasSkill(config, out Skill activator))
                    {
                        if (silence)
                        {
                            activator.EnterSilence();
                        }
                        else
                        {
                            activator.ExitSilence();
                        }
                    }
                }
            }
            
        }
        private void HandleSilenceByCategory(StatusEffectContext context, bool silence)
        {
            if (context.Infected.HasBind(out SkillController skill))
            {
                if (m_CategoriesSkillToSilence.Length <= 0) return;
                List<Skill> activators = new List<Skill>(skill.GetSkillsByMultiCategory(m_CategoriesSkillToSilence));
                foreach (Skill activator in activators)
                {
                    if (silence)
                    {
                        activator.EnterSilence();
                        Debug.Log($"Silence {activator.SkillConfig.name} by category");
                    }
                    else
                    {
                        activator.ExitSilence();
                    }
                }
            }
        }
        private void HandleStartDefensive(StatusEffectContext context)
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
        private void HandleStackAddedDefensive(StatusEffectContext context)
        {
            if (context.Infected.HasBind(out Damageable damageable))
            {
                var skill = context.AbilityContext.SkillContext.Skill;
                float statuslevel = skill.Progression.Level + CardUnit.GrowthOf(skill);

                damageable.AddRemainingRebornCount(m_RebornCount, true);
                damageable.AddShield(GetFinalValue(m_Base.Shield, m_GrowthPerLevel.Shield, statuslevel), true);
                damageable.AddShieldBasedOnDefendRate(GetFinalValue(m_Base.ShieldBasedDefendRate, m_GrowthPerLevel.ShieldBasedDefendRate, statuslevel), true);
                damageable.AddShieldBasedOnMaxHealth(GetFinalValue(m_Base.ShieldBasedMaxHpRate, m_GrowthPerLevel.ShieldBasedMaxHpRate, statuslevel), true);
                damageable.AddBarrier(GetFinalValue(m_Base.Barrier, m_GrowthPerLevel.Barrier, statuslevel), true);
                damageable.AddDamageReductionRate(GetFinalValue(m_Base.DamageReductionRate, m_GrowthPerLevel.DamageReductionRate, statuslevel));
            }
        }
        private void HandleStackRemovedDefensive(StatusEffectContext context)
        {
            if (context.Infected.HasBind(out Damageable damageable))
            {
                var skill = context.AbilityContext.SkillContext.Skill;
                float statuslevel = skill.Progression.Level + CardUnit.GrowthOf(skill);

                damageable.AddRemainingRebornCount(-m_RebornCount, false);
                damageable.AddShield(-GetFinalValue(m_Base.Shield, m_GrowthPerLevel.Shield, statuslevel), false);
                damageable.AddShieldBasedOnDefendRate(-GetFinalValue(m_Base.ShieldBasedDefendRate, m_GrowthPerLevel.ShieldBasedDefendRate, statuslevel), false);
                damageable.AddShieldBasedOnMaxHealth(-GetFinalValue(m_Base.ShieldBasedMaxHpRate, m_GrowthPerLevel.ShieldBasedMaxHpRate, statuslevel), false);
                damageable.AddBarrier(-GetFinalValue(m_Base.Barrier, m_GrowthPerLevel.Barrier, statuslevel), false);
                damageable.AddDamageReductionRate(-GetFinalValue(m_Base.DamageReductionRate, m_GrowthPerLevel.DamageReductionRate, statuslevel));
            }
        }
        private void HandleEndDefensive(StatusEffectContext context)
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
        }
        private int GetFinalValue(int baseValue, int growthPerLevel, float level)
        {
            return baseValue + Mathf.RoundToInt(growthPerLevel * (level - 1));
        }
        private float GetFinalValue(float baseValue, float growthPerLevel, float level)
        {
            return baseValue + growthPerLevel * (level - 1);
        }
    }
    [System.Serializable]
    public class StatusEffecSkill
    {
        [SerializeField]
        private bool m_OverrideTargetToInfected;
        [SerializeField]
        private SkillConfig m_SkillConfig;

        public SkillConfig SkillConfig => m_SkillConfig;

        public void Execute(StatusEffectContext context)
        {
            if (!context.HasCurrentSource) return;
            Unit infector = context.AbilityContext.SkillContext.ModuleContext.Unit;
            if (infector.HasBind(out SkillController infectorController))
            {
                if (m_OverrideTargetToInfected)
                {
                    Damageable infectedDamageable = GetDamageableInternal(context.Infected);
                    infectorController.ForceActiveOverrideTarget(m_SkillConfig, new List<ITargetable> { infectedDamageable});
                }
                else
                {
                    infectorController.ForceActive(m_SkillConfig);
                }
            }
        }
        private Damageable GetDamageableInternal(Unit source)
        {
            if (source.HasBind(out Damageable damageable))
            {
                return damageable;
            }
            return null;
        }
    }
}

