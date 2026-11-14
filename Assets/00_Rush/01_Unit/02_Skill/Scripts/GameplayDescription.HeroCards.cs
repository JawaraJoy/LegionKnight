using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

namespace Rush
{
    public static partial class GameplayDescription
    {
        // {power} uses the first skill/ability. {1.power} selects skill 2, {0.1.power} selects ability 2.
        private static readonly Regex s_CardToken = new Regex(@"\{(?:(?<skill>\d+)\.)?(?:(?<ability>\d+)\.)?(?<key>[A-Za-z]+)\}");
        private const string c_PowerArrow = " → ";

        private readonly struct CardPowerSnapshot
        {
            private readonly SkillConfig[] m_Skills;
            private readonly Func<SkillConfig, int> m_Levels;
            private readonly Func<SkillConfig, float> m_Steps;

            public CardPowerSnapshot(SkillConfig[] skills, Func<SkillConfig, int> levels, Func<SkillConfig, float> steps)
            {
                m_Skills = skills;
                m_Levels = levels;
                m_Steps = steps;
            }

            public string Value(Match token)
            {
                int.TryParse(token.Groups["skill"].Value, out int slot);
                int.TryParse(token.Groups["ability"].Value, out int index);
                if (slot >= m_Skills.Length) return null;
                string key = token.Groups["key"].Value;
                if (key == "stages" || key == "shots" || key == "stageBlasts") return StageValues(key);
                SkillConfig skill = m_Skills[slot];
                var ability = skill.AbilitySets?.ElementAtOrDefault(index);
                return ability == null ? null : EffectValue(key, skill, ability, Mathf.Max(1, m_Levels(skill)), m_Steps(skill));
            }

            private string StageValues(string key)
            {
                var values = new List<string>();
                foreach (var skill in m_Skills)
                {
                    if (!(skill.AbilitySets?.FirstOrDefault() is DamageAbilityConfig damage)) return null;
                    string value = key == "shots" ? damage.SpawningSetup?.FireCount.ToString() :
                        EffectValue(key == "stageBlasts" ? "blast" : "power", skill, damage, Mathf.Max(1, m_Levels(skill)), m_Steps(skill));
                    if (value == null) return null;
                    values.Add(value);
                }
                if (values.Distinct().Count() == 1) return values[0];
                string unit = CommonUnit(values);
                return string.Join("/", values.Select(v => v.Substring(0, v.Length - unit.Length))) + unit;
            }
        }

        private static string CommonUnit(IList<string> values)
        {
            foreach (string suffix in new[] { "% ATK", "% DEF", "% max HP" })
                if (values.All(v => v.EndsWith(suffix, StringComparison.Ordinal))) return suffix;
            return string.Empty;
        }

        private static string EffectValue(string key, SkillConfig skill, AbilityConfig ability, int level, float steps)
        {
            var shooter = ability.DeliverPrefab as Shooter;
            var ammo = shooter?.AmmoConfig;
            var damage = ability as DamageAbilityConfig;
            var modifier = ability as StatModifierConfig;
            var delivered = ability.StatusEffectOnDelivered?.FirstOrDefault();
            var entity = (ability as AnomalyIncursionConfig)?.Entity;
            switch (key)
            {
                case "power": return Power(ability.Power, level, steps);
                case "cost": return Number(skill.Activation.Charge);
                case "range": return Number(ability.Range);
                case "count": return damage?.SpawningSetup?.FireCount.ToString();
                case "kind":
                    if (ability is DirectDamageAbilityConfig) return "direct attack";
                    if (ammo is BeamConfig) return "beam";
                    return ammo == null ? null : ammo.TargetingDistributeMode == TargetingMode.Homing ? "homing projectile" : "projectile";
                case "radius": return ammo is ProjectileConfig projectile ? Number(projectile.ExplodeSetup.ExplosionRadius) : null;
                case "healRate": return damage == null ? null : Percent(damage.HealFromDamageRate);
                case "shieldRate": return damage == null ? null : Percent(damage.ShieldFromDamageRate);
                case "markMax": return (skill.OnHitMarkForDescription ?? delivered)?.MaxStackCount.ToString();
                case "blast": return delivered is AnomalySignConfig sign && sign.Collapse != null ? Power(sign.Collapse.Power, level, steps) : null;
                case "deathBlast": return entity?.DeathExplosion == null ? null : Power(entity.DeathExplosion.Power, level, steps);
                case "deathRadius": return entity?.DeathExplosion == null ? null : Number(entity.DeathExplosion.Range);
                case "entities": return entity?.Count.ToString();
                case "entityHP": return entity?.Unit?.MainStats.GetFinalStat(entity.Unit.Progression.Level).Health.ToString();
                case "max": return modifier?.MaxStackCount.ToString();
                case "starting": return modifier?.GetStartingStack().ToString();
                case "decay": return modifier == null ? null : Number(modifier.StackDurationUpdate);
                case "duration": return modifier == null ? null : Number(modifier.FinalDurationByLevel(level));
                case "statusDecay": return delivered == null ? null : Number(delivered.StackDecayInterval);
                case "steal":
                    if (!(modifier is AegisConversionConfig aegis)) return null;
                    return Percent(aegis.LifeStealPerStackAtLevel(level) + steps * (aegis.LifeStealPerStackAtLevel(2) - aegis.LifeStealPerStackAtLevel(1)));
                case "buff":
                case "extraBuff":
                    if (modifier == null) return null;
                    int first = Mathf.Max(1, modifier.GetStartingStack());
                    PowerField value = ModifierPower(modifier, level, first, steps);
                    if (key == "extraBuff") value = ModifierPower(modifier, level, first + 1, steps) - value;
                    return value.InitialAmount == 0f ? Percent(value.MultiplierAmount) : Power(value, modifier.Power.ScaleBy);
                case "shield":
                    if (!(delivered is DefensiveStatusEffectConfig shield)) return null;
                    float growth = level - 1 + steps;
                    float baseRate = shield.BasePower.ShieldBasedDefendRate;
                    return Percent(shield.TracksStackPower ? baseRate * (1f + shield.LevelOnePowerGrowth * growth) :
                        baseRate + shield.GrowthPerLevel.ShieldBasedDefendRate * growth);
                case "summonCount": return (ability as SummonAbilityConfig)?.SpawnSetup?.BurstCount.ToString();
                case "hp":
                case "atk":
                    if (!(ability is SummonAbilityConfig summon) || summon.UnitToSpawn?.MainStats == null) return null;
                    var stats = summon.UnitToSpawn.MainStats;
                    var final = stats.GetFinalStat(level) + (stats.GetFinalStat(2) - stats.GetFinalStat(1)) * steps;
                    return (key == "hp" ? final.Health : final.Attack).ToString();
                default: return null;
            }
        }

