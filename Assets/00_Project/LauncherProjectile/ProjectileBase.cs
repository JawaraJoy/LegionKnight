using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Purchasing;

namespace LegionKnight
{
    /// <summary>
    /// Base projectile class shared between 2D and 3D implementations.
    /// Handles lifetime, homing, hit logic, and pooling safety.
    /// </summary>
    public abstract class ProjectileBase : MonoBehaviour
    {
        [Header("Events")]
        [Tooltip("Called when projectile is spawned from pool")]
        public UnityEvent OnSpawned;

        [Tooltip("Called when projectile is released back to pool")]
        public UnityEvent OnReleased;
        [Tooltip("Called when projectile collides with a target")]
        public UnityEvent<GameObject> OnCollideEnter;

        [Tooltip("Called when projectile stops colliding with a target")]
        public UnityEvent<GameObject> OnCollideExit;

        [Tooltip("Called when projectile pierce count changes")]
        public UnityEvent<int> OnPierceCount;

        protected ProjectileShotConfig m_Config;
        protected Transform m_Target;
        protected float m_Timer;
        protected int m_RemainingPierce;
        protected Vector3 m_Direction;
        protected bool m_IsReleased;

        /// <summary>
        /// Initializes projectile state when spawned from pool.
        /// </summary>
        public virtual void Initialize(ProjectileShotConfig config, Transform target)
        {
            m_Config = config;
            m_Target = target;
            m_Timer = 0f;
            m_RemainingPierce = config.PierceCount;
            m_Direction = transform.right;
        }

        protected virtual void OnEnable()
        {
            m_IsReleased = false;
            OnSpawned?.Invoke();
        }

        protected virtual void Update()
        {
            UpdateTarget();
            UpdateMovement();
            UpdateLifetime();
        }

        /// <summary>
        /// Updates homing target and turning behavior.
        /// </summary>
        protected virtual void UpdateTarget()
        {
            if (!m_Config.Homing || m_Target == null)
                return;

            Vector3 desired = (m_Target.position - transform.position).normalized;
            m_Direction = Vector3.Lerp(
                m_Direction,
                desired,
                m_Config.TurnSpeed * Time.deltaTime
            ).normalized;
        }

        /// <summary>
        /// Handles projectile movement based on motion type.
        /// </summary>
        protected abstract void UpdateMovement();

        /// <summary>
        /// Handles auto-despawn based on lifetime.
        /// </summary>
        protected virtual void UpdateLifetime()
        {
            if (!m_Config.UseLifetime)
                return;

            m_Timer += Time.deltaTime;
            if (m_Timer >= m_Config.Lifetime)
                Release();
        }
        /// <summary>
        /// Handles hit behavior (destroy, pierce, explode, stick).
        /// </summary>
        protected void HandleHit(GameObject target)
        {
            OnCollideEnter?.Invoke(target);
            switch (m_Config.HitBehavior)
            {
                case HitBehavior.Pierce:
                    m_RemainingPierce--;
                    OnPierceCount?.Invoke(m_RemainingPierce);
                    if (m_RemainingPierce <= 0)
                        Release();
                    break;

                case HitBehavior.Explode:
                    Explode();
                    Release();
                    break;

                case HitBehavior.Stick:
                    enabled = false;
                    break;

                default:
                    Release();
                    break;
            }
        }

        /// <summary>
        /// Explosion logic (2D / 3D specific).
        /// </summary>
        protected abstract void Explode();

        /// <summary>
        /// Releases projectile back to pool safely.
        /// </summary>
        protected void Release()
        {
            if (m_IsReleased) return;
            m_IsReleased = true;

            OnReleased?.Invoke();
            ProjectilePool.Release(m_Config.ProjectilePrefab, this);
        }
    }
}
