using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using UnityEngine;

namespace Rush
{
    public static partial class GameplayDescription
    {
        private const int c_MaxBattleEffects = 2;
        private const string c_BattleNumberFormat = "0.##";

        private enum BattleCardMode { Normal, Upgrade, Runtime }

        private readonly struct CardPreview
        {
            private readonly SkillController m_Controller;
            private readonly SkillConfig[] m_Upgraded;
            private readonly float m_Steps;
            internal CardPreview(SkillController controller, SkillConfig[] skills, int rank)
            {
                m_Controller = controller;
                m_Upgraded = skills;
                m_Steps = CardUnit.ExtraGrowthSteps(rank);
            }
            internal float Before(SkillConfig skill) => CurrentGrowthSteps(skill, m_Controller);
            internal float After(SkillConfig skill) => Before(skill) + (Array.IndexOf(m_Upgraded, skill) >= 0 ? m_Steps : 0f);
        }

        public static string NormalCard(CardConfig config, int maxEffects = c_MaxBattleEffects) =>
            BuildBattleCard(config, null, null, BattleCardMode.Normal, maxEffects);

        public static string UpgradeCard(CardConfig config, SkillController controller = null, int maxEffects = c_MaxBattleEffects) =>
            BuildBattleCard(config, controller, null, BattleCardMode.Upgrade, maxEffects);

        public static string BattleCard(CardConfig config, SkillController controller = null,
            int maxEffects = c_MaxBattleEffects, PlatformHandler platformHandler = null) =>
            BuildBattleCard(config, controller, platformHandler, BattleCardMode.Runtime, maxEffects);

        private static string BuildBattleCard(CardConfig config, SkillController controller,
            PlatformHandler platformHandler, BattleCardMode mode, int maxEffects)
            => BuildBattleCard(config, controller, platformHandler, mode, maxEffects, false, out _);

        internal static string CollectionUpgrade(CardConfig card, int rank) =>
            BuildBattleCard(card, null, null, BattleCardMode.Upgrade, 2, true, out _, rank);

        public static string BattleCardDetails(CardConfig config, SkillController controller,
            PlatformHandler platformHandler, out string progression) =>
            BuildBattleCard(config, controller, platformHandler, BattleCardMode.Runtime, int.MaxValue, true, out progression);

