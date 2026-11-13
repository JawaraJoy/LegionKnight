using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

namespace Rush
{
    /// <summary>Read-only descriptions built from the same configuration and level evaluators as combat.</summary>
    public static partial class GameplayDescription
    {
        private const int c_MaxDepth = 8;
        private const float c_Percent = 100f;
        private const string c_NumberFormat = "0.###";
        private const string c_NumericPattern = @"[-+]?\d+(?:\.\d+)?";

        public sealed class SkillDetails
        {
            public string Description { get; internal set; }
            public string DamageLabel { get; internal set; }
            public string Damage { get; internal set; }
            public string Charge { get; internal set; }
            public string Range { get; internal set; }
        }

        public static SkillDetails ForSkillPanel(SkillConfig config, int level, SkillController controller = null)
        {
            var details = new SkillDetails();
            details.Description = BuildSkill(config, level, controller, details);
            return details;
        }

        public static string Skill(SkillConfig config, int level, SkillController controller = null) =>
            BuildSkill(config, level, controller, null);

        private static string BuildSkill(SkillConfig config, int level, SkillController controller, SkillDetails details)
        {
            if (config == null) return string.Empty;
            Func<SkillConfig, int> levels = skill => CurrentLevel(skill, controller);
            int displayLevel = ClampLevel(config, level);
            string description = DescribeSkill(config, displayLevel, levels, new HashSet<Configuration>(), 0, true, details);
            AllStatusEffectConfig mark = config.OnHitMarkForDescription;
            if (mark == null) return description;

            var lines = new List<string> { description };
            foreach (StatusEffecSkill action in mark.SkillsOnDone)
            {
                SkillConfig strike = action?.SkillConfig;
                if (strike == null) continue;
                int strikeLevel = controller == null ? ClampLevel(strike, displayLevel) : Mathf.Max(1, levels(strike));
                lines.AddRange(BattleSkillEffects(strike, strikeLevel, levels, new HashSet<Configuration> { config }, 1));
            }
            return string.Join("\n", lines.Distinct());
        }

