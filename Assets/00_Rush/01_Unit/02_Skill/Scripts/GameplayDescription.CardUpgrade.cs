using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Rush
{
    public static partial class GameplayDescription
    {
        private const float c_BattleDeltaPrecision = 0.00001f;

        private static string BattlePowerChange(AbilityPowerField power, int current, int next)
        {
            if (power == null || current == next) return string.Empty;
            PowerField previous = power.AtLevel(current);
            PowerField following = power.AtLevel(next);
            if (power.ScaleBy == ScalingWithStat.None)
            {
                int delta = Mathf.RoundToInt(following.InitialAmount) - Mathf.RoundToInt(previous.InitialAmount);
                return delta == 0 ? string.Empty : Signed(delta);
            }
            return BattlePowerChange(following - previous, power.ScaleBy);
        }

        private static string BattlePowerChange(PowerField delta, ScalingWithStat scaling)
        {
            var parts = new List<string>();
            if (Mathf.Abs(delta.InitialAmount) > c_BattleDeltaPrecision) parts.Add(Signed(delta.InitialAmount));
            if (scaling != ScalingWithStat.None && Mathf.Abs(delta.MultiplierAmount) > c_BattleDeltaPrecision)
                parts.Add(Signed(delta.MultiplierAmount * c_Percent) + "% " + StatName(scaling));
            return string.Join(" ", parts);
        }

        private static string BattleWeaponUpgrade(SkillConfig[] skills, Func<SkillConfig, int> before, Func<SkillConfig, int> after)
        {
            var abilities = skills.Select(s => (DamageAbilityConfig)s.AbilitySets[0]).ToArray();
            ScalingWithStat scaling = abilities[0].Power.ScaleBy;
            if (abilities.Any(a => a.Power.ScaleBy != scaling))
                return BattlePowerChange(abilities[0].Power, before(skills[0]), after(skills[0])) + " (stage 1)";
            PowerField[] changes = skills.Select((s, i) => abilities[i].Power.AtLevel(after(s)) -
                abilities[i].Power.AtLevel(before(s))).ToArray();
            var parts = new List<string>();
            if (changes.Any(v => Mathf.Abs(v.InitialAmount) > c_BattleDeltaPrecision))
                parts.Add(BattleChangeRange(changes.Min(v => v.InitialAmount), changes.Max(v => v.InitialAmount)));
            if (scaling != ScalingWithStat.None && changes.Any(v => Mathf.Abs(v.MultiplierAmount) > c_BattleDeltaPrecision))
                parts.Add(BattleChangeRange(changes.Min(v => v.MultiplierAmount) * c_Percent,
                    changes.Max(v => v.MultiplierAmount) * c_Percent) + "% " + StatName(scaling));
            return string.Join(" ", parts);
        }

        private static string BattleChangeRange(float minimum, float maximum) => Mathf.Approximately(minimum, maximum)
            ? Signed(minimum) : Signed(minimum) + " to " + Signed(maximum);

        private static IEnumerable<string> BattleSkillUpgradeEffects(SkillConfig skill, int current, int next,
            Func<SkillConfig, int> before, Func<SkillConfig, int> after, HashSet<Configuration> path, int depth)
        {
            if (skill == null || depth >= c_MaxDepth || path.Contains(skill)) yield break;
            var visited = new HashSet<Configuration>(path) { skill };
            foreach (AbilityConfig ability in skill.AbilitySets ?? Array.Empty<AbilityConfig>())
                foreach (string effect in BattleAbilityUpgradeEffects(ability, current, next, before, after, visited, depth + 1))
                    yield return effect;
        }

        private static IEnumerable<string> BattleAbilityUpgradeEffects(AbilityConfig ability, int current, int next,
            Func<SkillConfig, int> before, Func<SkillConfig, int> after, HashSet<Configuration> path, int depth)
        {
            if (ability == null || depth >= c_MaxDepth || path.Contains(ability)) yield break;
            var visited = new HashSet<Configuration>(path) { ability };
            if (ability is AegisConversionConfig aegis)
            {
                float delta = aegis.LifeStealPerStackAtLevel(next) - aegis.LifeStealPerStackAtLevel(current);
                if (Mathf.Abs(delta) > c_BattleDeltaPrecision)
                    yield return "Life steal " + Signed(delta * c_Percent) + "% / stack";
            }
            else if (ability is StatModifierConfig modifier && modifier.Power != null && modifier.Power.ScaleBy != ScalingWithStat.None)
            {
                int stacks = Mathf.Max(1, modifier.GetStartingStack());
                PowerField delta = ModifierPower(modifier, next, stacks) - ModifierPower(modifier, current, stacks);
                string value = Mathf.Abs(delta.InitialAmount) <= c_BattleDeltaPrecision &&
                    Mathf.Abs(delta.MultiplierAmount) > c_BattleDeltaPrecision
                    ? Signed(delta.MultiplierAmount * c_Percent) + "%" : BattlePowerChange(delta, modifier.Power.ScaleBy);
                if (value.Length > 0) yield return StatName(modifier.Power.ScaleBy) + " " + value;
                if (modifier.MaxStackCount > 1 && modifier.HowStackUpdate == HowStackUpdate.Addictive)
                {
                    PowerField extra = ModifierPower(modifier, next, stacks + 1) - ModifierPower(modifier, current, stacks + 1) - delta;
                    string growth = BattlePowerChange(extra, modifier.Power.ScaleBy);
                    if (growth.Length > 0 && growth != value) yield return "Extra stack: " + growth;
                }
            }
            else if (ability is AnomalyIncursionConfig incursion && incursion.Entity != null && incursion.Entity.DeathExplosion != null)
            {
                string delta = BattlePowerChange(incursion.Entity.DeathExplosion.Power, current, next);
                if (delta.Length > 0) yield return "Death blast " + delta + " damage";
            }
            else if (ability is SummonAbilityConfig summon && summon.UnitToSpawn != null && summon.UnitToSpawn.MainStats != null)
            {
                StatField oldStats = summon.UnitToSpawn.MainStats.GetFinalStat(current);
                StatField newStats = summon.UnitToSpawn.MainStats.GetFinalStat(next);
                var parts = new List<string>();
                if (oldStats.Health != newStats.Health) parts.Add("HP " + Signed(newStats.Health - oldStats.Health));
                if (oldStats.Attack != newStats.Attack) parts.Add("ATK " + Signed(newStats.Attack - oldStats.Attack));
                if (oldStats.Defense != newStats.Defense) parts.Add("DEF " + Signed(newStats.Defense - oldStats.Defense));
                if (parts.Count > 0) yield return "Summon " + string.Join(", ", parts);
            }
            else if (ability is DamageAbilityConfig damage)
            {
                string delta = BattlePowerChange(damage.Power, current, next);
                if (delta.Length > 0) yield return "Damage " + delta + " / hit";
            }
            else if (ability is HealAbilityConfig heal && heal.HealTickCount > 0)
            {
                string delta = BattlePowerChange(heal.Power, current, next);
                if (delta.Length > 0) yield return "Heal " + delta + (heal.HealTickCount > 1 ? " / tick" : string.Empty);
            }
            else if (ability is ChargeAbilityConfig charge && charge.ChargeTickCount > 0)
            {
                string delta = BattlePowerChange(charge.Power, current, next);
                if (delta.Length > 0) yield return "Charge " + delta + (charge.ChargeTickCount > 1 ? " / tick" : string.Empty);
            }
            if (ability is StatModifierConfig stat)
            {
                foreach (SkillConfig linked in stat.InfluencedSkillToActivateOnStackFull)
                    foreach (string effect in BattleSkillUpgradeEffects(linked, before(linked), after(linked), before, after, visited, depth + 1))
                        yield return "At max stacks: " + effect;
                foreach (SkillConfig linked in stat.InfluencedSkillToActivateOnStackEmpty)
                    foreach (string effect in BattleSkillUpgradeEffects(linked, before(linked), after(linked), before, after, visited, depth + 1))
                        yield return "On expiry: " + effect;
            }
            foreach (StatusEffectConfig status in ability.StatusEffectOnSelf ?? Array.Empty<StatusEffectConfig>())
                foreach (string effect in BattleStatusUpgradeEffects(status, current, next, before, after, visited, depth + 1)) yield return effect;
            foreach (StatusEffectConfig status in ability.StatusEffectOnDelivered ?? Array.Empty<StatusEffectConfig>())
                foreach (string effect in BattleStatusUpgradeEffects(status, current, next, before, after, visited, depth + 1))
                    yield return (ability is DamageAbilityConfig ? "On hit: " : string.Empty) + effect;
        }

        private static IEnumerable<string> BattleStatusUpgradeEffects(StatusEffectConfig status, int current, int next,
            Func<SkillConfig, int> before, Func<SkillConfig, int> after, HashSet<Configuration> path, int depth)
        {
            if (status == null || depth >= c_MaxDepth || path.Contains(status)) yield break;
            var visited = new HashSet<Configuration>(path) { status };
            if (status is DefensiveStatusEffectConfig defensive)
            {
                foreach (string delta in BattleDefenseChanges(defensive.BasePower, defensive.GrowthPerLevel, current, next,
                    defensive.LevelOnePowerGrowth, defensive.TracksStackPower, false)) yield return delta;
                foreach (SkillConfig linked in defensive.SkillsOnEffectDone)
                    foreach (string effect in BattleSkillUpgradeEffects(linked, before(linked), after(linked), before, after, visited, depth + 1))
                        yield return "On expiry: " + effect;
            }
            if (status is AllStatusEffectConfig all)
            {
                foreach (string delta in BattleDefenseChanges(all.BasePower, all.GrowthPerLevel, current, next, 0, false, true)) yield return delta;
                foreach (StatusEffecSkill action in all.SkillsOnStart.Concat(all.SkillsOnDone).Concat(all.SkillsOnStackAdded).Concat(all.SkillsOnStackRemoved))
                {
                    if (action == null || action.SkillConfig == null) continue;
                    SkillConfig linked = action.SkillConfig;
                    foreach (string effect in BattleSkillUpgradeEffects(linked, before(linked), after(linked), before, after, visited, depth + 1))
                        yield return effect;
                }
            }
            if (status is AnomalySignConfig sign && sign.Collapse != null)
            {
                string delta = BattlePowerChange(sign.Collapse.Power, current, next);
                if (delta.Length > 0) yield return "Collapse " + delta + " damage";
            }
        }

        private static IEnumerable<string> BattleDefenseChanges(DefensiveStatusField baseline, DefensiveStatusField growth,
            int current, int next, float rate, bool linear, bool maxHpShield)
        {
            if (baseline == null || growth == null || current == next) yield break;
            float steps = next - current;
            float shield = (linear ? baseline.Shield * rate : growth.Shield) * steps;
            float defense = (linear ? baseline.ShieldBasedDefendRate * rate : growth.ShieldBasedDefendRate) * steps;
            float health = maxHpShield ? growth.ShieldBasedMaxHpRate * steps : 0;
            var parts = new List<string>();
            if (Mathf.Abs(shield) > c_BattleDeltaPrecision) parts.Add(Signed(shield));
            if (Mathf.Abs(defense) > c_BattleDeltaPrecision) parts.Add(Signed(defense * c_Percent) + "% DEF");
            if (Mathf.Abs(health) > c_BattleDeltaPrecision) parts.Add(Signed(health * c_Percent) + "% max HP");
            if (parts.Count > 0) yield return "Shield " + string.Join(" ", parts);
            int barriers = growth.Barrier * (next - current);
            if (barriers != 0) yield return "Barrier " + Signed(barriers);
            float reduction = (linear ? baseline.DamageReductionRate * rate : growth.DamageReductionRate) * steps;
            if (Mathf.Abs(reduction) > c_BattleDeltaPrecision)
                yield return "Damage reduction " + Signed(reduction * c_Percent) + " pp";
        }
    }
}