        private static string BuildBattleCard(CardConfig config, SkillController controller,
            PlatformHandler platformHandler, BattleCardMode mode, int maxEffects, bool separateProgression, out string progression, int? previewRank = null)
        {
            progression = string.Empty;
            if (config == null) return string.Empty;
            if (!string.IsNullOrEmpty(config.BonusDescription)) return config.BonusDescription;
            CardSkillField[] fields = (config.SkillConfigs ?? Array.Empty<CardSkillField>())
                .Where(f => f != null && f.SkillConfig != null).ToArray();
            SkillConfig[] upgrades = fields.Where(f => f.CardPurpose == CardPurpose.SkillUp)
                .Select(f => f.SkillConfig).Distinct().ToArray();
            Func<SkillConfig, int> before = skill => mode == BattleCardMode.Normal ? 0 :
                mode == BattleCardMode.Runtime && controller == null ? 0 :
                mode == BattleCardMode.Upgrade ? Mathf.Max(1, CurrentLevel(skill, controller)) :
                CurrentLevel(skill, controller);
            Func<SkillConfig, int> after = skill => upgrades.Contains(skill)
                ? NextLevel(skill, before(skill)) : Mathf.Max(1, before(skill));
            Func<SkillConfig, int> oldLevels = skill => Mathf.Max(1, before(skill));
            PlatformConfig[] platforms = (config.PlatformToAdds ?? Array.Empty<PlatformConfig>())
                .Where(p => p != null).ToArray();
            bool ownedPlatforms = platforms.Length > 0 && (mode == BattleCardMode.Upgrade ||
                platformHandler != null && platforms.All(platformHandler.PreparedPlatformConfigs.Contains));
            bool known = upgrades.Length > 0 && upgrades.All(s => before(s) > 0);
            bool mixed = upgrades.Any(s => before(s) > 0) && upgrades.Any(s => before(s) <= 0);
            bool weaponStages = !mixed && upgrades.Length >= 3 && upgrades.All(s => s.AbilitySets != null &&
                s.AbilitySets.Length == 1 && s.AbilitySets[0] is DamageAbilityConfig);
            var lines = new List<string>();
            string level = mixed ? "Upgrade + unlock" : BattleLevel(upgrades, before, after);
            if (platforms.Length > 0)
                progression = (ownedPlatforms ? "Upgrade platform" : "Add platform") +
                    (level.Length > 0 ? " | " + level : string.Empty);
            else if (level.Length > 0)
                progression = (weaponStages ? "All stages: " : string.Empty) + level;


            int rank = previewRank ?? config.CollectionRank;
            var preview = new CardPreview(controller, upgrades.Where(s => before(s) > 0 && after(s) > before(s)).ToArray(), rank);
            var effects = new List<string>();
            if (weaponStages)
            {
                if (known)
                {
                    string change = BattleWeaponUpgrade(upgrades, before, after, preview);
                    if (change.Length > 0) effects.Add(change);
                }
                else effects.Add("Damage " + BattleWeaponPower(upgrades, after) + " / hit");
            }
            else
            {
                foreach (CardSkillField field in CardDisplayFieldsInternal(fields))
                {
                    SkillConfig skill = field.SkillConfig;
                    bool upgrading = field.CardPurpose == CardPurpose.SkillUp && before(skill) > 0;
                    string trigger = !upgrading && field.CardPurpose == CardPurpose.SkillUp &&
                        platforms.Length == 0 && skill.Activation != null ? Trigger(skill.Activation).TrimEnd('.') : string.Empty;
                    string[] descriptions = (upgrading
                        ? BattleSkillUpgradeEffects(skill, before(skill), after(skill), oldLevels, after,
                            new HashSet<Configuration>(), 0, preview)
                        : BattleSkillEffects(skill, after(skill), after, new HashSet<Configuration>(), 0)).ToArray();
                    for (int i = 0; i < descriptions.Length && (separateProgression || !mixed || i == 0); i++)
                    {
                        string prefix = mixed ? (upgrading ? "Upgrade: " : "Unlock: ") :
                            i == 0 && trigger.Length > 0 ? trigger + ": " : string.Empty;
                        effects.Add(prefix + descriptions[i]);
                    }
                }
            }
            if (platforms.Length > 0 && !ownedPlatforms)
            {
                var normal = new HashSet<SkillConfig>(platforms.SelectMany(p => LandingSkills(p, false)));
                var perfect = new HashSet<SkillConfig>(platforms.SelectMany(p => LandingSkills(p, true)));
                if (fields.Length == 0)
                {
                    foreach (SkillConfig skill in normal.Concat(perfect).Where(s => s != null).Distinct())
                    {
                        string trigger = perfect.Contains(skill) && !normal.Contains(skill) ? "Perfect: " : "Landing: ";
                        effects.AddRange(BattleSkillEffects(skill, after(skill), after, new HashSet<Configuration>(), 0)
                            .Select(effect => trigger + effect));
                    }
                }
                else if (effects.Count > 0 && CardDisplayFieldsInternal(fields).Where(field => field.CardPurpose == CardPurpose.SkillUp).All(field => perfect.Contains(field.SkillConfig)))
                    effects[0] = "Perfect: " + effects[0];
            }
            foreach (CardSkillCategoryModification modification in config.SkillCategoryModifications ?? Array.Empty<CardSkillCategoryModification>())
            {
                if (modification == null) continue;
                foreach (StatusEffectConfig status in modification.OnSelf ?? Array.Empty<StatusEffectConfig>())
                    effects.AddRange(BattleStatusEffects(status, 1, after, new HashSet<Configuration>(), 0));
                foreach (StatusEffectConfig status in modification.OnTarget ?? Array.Empty<StatusEffectConfig>())
                    effects.AddRange(BattleStatusEffects(status, 1, after, new HashSet<Configuration>(), 0).Select(effect => "On hit: " + effect));
            }
            if (ownedPlatforms) effects.Add(!config.HasSkillUpgrade && rank > 0 ? $"Platform spawn weight +{(1f + rank * .01f):0.##}" : "Platform appears more often");
            if (config.AetherEssenceReward != 0) effects.Add(Signed(config.AetherEssenceReward) + " Aether Essence");
            if (config.CurrencyReward != null && config.CurrencyReward.ItemConfig != null && config.CurrencyReward.Amount > 0)
                effects.Add("+" + Number(config.CurrencyReward.Amount) + " " + config.CurrencyReward.ItemConfig.BaseInfo.Name);
            if (effects.Count == 0 && upgrades.Length > 0)
                effects.Add(upgrades.All(s => before(s) == after(s)) ? "No further skill upgrades" : "No stat increase this level");
            if (mode == BattleCardMode.Runtime && !config.IsBattlePoolOnly)
            {
                bool repeatGain = Application.isPlaying && config.IsRepeatGain(RushGameManager.Instance?.RogueLikeManager);
                if (repeatGain && !config.HasSkillUpgrade && !config.HasPlatformUpgrade)
                {
                    progression = "Upgrade";
                    if (rank > 0) effects.Insert(0, $"+{rank}% of base gain on this pick");
                }
                if (rank > 0 || progression.Length > 0)
                    progression = (progression.Length > 0 ? progression + "  ·  " : "") + $"Card +{rank}";
            }
            if (!separateProgression && progression.Length > 0) lines.Add(progression);
            lines.AddRange(effects.Where(effect => !string.IsNullOrWhiteSpace(effect)).Distinct().Take(Mathf.Max(1, maxEffects)));
            return string.Join("\n", lines);
        }

