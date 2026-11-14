using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Rush
{
    public static partial class GameplayDescription
    {
        private const float c_BattleDeltaPrecision = 0.00001f;
        private const string c_WeaponStageCategoryId = "weaponstage";

        private static string BattlePowerChange(AbilityPowerField power, int current, int next, float oldSteps, float newSteps)
        {
            if (power == null || current == next) return string.Empty;
            PowerField previous = power.AtLevel(current, oldSteps);
            PowerField following = power.AtLevel(next, newSteps);
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

        private static string BattleWeaponUpgrade(SkillConfig[] skills, Func<SkillConfig, int> before, Func<SkillConfig, int> after, CardPreview preview)
        {
            var abilities = skills.Select(s => (DamageAbilityConfig)s.AbilitySets[0]).ToArray();
            ScalingWithStat scaling = abilities[0].Power.ScaleBy;
            if (abilities.Any(a => a.Power.ScaleBy != scaling))
                return "Damage " + BattlePowerChange(abilities[0].Power, before(skills[0]), after(skills[0]), preview.Before(skills[0]), preview.After(skills[0])) + " (stage 1) / hit";
            PowerField[] changes = skills.Select((s, i) => abilities[i].Power.AtLevel(after(s), preview.After(s)) -
                abilities[i].Power.AtLevel(before(s), preview.Before(s))).ToArray();
            if (scaling == ScalingWithStat.Attack && skills.All(s => s.Category?.BaseInfo?.Id == c_WeaponStageCategoryId))
            {
                string growth = WeaponStageAttackGrowthInternal(abilities, changes);
                if (growth.Length > 0) return growth;
            }
            var parts = new List<string>();
            if (changes.Any(v => Mathf.Abs(v.InitialAmount) > c_BattleDeltaPrecision))
                parts.Add(BattleChangeRange(changes.Min(v => v.InitialAmount), changes.Max(v => v.InitialAmount)));
            if (scaling != ScalingWithStat.None && changes.Any(v => Mathf.Abs(v.MultiplierAmount) > c_BattleDeltaPrecision))
                parts.Add(BattleChangeRange(changes.Min(v => v.MultiplierAmount) * c_Percent,
                    changes.Max(v => v.MultiplierAmount) * c_Percent) + "% " + StatName(scaling));
            return parts.Count == 0 ? string.Empty : "Damage " + string.Join(" ", parts) + " / hit";
        }

        private static string WeaponStageAttackGrowthInternal(DamageAbilityConfig[] abilities, PowerField[] changes)
        {
            float? commonRate = null;
            for (int i = 0; i < abilities.Length; i++)
            {
                PowerField baseline = abilities[i].Power.AtLevel(1);
                if (baseline.MultiplierAmount <= 0f) return string.Empty;
                float rate = changes[i].MultiplierAmount / baseline.MultiplierAmount;
                if (rate <= 0f || Mathf.Abs(changes[i].InitialAmount - baseline.InitialAmount * rate) > c_BattleDeltaPrecision ||
                    (commonRate.HasValue && Mathf.Abs(rate - commonRate.Value) > c_BattleDeltaPrecision))
                    return string.Empty;
                commonRate = rate;
            }
            return commonRate.HasValue ? Signed(commonRate.Value * c_Percent) + "% Attack" : string.Empty;
        }

        private static string BattleChangeRange(float minimum, float maximum) => Mathf.Approximately(minimum, maximum)
            ? Signed(minimum) : Signed(minimum) + " to " + Signed(maximum);

        private static IEnumerable<string> BattleSkillUpgradeEffects(SkillConfig skill, int current, int next,
            Func<SkillConfig, int> before, Func<SkillConfig, int> after, HashSet<Configuration> path, int depth, CardPreview preview)
        {
            if (skill == null || depth >= c_MaxDepth || path.Contains(skill)) yield break;
            // The card levels the follow-up separately. The mark's hit and stack rules are fixed.
            if (skill.OnHitMarkForDescription != null) yield break;
            var visited = new HashSet<Configuration>(path) { skill };
            foreach (AbilityConfig ability in skill.AbilitySets ?? Array.Empty<AbilityConfig>())
                foreach (string effect in BattleAbilityUpgradeEffects(ability, current, next, before, after, visited, depth + 1, preview, skill))
                    yield return effect;
        }

        private static IEnumerable<string> BattleAbilityUpgradeEffects(AbilityConfig ability, int current, int next,
            Func<SkillConfig, int> before, Func<SkillConfig, int> after, HashSet<Configuration> path, int depth, CardPreview preview, SkillConfig source)
        {
            if (ability == null || depth >= c_MaxDepth || path.Contains(ability)) yield break;
            var visited = new HashSet<Configuration>(path) { ability };
            float oldSteps = preview.Before(source), newSteps = preview.After(source);
            if (ability is AegisConversionConfig aegis)
            {
                float delta = (aegis.LifeStealPerStackAtLevel(2) - aegis.LifeStealPerStackAtLevel(1)) * (next - current + newSteps - oldSteps);
                if (Mathf.Abs(delta) > c_BattleDeltaPrecision)
                    yield return "Life steal " + Signed(delta * c_Percent) + "% / stack";
            }
            else if (ability is StatModifierConfig modifier && modifier.Power != null && modifier.Power.ScaleBy != ScalingWithStat.None)
            {
                int stacks = Mathf.Max(1, modifier.GetStartingStack());
                PowerField delta = ModifierPower(modifier, next, stacks, newSteps) - ModifierPower(modifier, current, stacks, oldSteps);
                string value = Mathf.Abs(delta.InitialAmount) <= c_BattleDeltaPrecision &&
                    Mathf.Abs(delta.MultiplierAmount) > c_BattleDeltaPrecision
                    ? Signed(delta.MultiplierAmount * c_Percent) + "%" : BattlePowerChange(delta, modifier.Power.ScaleBy);
                if (value.Length > 0) yield return StatName(modifier.Power.ScaleBy) + " " + value;
                if (modifier.MaxStackCount > 1 && modifier.HowStackUpdate == HowStackUpdate.Addictive)
                {
                    PowerField extra = ModifierPower(modifier, next, stacks + 1, newSteps) - ModifierPower(modifier, current, stacks + 1, oldSteps) - delta;
                    string growth = BattlePowerChange(extra, modifier.Power.ScaleBy);
                    if (growth.Length > 0 && growth != value) yield return "Extra stack: " + growth;
                }
            }
            else if (ability is AnomalyIncursionConfig incursion && incursion.Entity != null && incursion.Entity.DeathExplosion != null)
            {
                string delta = BattlePowerChange(incursion.Entity.DeathExplosion.Power, current, next, oldSteps, newSteps);
                if (delta.Length > 0) yield return "Death blast " + delta + " damage";
            }
            else if (ability is SummonAbilityConfig summon && summon.UnitToSpawn != null && summon.UnitToSpawn.MainStats != null)
            {
                var growth = summon.UnitToSpawn.MainStats.GetFinalStat(2) - summon.UnitToSpawn.MainStats.GetFinalStat(1);
                StatField oldStats = summon.UnitToSpawn.MainStats.GetFinalStat(current) + growth * oldSteps;
                StatField newStats = summon.UnitToSpawn.MainStats.GetFinalStat(next) + growth * newSteps;
                var parts = new List<string>();
                if (oldStats.Health != newStats.Health) parts.Add("HP " + Signed(newStats.Health - oldStats.Health));
                if (oldStats.Attack != newStats.Attack) parts.Add("ATK " + Signed(newStats.Attack - oldStats.Attack));
                if (oldStats.Defense != newStats.Defense) parts.Add("DEF " + Signed(newStats.Defense - oldStats.Defense));
                if (parts.Count > 0) yield return "Summon " + string.Join(", ", parts);
            }
            else if (ability is DamageAbilityConfig damage)
            {
                string delta = BattlePowerChange(damage.Power, current, next, oldSteps, newSteps);
                if (delta.Length > 0) yield return "Damage " + delta + " / hit";
            }
            else if (ability is HealAbilityConfig heal && heal.HealTickCount > 0)
            {
                string delta = BattlePowerChange(heal.Power, current, next, oldSteps, newSteps);
                if (delta.Length > 0) yield return "Heal " + delta + (heal.HealTickCount > 1 ? " / tick" : string.Empty);
            }
            else if (ability is ChargeAbilityConfig charge && charge.ChargeTickCount > 0)
            {
                string delta = BattlePowerChange(charge.Power, current, next, oldSteps, newSteps);
                if (delta.Length > 0) yield return "Charge " + delta + (charge.ChargeTickCount > 1 ? " / tick" : string.Empty);
            }
            if (ability is StatModifierConfig stat)
            {
                foreach (SkillConfig linked in stat.InfluencedSkillToActivateOnStackFull)
                    foreach (string effect in BattleSkillUpgradeEffects(linked, before(linked), after(linked), before, after, visited, depth + 1, preview))
                        yield return "At max stacks: " + effect;
                foreach (SkillConfig linked in stat.InfluencedSkillToActivateOnStackEmpty)
                    foreach (string effect in BattleSkillUpgradeEffects(linked, before(linked), after(linked), before, after, visited, depth + 1, preview))
                        yield return (stat.HowToRemove == HowStatRemoved.RemoveOnStackZero ? "At 0 stacks: " : "On expiry: ") + effect;
            }
            foreach (StatusEffectConfig status in ability.StatusEffectOnSelf ?? Array.Empty<StatusEffectConfig>())
                foreach (string effect in BattleStatusUpgradeEffects(status, current, next, before, after, visited, depth + 1, preview, source))
                    yield return (ability is DamageAbilityConfig ? "On hit (self): " : string.Empty) + effect;
            foreach (StatusEffectConfig status in ability.StatusEffectOnDelivered ?? Array.Empty<StatusEffectConfig>())
                foreach (string effect in BattleStatusUpgradeEffects(status, current, next, before, after, visited, depth + 1, preview, source))
                    yield return (ability is DamageAbilityConfig ? "On hit: " : string.Empty) + effect;
        }

        private static IEnumerable<string> BattleStatusUpgradeEffects(StatusEffectConfig status, int current, int next,
            Func<SkillConfig, int> before, Func<SkillConfig, int> after, HashSet<Configuration> path, int depth, CardPreview preview, SkillConfig source)
        {
            if (status == null || depth >= c_MaxDepth || path.Contains(status)) yield break;
            var visited = new HashSet<Configuration>(path) { status };
            float oldSteps = preview.Before(source), newSteps = preview.After(source);
            if (status is DefensiveStatusEffectConfig defensive)
            {
                foreach (string delta in BattleDefenseChanges(defensive.BasePower, defensive.GrowthPerLevel, current + oldSteps, next + newSteps,
                    defensive.LevelOnePowerGrowth, defensive.TracksStackPower, false)) yield return delta;
                foreach (SkillConfig linked in defensive.SkillsOnEffectDone)
                    foreach (string effect in BattleSkillUpgradeEffects(linked, before(linked), after(linked), before, after, visited, depth + 1, preview))
                        yield return StatusEndTriggerInternal(status) + effect;
            }
            if (status is AllStatusEffectConfig all)
            {
                foreach (string delta in BattleDefenseChanges(all.BasePower, all.GrowthPerLevel, current + oldSteps, next + newSteps, 0, false, true)) yield return delta;
                foreach (StatusEffecSkill action in all.SkillsOnStart.Concat(all.SkillsOnDone).Concat(all.SkillsOnStackAdded).Concat(all.SkillsOnStackRemoved))
                {
                    if (action == null || action.SkillConfig == null) continue;
                    SkillConfig linked = action.SkillConfig;
                    foreach (string effect in BattleSkillUpgradeEffects(linked, before(linked), after(linked), before, after, visited, depth + 1, preview))
                        yield return effect;
                }
            }
            if (status is AnomalySignConfig sign && sign.Collapse != null)
            {
                string delta = BattlePowerChange(sign.Collapse.Power, current, next, oldSteps, newSteps);
                if (delta.Length > 0) yield return "Collapse " + delta + " damage";
            }
        }

        private static IEnumerable<string> BattleDefenseChanges(DefensiveStatusField baseline, DefensiveStatusField growth,
            float current, float next, float rate, bool linear, bool maxHpShield)
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
            int barriers = Mathf.RoundToInt(growth.Barrier * (next - 1)) - Mathf.RoundToInt(growth.Barrier * (current - 1));
            if (barriers != 0) yield return "Barrier " + Signed(barriers);
            float reduction = (linear ? baseline.DamageReductionRate * rate : growth.DamageReductionRate) * steps;
            if (Mathf.Abs(reduction) > c_BattleDeltaPrecision)
                yield return "Damage reduction " + Signed(reduction * c_Percent) + " pp";
        }
    }
}

