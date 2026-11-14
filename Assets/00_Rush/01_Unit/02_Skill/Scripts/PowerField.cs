using UnityEngine;

namespace Rush
{
    [System.Serializable]
    public class PowerField
    {
        [SerializeField]
        private float m_InitialAmount;
        [SerializeField]
        private float m_MultiplierAmount;
        public float InitialAmount => m_InitialAmount;
        public float MultiplierAmount => m_MultiplierAmount;
        public PowerField Zero
        {
            get
            {
                return new PowerField
                {
                    m_InitialAmount = 0,
                    m_MultiplierAmount = 0f,
                };
            }
        }

        public static PowerField operator+(PowerField a, PowerField b)
        {
            return new PowerField
            {
                m_InitialAmount = a.m_InitialAmount + b.m_InitialAmount,
                m_MultiplierAmount = a.m_MultiplierAmount + b.m_MultiplierAmount,
            };
        }
        public static PowerField operator -(PowerField a, PowerField b)
        {
            return new PowerField
            {
                m_InitialAmount = a.m_InitialAmount - b.m_InitialAmount,
                m_MultiplierAmount = a.m_MultiplierAmount - b.m_MultiplierAmount,
            };
        }
        public static PowerField operator*(PowerField a, PowerField b)
        {
            return new PowerField
            {
                m_InitialAmount = (int)(a.m_InitialAmount * b.m_InitialAmount),
                m_MultiplierAmount = a.m_MultiplierAmount * b.m_MultiplierAmount,
            };
        }
        public static PowerField operator*(PowerField a, float scalar)
        {
            return new PowerField
            {
                m_InitialAmount = a.m_InitialAmount * scalar,
                m_MultiplierAmount = a.m_MultiplierAmount * scalar,
            };
        }

        public static PowerField GetFinalPower(PowerField baseEffect, PowerField scaleEffect, int scaleLevel, bool keepFractionalFlatPower = false, PowerField levelOnePrecision = null, float cardSteps = 0f)
            => GetFinalPowerInternal(baseEffect, scaleEffect, scaleLevel, keepFractionalFlatPower, levelOnePrecision, cardSteps);

        private static PowerField GetFinalPowerInternal(PowerField baseEffect, PowerField scaleEffect, int scaleLevel, bool keepFractionalFlatPower, PowerField levelOnePrecision, float cardSteps)
        {
            float growthSteps = scaleLevel - 1 + cardSteps;
            float currentFlatGrowth = scaleEffect.m_InitialAmount * (growthSteps + 1);
            // Level-one power + growth over (level - 1) steps. Subtracting the first
            // quantized contribution preserves existing float/whole-power rounding.
            double flatGrowth = keepFractionalFlatPower
                ? (double)currentFlatGrowth - scaleEffect.m_InitialAmount
                : Mathf.RoundToInt(scaleEffect.m_InitialAmount * (growthSteps + 1)) - Mathf.RoundToInt(scaleEffect.m_InitialAmount);
            double multiplierGrowth = (double)scaleEffect.m_MultiplierAmount * (growthSteps + 1) - scaleEffect.m_MultiplierAmount;
            double flatPrecision = levelOnePrecision == null ? 0d : levelOnePrecision.m_InitialAmount;
            double multiplierPrecision = levelOnePrecision == null ? 0d : levelOnePrecision.m_MultiplierAmount;
            return new PowerField
            {
                m_InitialAmount = (float)((double)baseEffect.m_InitialAmount + flatPrecision + flatGrowth),
                m_MultiplierAmount = (float)((double)baseEffect.m_MultiplierAmount + multiplierPrecision + multiplierGrowth),
            };
        }
        // Existing modifiers scale their stack base separately from skill-level growth.
        public static PowerField GetFinalStackPower(PowerField levelOnePower, PowerField scaleEffect, int level, int stackLevel, PowerField levelOnePrecision = null, float cardSteps = 0f)
            => GetFinalStackPowerInternal(levelOnePower, scaleEffect, level, stackLevel, levelOnePrecision, cardSteps);