        private static string BattleLevel(SkillConfig[] skills, Func<SkillConfig, int> before, Func<SkillConfig, int> after)
        {
            if (skills.Length == 0) return string.Empty;
            int[] current = skills.Select(before).Distinct().ToArray();
            int[] next = skills.Select(after).Distinct().ToArray();
            if (current.Length == 1 && next.Length == 1)
            {
                if (current[0] <= 0) return "Learn at Skill Lv. " + next[0];
                if (current[0] == next[0]) return "MAX LEVEL";
                return "Upgrade  ·  Skill Lv. " + current[0] + " → " + next[0];
            }
            return skills.All(s => before(s) == after(s)) ? "MAX LEVEL" : "Upgrade skills";
        }

        private static string BattleWeaponPower(SkillConfig[] skills, Func<SkillConfig, int> levels)
        {
            var abilities = skills.Select(s => (DamageAbilityConfig)s.AbilitySets[0]).ToArray();
            ScalingWithStat scaling = abilities[0].Power.ScaleBy;
            if (abilities.Any(a => a.Power.ScaleBy != scaling))
                return Power(abilities[0].Power, levels(skills[0])) + " (stage 1)";
            PowerField[] values = skills.Select((s, i) => abilities[i].Power.AtLevel(levels(s))).ToArray();
            var parts = new List<string>();
            if (values.Any(v => v.InitialAmount != 0))
                parts.Add(BattleRange(values.Min(v => v.InitialAmount), values.Max(v => v.InitialAmount)));
            if (scaling != ScalingWithStat.None && values.Any(v => v.MultiplierAmount != 0))
                parts.Add(BattleRange(values.Min(v => v.MultiplierAmount) * c_Percent,
                    values.Max(v => v.MultiplierAmount) * c_Percent) + "% " + StatName(scaling));
            return parts.Count == 0 ? "0" : string.Join(" + ", parts);
        }

