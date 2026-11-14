using System.Globalization;
using UnityEngine;

namespace Rush
{
    public enum BattleCardBonus { None, Lifesteal, PerfectCharge, Accuracy }

    public sealed class BattleCardRunBonuses : MonoBehaviour
    {
        private float m_Lifesteal, m_Accuracy;
        private double m_FractionalHealing;
        private int m_PerfectCharge;
        private StatField m_CardStats = StatField.Zero;
        internal StatField CardStats => m_CardStats;

        internal void AddRepeatGain(CardConfig card, int rank, Unit owner)
        {
            float fraction = rank * .01f;
            float healing = 0f;
            var extra = StatField.Zero;
            foreach (var field in card.SkillConfigs)
            {
                if (field?.SkillConfig == null || field.CardPurpose != CardPurpose.Activation) continue;
                foreach (var ability in field.SkillConfig.AbilitySets)
                {
                    if (ability is StatModifierConfig modifier)
                    {
                        float value = GameplayDescription.ModifierPower(modifier, 1, Mathf.Max(1, modifier.GetStartingStack())).InitialAmount * fraction;
                        switch (modifier.Power.ScaleBy)
                        {
                            case ScalingWithStat.Attack: extra.SetAttack(value); break;
                            case ScalingWithStat.Defense: extra.SetDefense(value); break;
                            case ScalingWithStat.Health: extra.SetHealth(value); break;
                        }
                    }
                    else if (ability is HealAbilityConfig heal) healing += heal.Power.AtLevel(1).InitialAmount * fraction;
                }
            }
            m_CardStats += extra;
            if (healing > 0f && owner.HasBind(out Damageable health))
            {
                health.RefreshDamageableStat(1f, false);
                m_FractionalCardHeal += healing;
                int whole = Mathf.FloorToInt(m_FractionalCardHeal);
                m_FractionalCardHeal -= whole;
                if (whole > 0) health.AddHealth(whole);
            }
        }
        private float m_FractionalCardHeal;
        public float Accuracy => m_Accuracy;
        public int PerfectCharge => m_PerfectCharge;

        public void Apply(BattleCardBonus bonus, float amount)
        {
            switch (bonus)
            {
                case BattleCardBonus.Lifesteal: m_Lifesteal = Mathf.Clamp01(m_Lifesteal + amount); break;
                case BattleCardBonus.PerfectCharge: m_PerfectCharge += Mathf.RoundToInt(amount); break;
                case BattleCardBonus.Accuracy: m_Accuracy += amount; break;
            }
        }

        public int AccrueHealing(int damage, float existingRate)
        {
            m_FractionalHealing += damage * (double)Mathf.Min(m_Lifesteal, Mathf.Clamp01(1f - existingRate));
            int whole = (int)m_FractionalHealing;
            m_FractionalHealing -= whole;
            return whole;
        }

        public void ResetProgression()
        {
            m_CardStats = StatField.Zero;
            m_FractionalCardHeal = 0f;
            m_Lifesteal = m_Accuracy = 0f;
            m_PerfectCharge = 0;
            m_FractionalHealing = 0;
        }

        public static string Describe(BattleCardBonus bonus, float amount)
        {
            var numbers = CultureInfo.InvariantCulture;
            switch (bonus)
            {
                case BattleCardBonus.Lifesteal: return "+" + (amount * 100f).ToString("0.##", numbers) + "% Lifesteal for this battle.";
                case BattleCardBonus.PerfectCharge: return "+" + amount.ToString("0", numbers) + " Charge per Perfect Landing.";
                case BattleCardBonus.Accuracy: return "+" + amount.ToString("0.##", numbers) + " Perfect Landing range per stack.";
                default: return string.Empty;
            }
        }
    }

    public partial class RushPlayer
    {
        [SerializeField] private BattleCardRunBonuses m_CardBonuses;
        public BattleCardRunBonuses CardBonuses => m_CardBonuses;
    }
}