        public static string Card(CardConfig config, SkillController controller = null, bool preview = false)
        {
            if (config == null) return string.Empty;
            CardSkillField[] fields = config.SkillConfigs ?? Array.Empty<CardSkillField>();
            var upgrades = new HashSet<SkillConfig>(fields.Where(f => f != null && f.SkillConfig != null &&
                f.CardPurpose == CardPurpose.SkillUp).Select(f => f.SkillConfig));
            Func<SkillConfig, int> before = skill => CurrentLevel(skill, controller);
            Func<SkillConfig, int> after = skill => upgrades.Contains(skill)
                ? NextLevel(skill, before(skill)) : Mathf.Max(1, before(skill));
            var lines = new List<string>();
            var stages = new Dictionary<string, int>();
            bool platformCard = config.PlatformToAdds != null && config.PlatformToAdds.Length > 0;
            var normalSkills = new HashSet<SkillConfig>((config.PlatformToAdds ?? Array.Empty<PlatformConfig>())
                .Where(p => p != null).SelectMany(p => LandingSkills(p, false)));
            var perfectSkills = new HashSet<SkillConfig>((config.PlatformToAdds ?? Array.Empty<PlatformConfig>())
                .Where(p => p != null).SelectMany(p => LandingSkills(p, true)));
            string weaponStages = DescribeWeaponStages(fields, before, after, preview);
            if (weaponStages.Length > 0) lines.Add(weaponStages);
            foreach (CardSkillField field in fields)
            {
                if (weaponStages.Length > 0) break;
                if (field == null || field.SkillConfig == null) continue;
                SkillConfig skill = field.SkillConfig;
                int current = before(skill);
                int next = preview && field.CardPurpose == CardPurpose.SkillUp ? after(skill) : Mathf.Max(1, current);
                string effects = DescribeSkill(skill, next, preview ? after : before,
                    new HashSet<Configuration>(), 0, !platformCard && field.CardPurpose == CardPurpose.SkillUp);
                string label = skill.BaseInfo.Name;
                if (fields.Count(f => f != null && f.SkillConfig != null && f.SkillConfig.BaseInfo.Name == label) > 1)
                {
                    stages.TryGetValue(label, out int stage);
                    stages[label] = stage + 1;
                    label += " stage " + (stage + 1);
                }
                if (preview && field.CardPurpose == CardPurpose.SkillUp)
                {
                    if (current > 0)
                    {
                        string oldEffects = DescribeSkill(skill, current, before, new HashSet<Configuration>(), 0,
                            !platformCard);
                        label += next == current ? " (MAX)" : " Lv " + current + " -> " + next;
                        effects = Preview(oldEffects, effects);
                    }
                    else label += " (unlock Lv " + next + ")";
                }
                if (normalSkills.Contains(skill) && perfectSkills.Contains(skill)) label = "On landing: " + label;
                else if (perfectSkills.Contains(skill)) label = "Perfect landing: " + label;
                else if (normalSkills.Contains(skill)) label = "Normal landing: " + label;
                lines.Add(label + ": " + effects);
            }
            foreach (PlatformConfig platform in config.PlatformToAdds ?? Array.Empty<PlatformConfig>())
            {
                if (platform == null) continue;
                lines.Add("Adds " + platform.BaseInfo.Name + "; further copies increase its spawn weight.");
                var normal = LandingSkills(platform, false).Distinct();
                var perfect = LandingSkills(platform, true).Distinct();
                foreach (SkillConfig skill in normal.Where(s => s != null && !upgrades.Contains(s)))
                    lines.Add("Normal landing: " + DescribeSkill(skill, Mathf.Max(1, before(skill)), before,
                        new HashSet<Configuration>(), 0, false));
                foreach (SkillConfig skill in perfect.Where(s => s != null && !upgrades.Contains(s)))
                    lines.Add("Perfect landing: " + DescribeSkill(skill, Mathf.Max(1, before(skill)), before,
                        new HashSet<Configuration>(), 0, false));
            }
            foreach (CardSkillCategoryModification modification in config.SkillCategoryModifications ?? Array.Empty<CardSkillCategoryModification>())
            {
                if (modification == null) continue;
                foreach (StatusEffectConfig status in modification.OnSelf ?? Array.Empty<StatusEffectConfig>())
                    lines.Add("On self: " + DescribeStatus(status, 1, before, new HashSet<Configuration>(), 0));
                foreach (StatusEffectConfig status in modification.OnTarget ?? Array.Empty<StatusEffectConfig>())
                    lines.Add("On hit: " + DescribeStatus(status, 1, before, new HashSet<Configuration>(), 0));
            }
            if (config.AetherEssenceReward != 0) lines.Add("Aether Essence " + Signed(config.AetherEssenceReward) + ".");
            if (config.CurrencyReward != null && config.CurrencyReward.ItemConfig != null && config.CurrencyReward.Amount > 0)
                lines.Add("Gain " + Number(config.CurrencyReward.Amount) + " " + config.CurrencyReward.ItemConfig.BaseInfo.Name + ".");
            return string.Join("\n", lines.Where(line => !string.IsNullOrWhiteSpace(line)).Distinct());
        }

