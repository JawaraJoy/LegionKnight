using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Rush
{
    [DisallowMultipleComponent]
    public sealed class BossAbilityVisuals : MonoBehaviour, IUnitExtension, IReseter
    {
        [Serializable]
        private class ThemeField
        {
            [SerializeField] private BossUnitConfig m_Boss;
            [SerializeField] private Color m_Color;
            public BossUnitConfig Boss => m_Boss;
            public Color Color => m_Color;
        }

        [SerializeField] private BossEnergyVfx m_EffectPrefab;
        [SerializeField] private StatusEffectConfig m_DevotionConfig;
        [SerializeField] private ThemeField[] m_Themes;
        private const int c_MaxEffects = 24;
        private const float c_CastSize = 4f;
        private const float c_LaunchSize = 2.5f;
        private const float c_ImpactSize = 2.3f;
        private const float c_ImpactSeconds = .45f;
        private const float c_SummonSize = 4f;
        private const float c_SummonSeconds = .8f;
        private const float c_DefenseSize = 6f;
        private const float c_DefenseSeconds = .9f;
        private const float c_ProjectileSize = .85f;
        private const int c_MaxOwnerDepth = 16;
        private readonly List<BossEnergyVfx> m_Effects = new();
        private readonly Dictionary<Skill, UnityAction<IAbilityContext>> m_Activations = new();
        private readonly Dictionary<Skill, UnityAction> m_CastStarts = new();
        private readonly Dictionary<Skill, UnityAction> m_CastEnds = new();
        private readonly Dictionary<Skill, BossEnergyVfx> m_Casts = new();
        private Unit m_Unit;
        private SkillController m_Skills;
        private ModuleContext m_Context;
        private Color m_Color = Color.white;
        public IModuleContext ModuleContext => m_Context;

        public void Init(Unit unit)
        {
            ResetProgressionInternal();
            m_Unit = unit;
            m_Context = new ModuleContext(unit, gameObject);
            foreach (var theme in m_Themes)
                if (theme.Boss == unit.Config) { m_Color = theme.Color; break; }
            if (!unit.HasBind(out m_Skills)) return;
            foreach (var skill in m_Skills.Skills) RegisterInternal(skill);
            m_Skills.OnSkillAdded.AddListener(RegisterInternal);
            m_Skills.OnSkillRemoved.AddListener(UnregisterInternal);
        }

        public static BossAbilityVisuals ForUnit(Unit unit)
        {
            for (int depth = 0; unit != null && depth < c_MaxOwnerDepth; depth++)
            {
                if (unit.TryGetComponent<BossAbilityVisuals>(out var visuals)) return visuals;
                if (unit is not SummonerUnit summoned || summoned.Summoner == null) return null;
                unit = summoned.Summoner.AbilityContext.SkillContext.ModuleContext.Unit;
            }
            return null;
        }

        private void RegisterInternal(Skill skill)
        {
            if (skill == null || m_Activations.ContainsKey(skill)) return;
            UnityAction<IAbilityContext> activate = context => ActivateInternal(context);
            UnityAction start = () => StartCastInternal(skill);
            UnityAction end = () => StopCastInternal(skill);
            m_Activations.Add(skill, activate);
            m_CastStarts.Add(skill, start);
            m_CastEnds.Add(skill, end);
            skill.OnActivateIndividu.AddListener(activate);
            skill.OnCastingStartEvent.AddListener(start);
            skill.OnCastingSuccessEvent.AddListener(end);
            skill.OnCastingFailEvent.AddListener(end);
        }

        private void UnregisterInternal(Skill skill)
        {
            if (skill == null || !m_Activations.TryGetValue(skill, out var action)) return;
            skill.OnActivateIndividu.RemoveListener(action);
            skill.OnCastingStartEvent.RemoveListener(m_CastStarts[skill]);
            skill.OnCastingSuccessEvent.RemoveListener(m_CastEnds[skill]);
            skill.OnCastingFailEvent.RemoveListener(m_CastEnds[skill]);
            StopCastInternal(skill);
            m_Activations.Remove(skill);
            m_CastStarts.Remove(skill);
            m_CastEnds.Remove(skill);
        }

        private void StartCastInternal(Skill skill)
        {
            StopCastInternal(skill);
            var effect = AcquireInternal();
            if (effect == null) return;
            m_Casts[skill] = effect;
            effect.Play(BossEnergyKind.Cast, m_Color, Center(m_Unit), c_CastSize,
                skill.SkillConfig.Casting.CastDuration, m_Unit.transform);
        }

        private void StopCastInternal(Skill skill)
        {
            if (!m_Casts.Remove(skill, out var effect) || effect == null) return;
            effect.Stop();
        }

        private void ActivateInternal(IAbilityContext context)
        {
            if (m_Unit == null || !m_Unit.gameObject.activeInHierarchy) return;
            var ability = context.AbilityDeliver.AbilityConfig;
            if (ability is ShooterAbilityConfig { UsesCustomShotVisuals: true }) return;
            BossEnergyKind kind = Kind(ability);
            float size = kind == BossEnergyKind.Shield || kind == BossEnergyKind.Barrier ? c_DefenseSize : c_LaunchSize;
            var effect = AcquireInternal();
            if (effect != null) effect.Play(kind, m_Color, Center(m_Unit), size,
                kind == BossEnergyKind.Summon ? c_SummonSeconds : c_DefenseSeconds, m_Unit.transform);
        }

        public void Impact(IAbilityContext context, Unit target)
        {
            if (target == null || m_Unit == null || !m_Unit.gameObject.activeInHierarchy) return;
            var ability = context.AbilityDeliver.AbilityConfig;
            if (ability is ShooterAbilityConfig { UsesCustomShotVisuals: true }) return;
            var kind = Kind(ability);
            if (kind == BossEnergyKind.Summon || kind == BossEnergyKind.Shield || kind == BossEnergyKind.Barrier) return;
            float size = c_ImpactSize;
            if (ability is DirectDamageAbilityConfig direct && direct.ExplodeSetup.ExplodeOnHit)
                size = Mathf.Max(size, direct.ExplodeSetup.ExplosionRadius * 2f);
            var effect = AcquireInternal();
            if (effect != null) effect.Play(kind == BossEnergyKind.Slash ? kind : BossEnergyKind.Impact,
                m_Color, Center(target), size, c_ImpactSeconds);
        }

        public void StatusApplied(StatusEffectConfig status, Unit target)
        {
            if (status != m_DevotionConfig || target == null) return;
            var effect = AcquireInternal();
            if (effect != null) effect.Play(BossEnergyKind.Devotion, m_Color, Center(target),
                c_ImpactSize, c_ImpactSeconds);
        }

        public void SummonAt(Unit summoned)
        {
            var effect = AcquireInternal();
            if (effect != null) effect.Play(BossEnergyKind.Summon, m_Color, Center(summoned),
                c_SummonSize, c_SummonSeconds);
        }

        public BossEnergyVfx CreateProjectileVisual(Transform ammo)
        {
            if (m_EffectPrefab == null) return null;
            var effect = Instantiate(m_EffectPrefab, ammo, false);
            effect.Play(BossEnergyKind.Projectile, m_Color, ammo.position, c_ProjectileSize, 0f);
            return effect;
        }

        public void ConfigureProjectileVisual(BossEnergyVfx effect, Transform ammo)
        {
            effect.Play(BossEnergyKind.Projectile, m_Color, ammo.position, c_ProjectileSize, 0f);
        }

        private BossEnergyVfx AcquireInternal()
        {
            foreach (var effect in m_Effects)
                if (effect != null && !effect.gameObject.activeSelf && !m_Casts.ContainsValue(effect)) return effect;
            if (m_EffectPrefab == null || m_Effects.Count >= c_MaxEffects) return null;
            var created = Instantiate(m_EffectPrefab, transform, false);
            m_Effects.Add(created);
            return created;
        }

        private static Vector3 Center(Unit unit)
        {
            if (unit.HasBind(out AvatarSpine avatar) &&
                avatar.SkeletonAnimation.TryGetComponent<MeshRenderer>(out var renderer))
                return renderer.bounds.center;
            return unit.transform.position;
        }

        private static BossEnergyKind Kind(AbilityConfig ability)
        {
            if (ability is SummonAbilityConfig) return BossEnergyKind.Summon;
            string id = ability.BaseInfo.Id;
            if (id.Contains("barrier")) return BossEnergyKind.Barrier;
            if (id.Contains("shield")) return BossEnergyKind.Shield;
            if (id == "demonslash") return BossEnergyKind.Slash;
            if (id == "devotionimplant") return BossEnergyKind.Devotion;
            return BossEnergyKind.Launch;
        }

        public void ResetProgression() => ResetProgressionInternal();
        private void ResetProgressionInternal()
        {
            if (m_Skills != null)
            {
                m_Skills.OnSkillAdded.RemoveListener(RegisterInternal);
                m_Skills.OnSkillRemoved.RemoveListener(UnregisterInternal);
            }
            foreach (var skill in new List<Skill>(m_Activations.Keys)) UnregisterInternal(skill);
            foreach (var effect in m_Effects) if (effect != null) effect.Stop();
            m_Skills = null;
            m_Unit = null;
        }

        private void OnDisable() => ResetProgressionInternal();
    }
}