        private static string BattleRange(float minimum, float maximum) => Mathf.Approximately(minimum, maximum)
            ? minimum.ToString(c_BattleNumberFormat, CultureInfo.InvariantCulture)
            : minimum.ToString(c_BattleNumberFormat, CultureInfo.InvariantCulture) + "-" + maximum.ToString(c_BattleNumberFormat, CultureInfo.InvariantCulture);

        private static IEnumerable<string> BattleSkillEffects(SkillConfig skill, int level, Func<SkillConfig, int> levels,
            HashSet<Configuration> path, int depth)
        {
            if (skill == null || depth >= c_MaxDepth || path.Contains(skill)) yield break;
            if (skill.OnHitMarkForDescription != null)
            {
                yield return (skill.BaseInfo.Description + " " + DescribeMarkTrigger(skill.OnHitMarkForDescription)).Trim();
                yield break;
            }
            var visited = new HashSet<Configuration>(path) { skill };
            foreach (AbilityConfig ability in skill.AbilitySets ?? Array.Empty<AbilityConfig>())
                foreach (string effect in BattleAbilityEffects(ability, level, levels, visited, depth + 1))
                    yield return effect;
        }

        private static IEnumerable<string> BattleAbilityEffects(AbilityConfig ability, int level, Func<SkillConfig, int> levels,
            HashSet<Configuration> path, int depth)
        {
            if (ability == null || depth >= c_MaxDepth || path.Contains(ability)) yield break;
            var visited = new HashSet<Configuration>(path) { ability };
            if (ability is AegisConversionConfig aegis)
                yield return Percent(aegis.LifeStealPerStackAtLevel(level)) + " life steal / stack";
            else if (ability is StatModifierConfig modifier)
            {
                string stat = BattleModifier(modifier, level);
                if (stat.Length > 0) yield return stat.TrimEnd('.');
            }
            else if (ability is AnomalyIncursionConfig incursion && incursion.Entity != null)
            {
                yield return "Summon " + incursion.Entity.Count + " entities";
                if (incursion.Entity.DeathExplosion != null)
                    yield return "Death blast: " + Power(incursion.Entity.DeathExplosion.Power, level) + " damage";
            }
            else if (ability is SummonAbilityConfig summon)
            {
                int count = summon.SpawnSetup == null ? 1 : Mathf.Max(1, summon.SpawnSetup.BurstCount);
                yield return "Summon " + count + " " + (summon.UnitToSpawn == null ? "unit" : summon.UnitToSpawn.BaseInfo.Name);
            }
            else if (ability is DamageAbilityConfig damage)
                yield return "Damage " + Power(damage.Power, level) + " / hit";
            else if (ability is HealAbilityConfig heal && heal.HealTickCount > 0 && HasPower(heal.Power, level))
                yield return "Heal " + Power(heal.Power, level) + (heal.HealTickCount > 1 ? " / tick" : string.Empty);
            else if (ability is ChargeAbilityConfig charge && charge.ChargeTickCount > 0 && HasPower(charge.Power, level))
                yield return "Restore " + Power(charge.Power, level) + " charge" + (charge.ChargeTickCount > 1 ? " / tick" : string.Empty);

            if (ability is DamageAbilityConfig drainDamage && drainDamage.HealFromDamageRate > 0f)
                yield return "Heal for " + Percent(drainDamage.HealFromDamageRate) + " of HP damage dealt";
            if (ability is DamageAbilityConfig shieldDamage && shieldDamage.ShieldFromDamageRate > 0f)
                yield return "Restore " + Percent(shieldDamage.ShieldFromDamageRate) + " of HP damage dealt as Shield";
            if (ability is StatModifierConfig counter && counter.HowStackUpdate == HowStackUpdate.Subtractive &&
                counter.InfluencedSkillToActivateOnStackEmpty.Length > 0)
                yield return "Attacks consume " + counter.GetStartingStack() + " charges; at 0, trigger " +
                    string.Join(" and ", counter.InfluencedSkillToActivateOnStackEmpty.Select(skill => skill.BaseInfo.Name));
            if (ability is StatModifierConfig statModifier)
            {
                foreach (SkillConfig linked in statModifier.InfluencedSkillToActivateOnStackFull)
                    foreach (string effect in BattleSkillEffects(linked, Mathf.Max(1, levels(linked)), levels, visited, depth + 1))
                        yield return "At max stacks: " + effect;
                foreach (SkillConfig linked in statModifier.InfluencedSkillToActivateOnStackEmpty)
                    foreach (string effect in BattleSkillEffects(linked, Mathf.Max(1, levels(linked)), levels, visited, depth + 1))
                        yield return (statModifier.HowToRemove == HowStatRemoved.RemoveOnStackZero ? "At 0 stacks: " : "When effect ends: ") + effect;
            }
            foreach (StatusEffectConfig status in ability.StatusEffectOnSelf ?? Array.Empty<StatusEffectConfig>())
                foreach (string effect in BattleStatusEffects(status, level, levels, visited, depth + 1))
                    yield return (ability is DamageAbilityConfig ? "On hit (self): " : string.Empty) + effect;
            foreach (StatusEffectConfig status in ability.StatusEffectOnDelivered ?? Array.Empty<StatusEffectConfig>())
                foreach (string effect in BattleStatusEffects(status, level, levels, visited, depth + 1))
                    yield return (ability is DamageAbilityConfig ? "On hit: " : string.Empty) + effect;
        }

