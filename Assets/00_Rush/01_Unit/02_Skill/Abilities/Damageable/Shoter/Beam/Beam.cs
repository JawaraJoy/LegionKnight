using System.Collections.Generic;
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
        private Damageable m_ContactTarget;
        private bool StopsAtShotTarget => m_BeamConfig.DamageOnlyShotTarget && !m_BeamConfig.Piercing;

        private readonly List<RaycastHit2D> m_Hits = new(64);

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
            m_ContactTarget = null;
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
            if (StopsAtShotTarget)
                UpdateBeam();
            OnBeamStartInvoke();
            m_Line.enabled = true;
        }

        public override void Tick()
        {
            if (!m_Line.enabled) return;
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

            if (StopsAtShotTarget)
            {
                UpdateShotTargetContactInternal();
            }
            else if (m_BeamConfig.Piercing)
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

            if (StopsAtShotTarget)
            {
                if (m_ContactTarget != null)
                    ApplyBeamDamageInternal(m_ContactTarget);
                return;
            }

            int hitCount = RaycastHitsInternal(m_CurrentBeamLength);
            for (int i = 0; i < hitCount; i++)
            {
                RaycastHit2D hit = m_Hits[i];
                if (!hit.collider.TryGetComponent(out Damageable damageable))
                    continue;
                if (m_BeamConfig.DamageOnlyShotTarget &&
                    (!ReferenceEquals(damageable, m_Targetable) || !damageable.IsAlive))
                    continue;
                if (!AbilityUltility.IsTargetAllowedByTargetObject(m_AbilityContext.AbilityDeliver, damageable))
                    continue;

                ApplyBeamDamageInternal(damageable);
                if (m_BeamConfig.DamageOnlyShotTarget || !m_BeamConfig.Piercing)
                    break;
            }
        }

        private void UpdateShotTargetContactInternal()
        {
            m_ContactTarget = null;
            m_CurrentBeamLength = 0f;
            if (!(m_Targetable is Damageable target) || target == null || !target.IsAlive ||
                !AbilityUltility.IsTargetAllowedByTargetObject(m_AbilityContext.AbilityDeliver, target))
                return;

            int hitCount = RaycastHitsInternal(m_BeamConfig.MaxLength);
            for (int i = 0; i < hitCount; i++)
            {
                RaycastHit2D hit = m_Hits[i];
                if (hit.collider.gameObject != target.gameObject) continue;
                if (m_ContactTarget != null && hit.distance >= m_CurrentBeamLength) continue;
                m_ContactTarget = target;
                m_CurrentBeamLength = hit.distance;
            }
        }

        private int RaycastHitsInternal(float distance)
        {
            var filter = new ContactFilter2D
            {
                useLayerMask = true,
                layerMask = m_Config.TargetLayer,
                useTriggers = Physics2D.queriesHitTriggers
            };
            return gameObject.scene.GetPhysicsScene2D().Raycast(
                transform.position, transform.right, distance, filter, m_Hits);
        }

        private void ApplyBeamDamageInternal(Damageable damageable)
        {
            damageable.TakeDamage(m_AbilityContext);
            OnHitInvoke(damageable.gameObject);
        }

        protected override void DisableAmmo()
        {
            m_Line.enabled = false;

            OnBeamEndInvoke();
            Transform parent = m_AbilityContext.AbilityDeliver.DeliverTransform;
            transform.SetParent(parent);
            base.DisableAmmo();
        }

private void OnBeamStartInvoke()
        {
            m_OnBeamStart?.Invoke();
        }

        private void OnBeamEndInvoke()
        {
            m_OnBeamEnd?.Invoke();
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
