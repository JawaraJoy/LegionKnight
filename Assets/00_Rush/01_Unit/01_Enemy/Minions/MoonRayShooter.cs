using UnityEngine;

namespace Rush
{
    public sealed class MoonRayShooter : Shooter
    {
        [SerializeField, Min(0.01f)] private float m_DartInterval = 2f;
        [SerializeField, Min(0.01f)] private float m_DartSpeed = 14f;
        [SerializeField, Min(0.01f)] private float m_Radius = 2.5f;
        private Transform m_UnitRoot;
        private Damageable m_Health;
        private Skill m_Skill;
        private Vector3 m_Anchor;
        private Vector3 m_Destination;
        private float m_Elapsed;
        private bool m_Anchored;

        public override void Init(AbilityConfig config, ISkillContext context)
        {
            base.Init(config, context);
            var owner = context.ModuleContext.Unit;
            m_UnitRoot = owner.transform;
            owner.HasBind(out m_Health);
            m_Skill = context.Skill as Skill;
        }

        private void Update()
        {
            if (m_Health == null || !m_Health.IsAlive || !m_Skill.IsActive) return;
            if (!m_Anchored)
            {
                m_Anchor = m_Destination = m_UnitRoot.localPosition;
                m_Anchored = true;
            }
            m_Elapsed += Time.deltaTime;
            if (m_Elapsed >= m_DartInterval)
            {
                m_Elapsed %= m_DartInterval;
                Vector2 offset = Random.insideUnitCircle.normalized * Random.Range(m_Radius * 0.55f, m_Radius);
                if (Vector2.Distance(m_Anchor + (Vector3)offset, m_UnitRoot.localPosition) < m_Radius * 0.5f)
                    offset = -offset;
                m_Destination = m_Anchor + (Vector3)offset;
            }
            m_UnitRoot.localPosition = Vector3.MoveTowards(m_UnitRoot.localPosition, m_Destination, m_DartSpeed * Time.deltaTime);
        }

        private void OnDisable()
        {
            m_Elapsed = 0f;
            m_Anchored = false;
        }
    }
}