        private static IEnumerable<string> BattleStatusEffects(StatusEffectConfig status, int level, Func<SkillConfig, int> levels,
            HashSet<Configuration> path, int depth)
        {
            if (status == null || depth >= c_MaxDepth || path.Contains(status)) yield break;
            var visited = new HashSet<Configuration>(path) { status };
            bool described = false;
            if (status is DefensiveStatusEffectConfig defensive)
            {
                string defense = Defense(defensive.BasePower, defensive.GrowthPerLevel, level,
                    defensive.LevelOnePowerGrowth, defensive.TracksStackPower, false);
                if (defense.Length > 0) { described = true; yield return defense.TrimEnd('.'); }
                if (defensive.RebornCount != 0) { described = true; yield return Signed(defensive.RebornCount) + " extra life"; }
                if (defensive.Immortality) { described = true; yield return "Temporary immortality"; }
                if (defensive.Invisibility) { described = true; yield return "Temporary invisibility"; }
                foreach (SkillConfig linked in defensive.SkillsOnEffectDone)
                    foreach (string effect in BattleSkillEffects(linked, Mathf.Max(1, levels(linked)), levels, visited, depth + 1))
                    { described = true; yield return StatusEndTriggerInternal(status) + effect; }
            }
            if (status is AllStatusEffectConfig all)
            {
                string defense = Defense(all.BasePower, all.GrowthPerLevel, level, 0f, false, true);
                if (defense.Length > 0) { described = true; yield return defense.TrimEnd('.'); }
                if (all.RebornCount != 0) { described = true; yield return Signed(all.RebornCount) + " extra life"; }
                if (all.Immortality) { described = true; yield return "Temporary immortality"; }
                if (all.Invisibility) { described = true; yield return "Temporary invisibility"; }
                foreach (StatusEffecSkill action in all.SkillsOnStart)
                {
                    if (action?.SkillConfig == null) continue;
                    foreach (string effect in BattleSkillEffects(action.SkillConfig, Mathf.Max(1, levels(action.SkillConfig)), levels, visited, depth + 1))
                    { described = true; yield return effect; }
                }
                foreach (StatusEffecSkill action in all.SkillsOnDone)
                {
                    if (action == null || action.SkillConfig == null) continue;
                    foreach (string effect in BattleSkillEffects(action.SkillConfig, Mathf.Max(1, levels(action.SkillConfig)), levels, visited, depth + 1))
                    { described = true; yield return (all.RemoveRule == StatusRemoveRule.OnStackReachMax ||
                        all.RemoveRule == StatusRemoveRule.OnStackExceedMax ? "Stack trigger: " : "On expiry: ") + effect; }
                }
            }
            if (status is AnomalySignConfig sign && sign.Collapse != null)
            {
                described = true;
                yield return sign.MaxStackCount + " marks: " + Power(sign.Collapse.Power, level) + " blast";
            }
            if (!described) yield return "Apply " + status.BaseInfo.Name;
        }

