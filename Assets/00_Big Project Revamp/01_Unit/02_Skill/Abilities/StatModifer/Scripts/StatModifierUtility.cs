using UnityEngine;

namespace Rush
{
    [System.Serializable]
    public static partial class StatModifierUtility
    {
        public static StatField GetFinalAddionalStat(StatModifierContext context, Unit unitTarget)
        {
            StatField ownerStat = unitTarget.Config.MainStats.GetFinalStat(unitTarget.Progression.Level);
            AbilityPowerField EffectScore = context.AbilityContext.AbilityDeliver.AbilityConfig.Power;
            StatField result = StatField.Zero;
            PowerField finalScore = PowerField.GetFinalPower(EffectScore.BaseAmount, EffectScore.ScaleByLevel, context.Influencer.StackCount);

            switch (EffectScore.ScaleBy)
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
                    Debug.LogWarning($"Unhandled ScalingStat: {EffectScore.ScaleBy}");
                    break;
            }

            return result;
        }
    }
}
