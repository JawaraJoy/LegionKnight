using System.Collections.Generic;
using UnityEngine;

namespace Rush
{
    public sealed class EmberBellApproach : AbilityDeliver
    {
        [SerializeField, Min(0f)] private float m_Stride = 4f;
        [SerializeField, Min(0f)] private float m_Speed = 8f;
        [SerializeField, Min(0f)] private float m_StopDistance = 1f;
        [SerializeField, Min(0.01f)] private float m_PulseInterval = 2f;
        [SerializeField] private SkillConfig m_PulseSkill;

        private Unit m_Owner;
        private Damageable m_Health;
        private Collider2D m_Hitbox;
        private Damageable m_Hero;
        private Collider2D m_HeroHitbox;
        private SkillController m_Skills;
        private Skill m_Skill;
        private Skill m_Pulse;
        private float m_RemainingStride;
        private float m_TimeInRange;
        private readonly List<ITargetable> m_Target = new(1);

        public override void Init(AbilityConfig config, ISkillContext context)
        {
            base.Init(config, context);
            m_Owner = context.ModuleContext.Unit;
            m_Skill = context.Skill as Skill;
            m_Owner.HasBind(out m_Health);
            m_Owner.HasBind(out m_Skills);
            m_Hitbox = m_Health.GetComponent<Collider2D>();
        }

        public override void Activate()
        {
            m_RemainingStride = m_Stride;
            base.Activate();
        }

        private void Update()
        {
            if (m_Health == null || !m_Health.IsAlive || !m_Skill.IsActive) return;
            var hero = RushPlayer.Instance?.Unit;
            if (hero == null || !hero.HasBind(out Damageable target) || !target.IsAlive)
            {
                ResetCombat();
                return;
            }
            if (m_Hero != target)
            {
                m_Hero = target;
                m_HeroHitbox = target.GetComponent<Collider2D>();
                m_Target.Clear();
                m_Target.Add(target);
                m_TimeInRange = 0f;
            }
            Vector2 ownCenter = m_Hitbox.transform.TransformPoint(m_Hitbox.offset);
            Vector2 heroCenter = m_HeroHitbox != null ? m_HeroHitbox.transform.TransformPoint(m_HeroHitbox.offset) : target.transform.position;
            Vector2 direction = heroCenter - ownCenter;
            float distance = direction.magnitude;
            float travel = Mathf.Min(m_RemainingStride, m_Speed * Time.deltaTime, Mathf.Max(0f, distance - m_StopDistance));
            if (travel > 0f)
            {
                m_Owner.transform.position += (Vector3)(direction * (travel / distance));
                m_RemainingStride -= travel;
                distance -= travel;
            }
            if (distance <= m_StopDistance + 0.001f) m_RemainingStride = 0f;
            if (distance > m_PulseSkill.AbilitySets[0].Range)
            {
                m_TimeInRange = 0f;
                return;
            }
            m_TimeInRange += Time.deltaTime;
            if (m_TimeInRange < m_PulseInterval) return;
            m_TimeInRange -= m_PulseInterval;
            if (m_Pulse == null) m_Pulse = m_Skills.GetSkillActivator(m_PulseSkill);
            if (m_Pulse != null)
            {
                m_Pulse.ForceActivateAllOverrideTargets(m_Target);
                m_Owner.Config.PlayAttack(m_Owner);
            }
        }

        private void ResetCombat()
        {
            m_RemainingStride = 0f;
            m_TimeInRange = 0f;
        }

        private void OnDisable()
        {
            ResetCombat();
            m_Hero = null;
            m_Pulse = null;
            m_Target.Clear();
        }
    }
}
