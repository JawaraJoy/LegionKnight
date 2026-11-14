using UnityEngine;

namespace Rush
{
    [System.Serializable]
    public static partial class StatModifierUtility
    {
        public static StatField GetFinalAddionalStat(StatModifierContext context, Unit unitTarget)
        {
            AbilityPowerField effectScore = context.AbilityContext.AbilityDeliver.AbilityConfig.Power;
            if (effectScore.ScaleBy == ScalingWithStat.None) return StatField.Zero;
            StatField ownerStat = unitTarget.BaseStats;

            StatField result = StatField.Zero;

            StatModifier modifier = context.Influencer;
            int level = context.AbilityContext.SkillContext.Skill.Progression.Level;
            float cardSteps = CardUnit.GrowthOf(context.AbilityContext.SkillContext.Skill);
            int stacks = Mathf.Clamp(modifier.StackCount, 0, modifier.Config.MaxStackCount);
            AbilityPowerField additional = modifier.Config.AdditionalStackPower;
            PowerField finalScore;
            if (additional != null && additional.ScaleBy == effectScore.ScaleBy)
            {
                if (stacks == 0) return StatField.Zero;
                finalScore = effectScore.AtLevel(level, cardSteps) + additional.AtLevel(level, cardSteps) * (stacks - 1);
            }
            else
            {
                int offset = modifier.Config.IncludeStartingStackPower ? 0 : 1;
                int stackLevel = Mathf.Max(0, modifier.StackCount - offset);
                finalScore = PowerField.GetFinalStackPower(effectScore.BaseAmount, effectScore.ScaleByLevel, level, stackLevel, effectScore.LevelOnePrecision, cardSteps);
            }

            switch (effectScore.ScaleBy)
            {
                case ScalingWithStat.Health:
                    result.SetHealth(ownerStat.Health * finalScore.MultiplierAmount + finalScore.InitialAmount);
                    break;

                case ScalingWithStat.Attack:
                    result.SetAttack(ownerStat.Attack * finalScore.MultiplierAmount + finalScore.InitialAmount);
                    break;

                case ScalingWithStat.Defense:
                    result.SetDefense(ownerStat.Defense * finalScore.MultiplierAmount + finalScore.InitialAmount);
                    break;

                case ScalingWithStat.CriticalChance:
                    result.SetCriticalChance(ownerStat.CriticalChance * finalScore.MultiplierAmount + finalScore.InitialAmount);
                    break;

                case ScalingWithStat.CriticalDamageFlat:
                    result.SetCriticalDamageFlat(ownerStat.CriticalDamageFlat * finalScore.MultiplierAmount + finalScore.InitialAmount);
                    break;

                case ScalingWithStat.CriticalDamageRate:
                    result.SetCriticalDamageRate(ownerStat.CriticalDamageRate * finalScore.MultiplierAmount + finalScore.InitialAmount);
                    break;

                default:
                    Debug.LogWarning($"Unhandled ScalingStat: {effectScore.ScaleBy}");
                    break;
            }

            return result;
        }
    }
}