        private static string DescribeWeaponStages(CardSkillField[] fields, Func<SkillConfig, int> before,
            Func<SkillConfig, int> after, bool preview)
        {
            if (fields.Length < 3 || fields.Any(f => f == null || f.CardPurpose != CardPurpose.SkillUp ||
                f.SkillConfig == null || f.SkillConfig.AbilitySets == null || f.SkillConfig.AbilitySets.Length != 1 ||
                !(f.SkillConfig.AbilitySets[0] is DamageAbilityConfig))) return string.Empty;
            var stages = new List<string>();
            var extraEffects = new List<string>();
            for (int i = 0; i < fields.Length; i++)
            {
                SkillConfig skill = fields[i].SkillConfig;
                var damage = (DamageAbilityConfig)skill.AbilitySets[0];
                int current = before(skill);
                int next = preview ? after(skill) : Mathf.Max(1, current);
                string nextPower = Power(damage.Power, next);
                string power = preview && current > 0 ? Preview(Power(damage.Power, current), nextPower) : nextPower;
                int shots = damage.SpawningSetup == null ? 1 : damage.SpawningSetup.FireCount;
                string level = preview ? current <= 0 ? " (unlock)" : next == current ? " (MAX)" : " Lv " + current + " -> " + next : string.Empty;
                string damageType = damage.DamageType == DamageType.TrueDamage ? " true damage" :
                    damage.DamageType == DamageType.FatalDamage ? " fatal damage" : " damage";
                stages.Add("Stage " + (i + 1) + level + ": " + shots + " hit(s), " + power + damageType + " per hit.");
                string extra = DescribeDamageExtras(damage, next, preview ? after : before);
                if (preview && current > 0) extra = Preview(DescribeDamageExtras(damage, current, before), extra);
                if (extra.Length > 0) extraEffects.Add(extra);
            }
            return string.Join("\n", stages.Concat(extraEffects.Distinct()));
        }
        private static string DescribeDamageExtras(DamageAbilityConfig damage, int level, Func<SkillConfig, int> levels)
        {
            var parts = new List<string>();
            var path = new HashSet<Configuration> { damage };
            foreach (StatusEffectConfig status in damage.StatusEffectOnSelf ?? Array.Empty<StatusEffectConfig>())
                parts.Add("Self: " + DescribeStatus(status, level, levels, path, 1));
            foreach (StatusEffectConfig status in damage.StatusEffectOnDelivered ?? Array.Empty<StatusEffectConfig>())
                parts.Add("On hit: " + DescribeStatus(status, level, levels, path, 1));
            if (damage.DeliverPrefab is Shooter shooter && shooter.AmmoConfig is ProjectileConfig projectile &&
                projectile.ExplodeSetup != null && projectile.ExplodeSetup.ExplodeOnHit)
                parts.Add("Explosion radius " + Number(projectile.ExplodeSetup.ExplosionRadius) + "m.");
            if (damage is DirectDamageAbilityConfig direct && direct.ExplodeSetup != null && direct.ExplodeSetup.ExplodeOnHit)
                parts.Add("Explosion radius " + Number(direct.ExplodeSetup.ExplosionRadius) + "m.");
            return string.Join(" ", parts);
        }
        private static int CurrentLevel(SkillConfig skill, SkillController controller)
        {
            if (skill == null) return 0;
            if (controller != null) return controller.HasSkill(skill, out Skill runtime) ? runtime.Progression.Level : 0;
            return skill.LevelSet == null ? 1 : skill.LevelSet.Level;
        }
        private static int ClampLevel(SkillConfig skill, int level) =>
            Mathf.Clamp(level, 1, skill.LevelSet == null ? int.MaxValue : Mathf.Max(1, skill.LevelSet.MaxLevel));
        private static int NextLevel(SkillConfig skill, int current) => ClampLevel(skill, current > 0 ? current + 1 : 1);
        private static IEnumerable<SkillConfig> LandingSkills(PlatformConfig config, bool perfect)
        {
            PlatformSkillField[] sides = { config.LeftSkillField, config.RightSkillField };
            return sides.Where(side => side != null).SelectMany(side =>
                (perfect ? side.OnPerfectTouchSkill : side.OnNormalTouchSkill) ?? Array.Empty<SkillConfig>());
        }