        private static string LearnText(CardConfig card, CardPowerSnapshot snapshot)
        {
            bool valid = true;
            string text = s_CardToken.Replace(card.LearnTemplate, token =>
            {
                string value = snapshot.Value(token);
                if (value == null) valid = false;
                return value ?? string.Empty;
            });
            return valid ? text : null;
        }

        private static string PowerComparison(string current, string next)
        {
            string unit = CommonUnit(new[] { current, next });
            // Keep a percent sign on both sides, put the shared stat name at the end.
            string stat = unit.Length > 0 ? unit.Substring(1) : string.Empty;
            return current.Substring(0, current.Length - stat.Length) + c_PowerArrow + next;
        }

        private static string UpgradeText(CardConfig card, CardPowerSnapshot current, CardPowerSnapshot next, bool onlyChanges)
        {
            var lines = new List<string>();
            foreach (string line in card.PowerTemplate.Split('\n'))
            {
                bool valid = true, changed = false;
                string rendered = s_CardToken.Replace(line, token =>
                {
                    string a = current.Value(token), b = next.Value(token);
                    if (a == null || b == null) { valid = false; return string.Empty; }
                    changed |= a != b;
                    return PowerComparison(a, b);
                });
                if (!valid) return null;
                if (changed || !onlyChanges) lines.Add(rendered);
            }
            return string.Join("\n", lines);
        }

        private static string HeroBattleText(CardConfig card, SkillConfig[] skills, Func<SkillConfig, int> before,
            Func<SkillConfig, int> after, CardPreview preview, bool mixed, bool known)
        {
            var next = new CardPowerSnapshot(skills, after, preview.After);
            if (!known && !mixed) return LearnText(card, next);
            if (skills.All(s => before(s) == after(s))) return "MAX LEVEL";
            var current = new CardPowerSnapshot(skills, before, preview.Before);
            string changes = UpgradeText(card, current, next, mixed);
            if (!mixed || changes == null) return changes;
            // A linked ability can already be known when its trigger is learned (for example Back Stab).
            string learn = LearnText(card, next);
            return learn == null ? null : changes.Length == 0 ? learn : learn + "\n" + changes;
        }

        internal static string HeroCollectionPower(CardConfig card, int rank, int nextRank)
        {
            // Compare the same skill upgrade at the two collection ranks, not two successive skill levels.
            SkillConfig[] skills = card.SkillConfigs.Select(f => f.SkillConfig).ToArray();
            int AtNextLevel(SkillConfig skill) => NextLevel(skill, Mathf.Max(1, CurrentLevel(skill, null)));
            CardPowerSnapshot AtRank(int r) => new CardPowerSnapshot(skills, AtNextLevel,
                s => AtNextLevel(s) > Mathf.Max(1, CurrentLevel(s, null)) ? CardUnit.ExtraGrowthSteps(r) : 0f);
            if (rank == nextRank) return "MAX RANK";
            return UpgradeText(card, AtRank(rank), AtRank(nextRank), false);
        }
    }
}

