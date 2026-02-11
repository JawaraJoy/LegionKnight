using MoreMountains.Tools;
using UnityEngine;

namespace Rush
{
    [RequireComponent(typeof(LineRenderer))]
    public class Beam : Ammo
    {
        [SerializeField, MMReadOnly]
        private LineRenderer m_Line;

        [SerializeField, MMReadOnly]
        private BeamConfig m_BeamConfig;

        private float m_DamageTimer;
        private float m_CurrentWidth;

        public override void Init(AbilityContext context, AmmoConfig config)
        {
            base.Init(context, config);

            m_BeamConfig = config as BeamConfig;
            m_Line = GetComponent<LineRenderer>();

            m_Line.positionCount = 2;
            m_Line.enabled = false;
        }

        public override void Shot(Targetable targetable)
        {
            base.Shot(targetable);

            m_Line.enabled = true;
            m_CurrentWidth = m_BeamConfig.AnimateWidth ? 0f : m_BeamConfig.Width;
            ApplyWidth();
        }

        public override void Tick()
        {
            UpdateBeam();
            HandleDamage();
            UpdateLifetime(0f);
        }

        private void UpdateBeam()
        {
            Vector3 origin = transform.position;
            Vector3 dir = transform.right;

            float length = m_BeamConfig.MaxLength;

            RaycastHit2D hit = Physics2D.Raycast(
                origin,
                dir,
                length,
                m_Config.TargetLayer
            );

            if (hit.collider != null)
            {
                length = hit.distance;
            }

            m_Line.SetPosition(0, origin);
            m_Line.SetPosition(1, origin + dir * length);

            UpdateWidth();
        }

        private void UpdateWidth()
        {
            if (!m_BeamConfig.AnimateWidth)
                return;

            m_CurrentWidth = Mathf.MoveTowards(
                m_CurrentWidth,
                m_BeamConfig.Width,
                Time.deltaTime / m_BeamConfig.ExpandTime
            );

            ApplyWidth();
        }

        private void ApplyWidth()
        {
            m_Line.startWidth = m_CurrentWidth;
            m_Line.endWidth = m_CurrentWidth;
        }

        private void HandleDamage()
        {
            m_DamageTimer -= Time.deltaTime;
            if (m_DamageTimer > 0f)
                return;

            m_DamageTimer = m_BeamConfig.DamageInterval;

            RaycastHit2D[] hits = Physics2D.RaycastAll(
                transform.position,
                transform.right,
                m_BeamConfig.MaxLength,
                m_Config.TargetLayer
            );

            foreach (var hit in hits)
            {
                if (!hit.collider.TryGetComponent(out Targetable target))
                    continue;

                if (!AbilityUltility.IsTargetAllowedByTargetObject(
                    m_AbilityContext.AbilityDeliver,
                    target))
                    continue;

                target.Notify(m_AbilityContext);

                if (!m_BeamConfig.Piercing)
                    break;
            }
        }

        protected override void DisableAmmo()
        {
            m_Line.enabled = false;
            base.DisableAmmo();
        }
    }
}
