using System.Collections.Generic;
using UnityEngine;

namespace Rush
{
    /// <summary>
    /// Projectile ammo with collision handling and optional explosion.
    /// Movement & lifetime are handled by Ammo base class.
    /// </summary>
    [DisallowMultipleComponent]
    public class Projectile : Ammo
    {
        private ProjectileConfig m_ProjectileConfig;
        [SerializeField]
        private Attacker m_Attacker;
        [SerializeField]
        private bool m_UseSweptCollision2D;

        private readonly List<RaycastHit2D> m_SweepHits = new List<RaycastHit2D>();
        private bool m_HasFinishedHit;
        private bool UsesSweep => m_UseSweptCollision2D &&
            RushGameManager.Instance.GameConfig.PhysicsMode == PhysicsMode.Physics2D;
        private float SweepRadius
        {
            get
            {
                if (!TryGetComponent(out CircleCollider2D circle)) return 0f;
                Vector3 scale = transform.lossyScale;
                return circle.radius * Mathf.Max(Mathf.Abs(scale.x), Mathf.Abs(scale.y));
            }
        }
        
        public override void Init(AbilityContext context, AmmoConfig config)
        {
            base.Init(context, config);
            m_Attacker.OnAttackDeliveredTarget.RemoveListener(OnAttackDelivered);
            m_Attacker.OnAttackDeliveredTarget.AddListener(OnAttackDelivered);
            m_Attacker.Init(context);
            if (config is ProjectileConfig projectileConfig)
            {
                m_ProjectileConfig = projectileConfig;
            }
            else
            {
                Debug.LogError("[Projectile] Invalid config type. Expected ProjectileConfig.");
            }
            bool isExplodeOnHit = m_ProjectileConfig.ExplodeSetup.ExplodeOnHit;
            m_Attacker.AttackerField.SetEnabled(!isExplodeOnHit && !UsesSweep);
        }

        public override void PrepareForSpawn(Vector3 position, Quaternion rotation)
        {
            base.PrepareForSpawn(position, rotation);
            m_HasFinishedHit = false;
        }

        protected override float Move()
        {
            Vector3 start = transform.position;
            float traveled = base.Move();
            if (!UsesSweep)
                return traveled;

            Vector3 end = transform.position;
            Vector2 displacement = end - start;
            float distance = displacement.magnitude;
            if (distance <= Mathf.Epsilon)
                return traveled;

            var filter = new ContactFilter2D
            {
                useLayerMask = true,
                layerMask = m_AbilityContext.AbilityDeliver.AbilityConfig.TargetFilter,
                useTriggers = true
            };
            m_SweepHits.Clear();
            Physics2D.CircleCast(start, SweepRadius, displacement / distance, filter, m_SweepHits, distance);
            m_SweepHits.Sort((a, b) => a.distance.CompareTo(b.distance));
            foreach (var hit in m_SweepHits)
            {
                if (hit.collider == null || !IsValidHit(hit.collider.gameObject))
                    continue;
                transform.position = start + (Vector3)(displacement / distance * hit.distance);
                HandleHit(hit.collider.gameObject);
                if (m_ProjectileConfig.DespawnOnHit)
                    return hit.distance;
            }
            transform.position = end;
            return traveled;
        }
        protected virtual void OnTriggerEnter(Collider other)
        {
            if (RushGameManager.Instance.GameConfig.PhysicsMode != PhysicsMode.Physics3D)
                return;

            if (!IsValidHit(other.gameObject))
                return;

            HandleHit(other.gameObject);
        }

        protected virtual void OnCollisionEnter(Collision collision)
        {
            if (RushGameManager.Instance.GameConfig.PhysicsMode != PhysicsMode.Physics3D)
                return;

            if (!IsValidHit(collision.gameObject))
                return;

            HandleHit(collision.gameObject);
        }

        protected virtual void OnTriggerEnter2D(Collider2D other)
        {
            if (RushGameManager.Instance.GameConfig.PhysicsMode != PhysicsMode.Physics2D)
                return;

            if (!IsValidHit(other.gameObject))
                return;

            HandleHit(other.gameObject);
        }

        protected virtual void OnCollisionEnter2D(Collision2D collision)
        {
            if (RushGameManager.Instance.GameConfig.PhysicsMode != PhysicsMode.Physics2D)
                return;

            if (!IsValidHit(collision.gameObject))
                return;

            HandleHit(collision.gameObject);
        }

        protected virtual void HandleHit(GameObject target)
        {
            
            if (UsesSweep && m_HasFinishedHit) return;
            if (UsesSweep && m_ProjectileConfig.DespawnOnHit) m_HasFinishedHit = true;
            bool explodeOnHit = m_ProjectileConfig.ExplodeSetup.ExplodeOnHit;
            OnHitInvoke(target);
            if (UsesSweep && !explodeOnHit && target.TryGetComponent(out ITargetable receiver))
            {
                target.TryGetComponent(out IDamageable damageable);
                receiver.Notify(m_AbilityContext);
                if (damageable != null) m_Attacker.OnAttackDeliveredInvoke(damageable);
                m_Attacker.OnAttackDeliveredTargetInvoke(receiver);
            }
            if (explodeOnHit)
            {
                Explode();
            }
            if (m_ProjectileConfig.DespawnOnHit)
            {
                DisableAmmo();
            }
            
        }
        private void OnAttackDelivered(ITargetable target)
        {
            if (m_ProjectileConfig.DespawnOnHit)
            {
                DisableAmmo();
            }
        }

        private void Explode()
        {
            if (m_ProjectileConfig == null)
                return;

            AbilityConfig abilityConfig = m_AbilityContext.AbilityDeliver.AbilityConfig;
            float radius = m_ProjectileConfig.ExplodeSetup.ExplosionRadius;

            if (RushGameManager.Instance.GameConfig.PhysicsMode == PhysicsMode.Physics2D)
            {
                Collider2D[] hits = Physics2D.OverlapCircleAll(
                    transform.position,
                    radius,
                    abilityConfig.TargetFilter
                );

                foreach (var hit in hits)
                {
                    if (!hit.TryGetComponent(out ITargetable target))
                        continue;

                    if (!abilityConfig.CanTargetDeathUnit && !target.IsAlive)
                        continue;

                    if (!AbilityUltility.IsTargetAllowedByTargetObject(
                            m_AbilityContext.AbilityDeliver,
                            target))
                        continue;

                    target.Notify(m_AbilityContext);
                }
            }
            else
            {
                Collider[] hits = Physics.OverlapSphere(
                    transform.position,
                    radius,
                    abilityConfig.TargetFilter
                );

                foreach (var hit in hits)
                {
                    if (!hit.TryGetComponent(out ITargetable target))
                        continue;

                    if (!abilityConfig.CanTargetDeathUnit && !target.IsAlive)
                        continue;

                    if (!AbilityUltility.IsTargetAllowedByTargetObject(
                            m_AbilityContext.AbilityDeliver,
                            target))
                        continue;

                    target.Notify(m_AbilityContext);
                }
            }
        }
    }
}