        private static PowerField GetFinalStackPowerInternal(PowerField levelOnePower, PowerField scaleEffect, int level, int stackLevel, PowerField levelOnePrecision, float cardSteps)
        {
            double flatPrecision = levelOnePrecision == null ? 0d : levelOnePrecision.m_InitialAmount;
            double multiplierPrecision = levelOnePrecision == null ? 0d : levelOnePrecision.m_MultiplierAmount;
            float firstFlatGrowth = Mathf.RoundToInt(scaleEffect.m_InitialAmount);
            float stackFlat = (float)((double)levelOnePower.m_InitialAmount + flatPrecision - firstFlatGrowth) * stackLevel;
            float stackMultiplier = (float)((double)levelOnePower.m_MultiplierAmount + multiplierPrecision - scaleEffect.m_MultiplierAmount) * stackLevel;
            double levelOneFlat = (double)stackFlat + firstFlatGrowth;
            double levelOneMultiplier = (double)stackMultiplier + scaleEffect.m_MultiplierAmount;
            var stackPower = new PowerField
            {
                m_InitialAmount = (float)levelOneFlat,
                m_MultiplierAmount = (float)levelOneMultiplier,
            };
            var stackPrecision = new PowerField
            {
                m_InitialAmount = (float)(levelOneFlat - stackPower.m_InitialAmount),
                m_MultiplierAmount = (float)(levelOneMultiplier - stackPower.m_MultiplierAmount),
            };
            return GetFinalPowerInternal(stackPower, scaleEffect, level, false, stackPrecision, cardSteps);
        }

        public static int GetFinalPowerByStatScaling(ScalingWithStat scaling, StatController unitStat)
        {
            Unit unit = unitStat.ModuleContext.Unit;
            int ownerLevel = unit.Progression.Level;
            StatField statValue = unit.BaseStats;
            int finalInitialAmount = unitStat.GetFinalStat(statValue).Attack;

            switch (scaling)
            {
                case ScalingWithStat.None:
                    break;
                case ScalingWithStat.Health:
                    finalInitialAmount = unitStat.GetFinalStat(statValue).Health;
                    break;
                case ScalingWithStat.Attack:
                    finalInitialAmount = unitStat.GetFinalStat(statValue).Attack;
                    break;
                case ScalingWithStat.Defense:
                    finalInitialAmount = unitStat.GetFinalStat(statValue).Defense;
                    break;
            }
            return finalInitialAmount;
        }
    }
    [System.Serializable]
    public partial class AbilityPowerField
    {
        [SerializeField]
        private ScalingWithStat m_ScaleBy = ScalingWithStat.None;
        [SerializeField, Tooltip("Power at skill level 1, before stat scaling.")]
        private PowerField m_BaseAmount;
        [SerializeField, Tooltip("Power added for each level after level 1.")]
        private PowerField m_ScaleByLevel;
        [SerializeField, Tooltip("Keep fractional flat power until damage, healing or charge is delivered.")]
        private bool m_KeepFractionalFlatPower;
        // Retains only the sub-float remainder lost while rebasing existing data.
        [SerializeField, HideInInspector]
        private PowerField m_LevelOnePrecision = new PowerField();
        public PowerField AtLevel(int level, float cardSteps = 0f) => PowerField.GetFinalPower(m_BaseAmount, m_ScaleByLevel, level, m_KeepFractionalFlatPower, m_LevelOnePrecision, cardSteps);
        public ScalingWithStat ScaleBy => m_ScaleBy;
        public PowerField BaseAmount => m_BaseAmount;
        public PowerField ScaleByLevel => m_ScaleByLevel;
        public PowerField LevelOnePrecision => m_LevelOnePrecision;
    }

    public enum PowerPurpose
    {
        Damage,
        Heal,
        Shield,
        StatModifier,
    }
}