        private static string DescribeSkill(SkillConfig skill, int level, Func<SkillConfig, int> levels,
            HashSet<Configuration> path, int depth, bool activation, SkillDetails details = null)
        {
            if (skill == null) return string.Empty;
            if (depth >= c_MaxDepth || path.Contains(skill)) return skill.BaseInfo.Name;
            AllStatusEffectConfig mark = skill.OnHitMarkForDescription;
            if (mark != null)
            {
                string perHit = mark.StackPerApply == 1 ? "1 stack" : mark.StackPerApply + " stacks";
                return (skill.BaseInfo.Description + " Each hit adds " + perHit + ". " + DescribeMarkTrigger(mark)).Trim();
            }
            var visited = new HashSet<Configuration>(path) { skill };
            var parts = new List<string>();
            if (activation && skill.Activation != null)
            {
                string trigger = Trigger(skill.Activation);
                if (details != null && skill.Activation.ForceActiveState == ForceActiveState.OnChargeFull && skill.Activation.Charge > 0)
                    details.Charge = Number(skill.Activation.Charge);
                else if (trigger.Length > 0) parts.Add(trigger);
                if (skill.Casting != null && skill.Casting.CastDuration > 0)
                    parts.Add("Cast " + Number(skill.Casting.CastDuration) + "s.");
            }
            foreach (AbilityConfig ability in skill.AbilitySets ?? Array.Empty<AbilityConfig>())
            {
                string effect = DescribeAbility(ability, level, levels, visited, depth + 1,
                    skill.AbilitySets.Length == 1 ? details : null);
                if (effect.Length > 0) parts.Add(effect);
            }
            return string.Join(" ", parts.Distinct());
        }

        private static string DescribeMarkTrigger(AllStatusEffectConfig mark)
        {
            string condition;
            switch (mark.RemoveRule)
            {
                case StatusRemoveRule.OnStackExceedMax:
                    condition = "At " + mark.MaxStackCount + " stacks on an enemy, the next mark consumes them all";
                    break;
                case StatusRemoveRule.OnStackReachMax:
                    condition = "Reaching " + mark.MaxStackCount + " stacks consumes all marks";
                    break;
                default:
                    condition = "The mark ends";
                    break;
            }
            var followUps = mark.SkillsOnDone.Where(action => action?.SkillConfig != null)
                .Select(action => action.SkillConfig.BaseInfo.Name).Distinct();
            string names = string.Join(" and ", followUps);
            return names.Length == 0 ? condition + "." : condition + " and automatically triggers " + names + ".";
        }

        private static string Trigger(ActivationTriggerField activation)
        {
            switch (activation.ForceActiveState)
            {
                case ForceActiveState.OnChargeFull: return activation.Charge > 0 ? "Cost " + Number(activation.Charge) + " charge." : string.Empty;
                case ForceActiveState.OnCooldownDone: return "Cooldown " + Number(activation.Cooldown) + "s.";
                case ForceActiveState.OnHit: return "On hit.";
                case ForceActiveState.OnGetHit: return "When hit.";
                case ForceActiveState.OnDeclareAttack: return "On attack.";
                case ForceActiveState.OnDamageDealed: return "On dealing damage.";
                case ForceActiveState.OnDamageTaken: return "On taking damage.";
                case ForceActiveState.OnShieldDamageTaken: return "On shield damage.";
                case ForceActiveState.OnBarrierDamageTaken: return "On barrier damage.";
                case ForceActiveState.OnHealing: return "On healing.";
                case ForceActiveState.OnHealed: return "When healed.";
                case ForceActiveState.OnNormalTouch: return "On normal landing.";
                case ForceActiveState.OnPerfectTouch: return "On perfect landing.";
                case ForceActiveState.OnSummonDeath: return "When a summon dies.";
                case ForceActiveState.OnDeath: return "On death.";
                case ForceActiveState.OnKill: return "On kill.";
                default: return string.Empty;
            }
        }

