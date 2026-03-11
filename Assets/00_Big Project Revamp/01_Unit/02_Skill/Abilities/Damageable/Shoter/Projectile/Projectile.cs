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
            m_Attacker.AttackerField.SetEnabled(!isExplodeOnHit);
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
            bool explodeOnHit = m_ProjectileConfig.ExplodeSetup.ExplodeOnHit;

            if (explodeOnHit)
            {
                Explode();
            }

            m_OnHit?.Invoke(target);

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