        private static string StatusEndTriggerInternal(StatusEffectConfig status)
        {
            if (status.RemoveRule == StatusRemoveRule.OnStackExceedMax)
                return "At " + status.MaxStackCount + " stacks, next application: ";
            if (status.RemoveRule == StatusRemoveRule.OnStackReachMax)
                return "At " + status.MaxStackCount + " stacks: ";
            return "On expiry: ";
        }

        private static IEnumerable<CardSkillField> CardDisplayFieldsInternal(CardSkillField[] fields)
        {
            var followUps = new HashSet<SkillConfig>();
            foreach (CardSkillField field in fields)
            {
                if (field?.SkillConfig == null) continue;
                foreach (AbilityConfig ability in field.SkillConfig.AbilitySets ?? Array.Empty<AbilityConfig>())
                {
                    if (ability is StatModifierConfig modifier)
                        followUps.UnionWith(modifier.InfluencedSkillToActivateOnStackEmpty);
                    foreach (StatusEffectConfig status in (ability.StatusEffectOnSelf ?? Array.Empty<StatusEffectConfig>())
                        .Concat(ability.StatusEffectOnDelivered ?? Array.Empty<StatusEffectConfig>()))
                    {
                        if (status is DefensiveStatusEffectConfig defensive)
                            followUps.UnionWith(defensive.SkillsOnEffectDone);
                        if (status is AllStatusEffectConfig all)
                            followUps.UnionWith(all.SkillsOnStart.Where(action => action?.SkillConfig != null).Select(action => action.SkillConfig));
                    }
                }
            }
            return fields.Where(field => field?.SkillConfig != null && !followUps.Contains(field.SkillConfig));
        }

        private static string BattleModifier(StatModifierConfig modifier, int level)
        {
            if (modifier.Power == null || modifier.Power.ScaleBy == ScalingWithStat.None) return string.Empty;
            int starting = Mathf.Max(1, modifier.GetStartingStack());
            PowerField first = ModifierPower(modifier, level, starting);
            if (first.InitialAmount == 0f && first.MultiplierAmount == 0f) return string.Empty;
            string value = first.InitialAmount == 0f ? Percent(first.MultiplierAmount) : Power(first, modifier.Power.ScaleBy);
            string result = StatName(modifier.Power.ScaleBy) + " +" + value;
            if (modifier.MaxStackCount > 1 && modifier.HowStackUpdate == HowStackUpdate.Addictive)
            {
                PowerField extra = ModifierPower(modifier, level, starting + 1) - first;
                if (!Mathf.Approximately(extra.InitialAmount, first.InitialAmount) ||
                    !Mathf.Approximately(extra.MultiplierAmount, first.MultiplierAmount))
                {
                    string growth = extra.InitialAmount == 0f ? Percent(extra.MultiplierAmount) : Power(extra, modifier.Power.ScaleBy);
                    result += "; +" + growth + " / extra stack";
                }
            }
            if (modifier.HowToRemove == HowStatRemoved.RemoveOnDurationEnd)
                result += " for " + Number(modifier.FinalDurationByLevel(level)) + "s; max " + modifier.MaxStackCount + " stacks";
            return result;
        }
    }
}