        private static string DescribeAbility(AbilityConfig ability, int level, Func<SkillConfig, int> levels,
            HashSet<Configuration> path, int depth, SkillDetails details = null)
        {
            if (ability == null) return string.Empty;
            if (depth >= c_MaxDepth || path.Contains(ability)) return ability.BaseInfo.Name;
            var visited = new HashSet<Configuration>(path) { ability };
            var parts = new List<string>();
            if (ability is AegisConversionConfig aegis)
                parts.Add(Percent(aegis.LifeStealPerStackAtLevel(level)) + " life steal per stack.");
            else if (ability is StatModifierConfig modifier)
                parts.Add(DescribeModifier(modifier, level));
            else if (ability is AnomalyIncursionConfig incursion)
            {
                AnomalyEntityField entity = incursion.Entity;
                if (entity != null)
                {
                    parts.Add("Summon " + entity.Count + " entities" + EntityStats(entity.Unit, entity.Unit == null ? 1 : entity.Unit.Progression.Level) +
                        "; " + entity.Barriers + " barriers each.");
                    if (entity.DeathExplosion != null)
                        parts.Add("On entity death: " + DescribeAbility(entity.DeathExplosion, level, levels, visited, depth + 1));
                }
            }
            else if (ability is SummonAbilityConfig summon)
            {
                int count = summon.SpawnSetup == null ? 1 : Mathf.Max(1, summon.SpawnSetup.BurstCount);
                parts.Add("Summon " + count + " " + (summon.UnitToSpawn == null ? "unit" : summon.UnitToSpawn.BaseInfo.Name) +
                    EntityStats(summon.UnitToSpawn, level) + ".");
                if (summon.SpawnDuration != null && summon.SpawnDuration.HasDuration)
                    parts.Add("Lasts " + Number(summon.SpawnDuration.Duration) + "s.");
            }
            else if (ability is DamageAbilityConfig damage)
            {
                string type = damage.DamageType == DamageType.TrueDamage ? " true damage" :
                    damage.DamageType == DamageType.FatalDamage ? " fatal damage" : " damage";
                string power = Power(damage.Power, level);
                if (damage.DamageBasedTargetMaxHP != 0) power += " + " + Percent(damage.DamageBasedTargetMaxHP) + " target max HP";
                string count = !(damage is AnomalyPulseConfig) && damage.SpawningSetup != null && damage.SpawningSetup.FireCount > 1
                    ? (damage.UseAllTargetsInRange ? "Up to " : string.Empty) + Number(damage.SpawningSetup.FireCount) +
                        (damage is ShooterAbilityConfig ? " projectiles; " : " hits; ") : string.Empty;
                if (details == null)
                    parts.Add(count + power + type + " per hit.");
                else
                {
                    details.Damage = power;
                    details.DamageLabel = char.ToUpperInvariant(type[1]) + type.Substring(2) + " per hit";
                    if (count.Length > 0) parts.Add(count.TrimEnd(' ', ';') + ".");
                }
                if (!(damage is AnomalyPulseConfig) && damage.UseAllTargetsInRange)
                {
                    if (details == null) parts.Add("Targets within " + Number(damage.Range) + "m.");
                    else details.Range = Number(damage.Range) + " m";
                }
                else if (!(damage is AnomalyPulseConfig) && damage.MaxTargetCount > 1)
                    parts.Add("Up to " + damage.MaxTargetCount + " targets within " + Number(damage.Range) + "m.");
                if (damage is AnomalyPulseConfig pulse)
                {
                    parts.Add("Damage radius " + Number(pulse.Range) + "m.");
                    if (pulse.Pull.Radius > 0f && pulse.Pull.Distance > 0f)
                        parts.Add("Pulls targets within " + Number(pulse.Pull.Radius) + "m by " + Number(pulse.Pull.Distance) + "m over " + Number(pulse.Pull.Duration) + "s.");
                }
                if (damage is DirectDamageAbilityConfig direct && direct.ExplodeSetup != null && direct.ExplodeSetup.ExplodeOnHit)
                    parts.Add("Explosion radius " + Number(direct.ExplodeSetup.ExplosionRadius) + "m.");
                if (damage.DeliverPrefab is Shooter shooter && shooter.AmmoConfig != null)
                {
                    AmmoConfig ammo = shooter.AmmoConfig;
                    if (ammo.TargetingDistributeMode == TargetingMode.Homing) parts.Add("Homing projectile.");
                    if (ammo is ProjectileConfig projectile && projectile.ExplodeSetup != null && projectile.ExplodeSetup.ExplodeOnHit)
                        parts.Add("Explosion radius " + Number(projectile.ExplodeSetup.ExplosionRadius) + "m.");
                }
            }
            else if (ability is HealAbilityConfig heal)
            {
                if (heal.HealTickCount > 0 && HasPower(heal.Power, level))
                    parts.Add("Heal " + Power(heal.Power, level) + TickText(heal.HealTickCount, heal.HealTickInterval) + ".");
            }
            else if (ability is ChargeAbilityConfig charge)
            {
                if (charge.ChargeTickCount > 0 && HasPower(charge.Power, level))
                    parts.Add("Restore " + Power(charge.Power, level) + " " + (charge.SkillCategoryToCharge == null ? "charge" : charge.SkillCategoryToCharge.BaseInfo.Name + " charge") +
                        TickText(charge.ChargeTickCount, charge.ChargeTickInterval) + ".");
            }
            else parts.Add(ability.BaseInfo.Name + ".");

            if (ability is StatModifierConfig stat)
            {
                if (stat.MaxStackCount > 1) parts.Add("Max " + stat.MaxStackCount + " stacks.");
                if (stat.HowStackUpdate == HowStackUpdate.Subtractive)
                    parts.Add("Starts with " + stat.GetStartingStack() + " stacks; consumes " + stat.UpdatePerStackCount + " per trigger.");
                if (stat.HowToRemove == HowStatRemoved.RemoveOnDurationEnd)
                    parts.Add("Duration " + Number(stat.FinalDurationByLevel(level)) + "s.");
                if (stat.UseStackDuration)
                    parts.Add("Lose " + stat.UpdatePerStackCount + " stack(s) every " + Number(Mathf.Clamp(stat.StackDurationUpdate, 0f, stat.FinalDurationByLevel(level))) + "s.");
                foreach (SkillConfig linked in stat.InfluencedSkillToActivateOnStackFull)
                    parts.Add("At max stacks: " + DescribeSkill(linked, Mathf.Max(1, levels(linked)), levels, visited, depth + 1, false));
                foreach (SkillConfig linked in stat.InfluencedSkillToActivateOnStackEmpty)
                    parts.Add("When effect ends: " + DescribeSkill(linked, Mathf.Max(1, levels(linked)), levels, visited, depth + 1, false));
            }
            foreach (StatusEffectConfig status in ability.StatusEffectOnSelf ?? Array.Empty<StatusEffectConfig>())
                parts.Add("Self: " + DescribeStatus(status, level, levels, visited, depth + 1));
            foreach (StatusEffectConfig status in ability.StatusEffectOnDelivered ?? Array.Empty<StatusEffectConfig>())
                parts.Add((ability is DamageAbilityConfig ? "On hit: " : "Apply: ") + DescribeStatus(status, level, levels, visited, depth + 1));
            return string.Join(" ", parts.Where(p => !string.IsNullOrWhiteSpace(p)).Distinct());
        }

