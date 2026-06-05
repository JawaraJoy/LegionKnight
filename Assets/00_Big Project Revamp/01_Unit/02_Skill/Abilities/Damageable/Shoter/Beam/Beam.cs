using MoreMountains.Tools;
using UnityEngine;
using UnityEngine.Events;

namespace Rush
{
    [RequireComponent(typeof(LineRenderer))]
    public class Beam : Ammo
    {
        [SerializeField, MMReadOnly]
        private LineRenderer m_Line;

        [SerializeField, MMReadOnly]
        private BeamConfig m_BeamConfig;
        [SerializeField]
        private UnityEvent m_OnBeamStart;
        [SerializeField]
        private UnityEvent m_OnBeamEnd;

        private float m_DamageTimer;
        private float m_CurrentWidth;
        private float m_CurrentBeamLength;

        private readonly RaycastHit2D[] m_Hits = new RaycastHit2D[64];

        public override void Init(AbilityContext context, AmmoConfig config)
        {
            base.Init(context, config);

            m_BeamConfig = config as BeamConfig;
            m_Line = GetComponent<LineRenderer>();

            m_Line.positionCount = 2;
            m_Line.enabled = false;
            m_Line.textureMode = LineTextureMode.Tile;
        }

        public override void PrepareForSpawn(Vector3 position, Quaternion rotation)
        {
            base.PrepareForSpawn(position, rotation);

            m_DamageTimer = 0f;
            m_CurrentBeamLength = m_BeamConfig.MaxLength;

            m_CurrentWidth = m_BeamConfig.AnimateWidth
                ? 0f
                : m_BeamConfig.Width;

            ApplyWidth();

            m_Line.enabled = false;
        }

        public override void Shot(ITargetable targetable)
        {
            base.Shot(targetable);

            FaceTarget(targetable);
            Transform parent = m_AbilityContext.AbilityDeliver.DeliverTransform;
            transform.SetParent(parent);
            m_OnBeamStart?.Invoke();
            m_Line.enabled = true;
        }

        public override void Tick()
        {
            UpdateBeam();
            UpdateDamage();

            UpdateLifetime(0f);
        }

        private void FaceTarget(ITargetable targetable)
        {
            if (targetable == null)
                return;

            if (targetable.TargetTransform == null)
                return;

            Vector2 direction =
                targetable.TargetTransform.position -
                transform.position;

            if (direction.sqrMagnitude < 0.0001f)
                return;

            transform.right = direction.normalized;
        }

        private void UpdateBeam()
        {
            Vector3 origin = transform.position;
            Vector3 direction = transform.right;

            if (m_BeamConfig.Piercing)
            {
                m_CurrentBeamLength = m_BeamConfig.MaxLength;
            }
            else
            {
                m_CurrentBeamLength = m_BeamConfig.MaxLength;

                RaycastHit2D hit = Physics2D.Raycast(
                    origin,
                    direction,
                    m_BeamConfig.MaxLength,
                    m_Config.TargetLayer);

                if (hit.collider != null)
                {
                    m_CurrentBeamLength = hit.distance;
                }
            }

            m_Line.SetPosition(0, origin);
            m_Line.SetPosition(
                1,
                origin + direction * m_CurrentBeamLength);

            UpdateWidth();
            UpdateTextureScale();
        }

        private void UpdateWidth()
        {
            if (!m_BeamConfig.AnimateWidth)
                return;

            float speed =
                m_BeamConfig.Width /
                Mathf.Max(0.01f, m_BeamConfig.ExpandTime);

            m_CurrentWidth = Mathf.MoveTowards(
                m_CurrentWidth,
                m_BeamConfig.Width,
                speed * Time.deltaTime);

            ApplyWidth();
        }

        private void ApplyWidth()
        {
            m_Line.widthMultiplier = m_CurrentWidth;
        }

        private void UpdateTextureScale()
        {
            if (m_Line.material == null)
                return;

            m_Line.material.mainTextureScale = new Vector2(
                m_CurrentBeamLength,
                1f);
        }

        private void UpdateDamage()
        {
            m_DamageTimer -= Time.deltaTime;

            if (m_DamageTimer > 0f)
                return;

            m_DamageTimer = m_BeamConfig.DamageInterval;

            int hitCount = Physics2D.RaycastNonAlloc(
                transform.position,
                transform.right,
                m_Hits,
                m_CurrentBeamLength,
                m_Config.TargetLayer);

            for (int i = 0; i < hitCount; i++)
            {
                RaycastHit2D hit = m_Hits[i];

                if (hit.collider == null)
                    continue;

                if (!hit.collider.TryGetComponent(
                        out Damageable damageable))
                    continue;

                if (!AbilityUltility.IsTargetAllowedByTargetObject(
                        m_AbilityContext.AbilityDeliver,
                        damageable))
                    continue;

                damageable.TakeDamage(m_AbilityContext);

                m_OnHit?.Invoke(damageable.gameObject);

                if (!m_BeamConfig.Piercing)
                    break;
            }
        }

        protected override void DisableAmmo()
        {
            m_Line.enabled = false;

            m_OnBeamEnd?.Invoke();
            Transform parent = m_AbilityContext.AbilityDeliver.DeliverTransform;
            transform.SetParent(parent);
            base.DisableAmmo();
        }

#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;

            Gizmos.DrawLine(
                transform.position,
                transform.position +
                transform.right * m_CurrentBeamLength);
        }
#endif
    }
}