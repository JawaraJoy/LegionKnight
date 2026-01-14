using UnityEngine;

namespace Rush
{
    [System.Serializable]
    public static partial class StatModifierUltility
    {
        public static StatField GetFinalAddionalStat(StatInfluencerContext context, Unit unitTarget)
        {
            StatField ownerStat = unitTarget.Config.MainStats.GetFinalStat(unitTarget.Progression.Level);
            EffectCalculatorField EffectScore = context.AbilityContext.AbilityDeliver.Config.EffectCalculator;
            StatField result = StatField.Zero;
            EffectField finalScore = EffectField.GetFinalEffect(EffectScore.BaseAmount, EffectScore.ScaleByLevel, context.Influencer.StackCount);

            switch (EffectScore.ScaleBy)
            {
                case ScalingStat.Health:
                    result.SetHealth(ownerStat.Health * finalScore.MultiplierAmount + finalScore.InitialAmount);
                    break;
                case ScalingStat.Attack:
                    result.SetAttack(ownerStat.Attack * finalScore.MultiplierAmount + finalScore.InitialAmount);
                    break;
                case ScalingStat.Defense:
                    result.SetDefense(ownerStat.Defense * finalScore.MultiplierAmount + finalScore.InitialAmount);
                    break;
            }
            return result;
        }
    }
}