        private static string DescribeModifier(StatModifierConfig config, int level)
        {
            if (config.Power == null || config.Power.ScaleBy == ScalingWithStat.None) return string.Empty;
            int starting = Mathf.Max(1, config.GetStartingStack());
            PowerField first = ModifierPower(config, level, starting);
            if (first.InitialAmount == 0f && first.MultiplierAmount == 0f) return string.Empty;
            string text = StatName(config.Power.ScaleBy) + " +" + Power(first, config.Power.ScaleBy) + ".";
            if (config.MaxStackCount > 1 && config.HowStackUpdate == HowStackUpdate.Addictive)
            {
                PowerField second = ModifierPower(config, level, starting + 1);
                string extra = Power(second - first, config.Power.ScaleBy);
                if (extra != "0") text += " Each extra stack: +" + extra + ".";
            }
            return text;
        }
        private static PowerField ModifierPower(StatModifierConfig config, int level, int stacks)
        {
            AbilityPowerField power = config.Power;
            if (config.AdditionalStackPower != null && config.AdditionalStackPower.ScaleBy == power.ScaleBy)
                return power.AtLevel(level) + config.AdditionalStackPower.AtLevel(level) * (stacks - 1);
            int offset = config.IncludeStartingStackPower ? 0 : 1;
            return PowerField.GetFinalStackPower(power.BaseAmount, power.ScaleByLevel, level,
                Mathf.Max(0, stacks - offset), power.LevelOnePrecision);
        }

        private static string DescribeStatus(StatusEffectConfig status, int level, Func<SkillConfig, int> levels,
            HashSet<Configuration> path, int depth)
        {
            if (status == null) return string.Empty;
            if (depth >= c_MaxDepth || path.Contains(status)) return status.BaseInfo.Name;
            var visited = new HashSet<Configuration>(path) { status };
            var parts = new List<string> { status.BaseInfo.Name + "." };
            if (status.MaxStackCount > 1) parts.Add("Max " + status.MaxStackCount + " stacks; +" + status.StackPerApply + " per application.");
            if (status.UseMainDuration && status.RemoveRule == StatusRemoveRule.OnMainDurationEnd)
                parts.Add("Duration " + Number(status.MainDuration) + "s.");
            if (status.RemoveRule == StatusRemoveRule.OnStackReachMax) parts.Add("Ends at max stacks.");
            if (status.RemoveRule == StatusRemoveRule.OnStackExceedMax) parts.Add("Ends when the stack limit is exceeded.");
            if (status.UseStackDecay) parts.Add("Lose " + status.StackDecayAmountPerInterval + " stack(s) every " + Number(status.StackDecayInterval) + "s.");
            if (status is DefensiveStatusEffectConfig defensive)
            {
                parts.Add(Defense(defensive.BasePower, defensive.GrowthPerLevel, level,
                    defensive.LevelOnePowerGrowth, defensive.TracksStackPower, false));
                if (defensive.RebornCount != 0) parts.Add("Extra lives " + Signed(defensive.RebornCount) + ".");
                if (defensive.Immortality) parts.Add("Immortal while active.");
                if (defensive.Invisibility) parts.Add("Invisible while active.");
                foreach (SkillConfig linked in defensive.SkillsOnEffectDone)
                    parts.Add("When effect ends: " + DescribeSkill(linked, Mathf.Max(1, levels(linked)), levels, visited, depth + 1, false));
            }
            if (status is AllStatusEffectConfig all)
            {
                parts.Add(Defense(all.BasePower, all.GrowthPerLevel, level, 0f, false, true));
                if (all.RebornCount != 0) parts.Add("Extra lives " + Signed(all.RebornCount) + ".");
                if (all.Immortality) parts.Add("Immortal while active.");
                if (all.Invisibility) parts.Add("Invisible while active.");
                foreach (StatusEffecSkill action in all.SkillsOnStart)
                    parts.Add("On application: " + DescribeLinked(action, levels, visited, depth));
                foreach (StatusEffecSkill action in all.SkillsOnDone)
                    parts.Add("When effect ends: " + DescribeLinked(action, levels, visited, depth));
                foreach (StatusEffecSkill action in all.SkillsOnStackAdded)
                    parts.Add("On stack gained: " + DescribeLinked(action, levels, visited, depth));
                foreach (StatusEffecSkill action in all.SkillsOnStackRemoved)
                    parts.Add("On stack lost: " + DescribeLinked(action, levels, visited, depth));
            }
            if (status is AnomalySignConfig sign && sign.Collapse != null)
                parts.Add("At " + sign.MaxStackCount + " stacks: " + DescribeAbility(sign.Collapse, level, levels, visited, depth + 1));
            if (status is SilenceEffectConfig) parts.Add("Silences configured skills while active.");
            return string.Join(" ", parts.Where(p => !string.IsNullOrWhiteSpace(p)).Distinct());
        }
        private static string DescribeLinked(StatusEffecSkill action, Func<SkillConfig, int> levels, HashSet<Configuration> path, int depth)
        {
            if (action == null || action.SkillConfig == null) return string.Empty;
            return DescribeSkill(action.SkillConfig, Mathf.Max(1, levels(action.SkillConfig)), levels, path, depth + 1, false);
        }
        private static string Defense(DefensiveStatusField baseline, DefensiveStatusField growth, int level, float rate, bool linear, bool maxHpShield)
        {
            if (baseline == null || growth == null) return string.Empty;
            float steps = Mathf.Max(0, level - 1);
            float factor = linear ? 1f + rate * steps : 1f;
            float shield = linear ? baseline.Shield * factor : baseline.Shield + growth.Shield * steps;
            float defense = linear ? baseline.ShieldBasedDefendRate * factor : baseline.ShieldBasedDefendRate + growth.ShieldBasedDefendRate * steps;
            float hp = maxHpShield ? baseline.ShieldBasedMaxHpRate + growth.ShieldBasedMaxHpRate * steps : 0f;
            float reduction = linear ? baseline.DamageReductionRate * factor : baseline.DamageReductionRate + growth.DamageReductionRate * steps;
            int barriers = baseline.Barrier + growth.Barrier * (level - 1);
            var parts = new List<string>();
            var shieldParts = new List<string>();
            if (shield != 0) shieldParts.Add(Number(shield));
            if (defense != 0) shieldParts.Add(Percent(defense) + " DEF");
            if (hp != 0) shieldParts.Add(Percent(hp) + " max HP");
            if (shieldParts.Count > 0) parts.Add("Shield " + string.Join(" + ", shieldParts) + ".");
            if (barriers != 0) parts.Add("Barrier " + Signed(barriers) + ".");
            if (reduction != 0) parts.Add("Damage reduction " + Signed(reduction * c_Percent) + " percentage points.");
            return string.Join(" ", parts);
        }
        private static string EntityStats(UnitConfig config, int level)
        {
            if (config == null || config.MainStats == null) return string.Empty;
            StatField stats = config.MainStats.GetFinalStat(level);
            return " (" + stats.Health + " HP, " + stats.Attack + " ATK, " + stats.Defense + " DEF)";
        }
        private static string TickText(int count, float interval) => count > 1
            ? " per tick, " + count + " ticks every " + Number(interval) + "s" : string.Empty;
        private static bool HasPower(AbilityPowerField field, int level) => field != null &&
            (field.AtLevel(level).InitialAmount != 0f || field.AtLevel(level).MultiplierAmount != 0f);
        private static string Power(AbilityPowerField field, int level) => field == null ? "0" :
            field.ScaleBy == ScalingWithStat.None ? Number(Mathf.RoundToInt(field.AtLevel(level).InitialAmount)) : Power(field.AtLevel(level), field.ScaleBy);
        private static string Power(PowerField power, ScalingWithStat scaling)
        {
            var parts = new List<string>();
            if (power.InitialAmount != 0) parts.Add(Number(power.InitialAmount));
            if (power.MultiplierAmount != 0 && scaling != ScalingWithStat.None)
                parts.Add(Percent(power.MultiplierAmount) + " " + StatName(scaling));
            return parts.Count == 0 ? "0" : string.Join(" + ", parts);
        }
        private static string StatName(ScalingWithStat stat)
        {
            switch (stat)
            {
                case ScalingWithStat.Health: return "max HP";
                case ScalingWithStat.Attack: return "ATK";
                case ScalingWithStat.Defense: return "DEF";
                case ScalingWithStat.CriticalChance: return "crit chance";
                case ScalingWithStat.CriticalDamageFlat: return "crit damage";
                case ScalingWithStat.CriticalDamageRate: return "crit damage rate";
                default: return string.Empty;
            }
        }
        private static string Number(float value) => value.ToString(c_NumberFormat, CultureInfo.InvariantCulture);
        private static string Preview(string before, string after)
        {
            if (before == after) return after;
            if (Regex.Replace(before, c_NumericPattern, "#") != Regex.Replace(after, c_NumericPattern, "#")) return after;
            MatchCollection next = Regex.Matches(after, c_NumericPattern);
            int index = 0;
            return Regex.Replace(before, c_NumericPattern, match =>
            {
                string value = next[index++].Value;
                return match.Value == value ? value : match.Value + " -> " + value;
            });
        }
        private static string Percent(float value) => Number(value * c_Percent) + "%";
        private static string Signed(float value) => (value >= 0 ? "+" : string.Empty) + Number(value);
    }
}
