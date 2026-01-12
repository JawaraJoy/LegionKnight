using LegionKnight;
using MoreMountains.Tools;
using UnityEngine;
using UnityEngine.Events;

namespace Rush
{
    /// <summary>
    /// Projectile that moves straight along a local axis,
    /// supports time/distance lifetime, and despawns early on collision.
    /// </summary>
    [DisallowMultipleComponent]
    public class Projectile : Bindable, IUpdater, IProjectile, IAbility
    {
        public enum LocalAxis
        {
            X,
            Y,
            Z
        }

        [SerializeField, MMReadOnly]
        private bool m_CanMove = true;
        [SerializeField, MMReadOnly]
        private ProjectileTargetingMode m_TargetingMode = ProjectileTargetingMode.None;
        [SerializeField]
        private bool m_ExplodeOnDespawn = false;
        [SerializeField]
        private float m_ExplosionRadius = 5f;
        [Header("Movement")]
        [SerializeField]
        [Tooltip("Movement speed in units per second")]
        private float m_Speed = 10f;
        [SerializeField]
        private float m_HomingTurnSpeed = 0.2f;

        [SerializeField]
        [Tooltip("Local axis the projectile will move along")]
        private LocalAxis m_MoveAxis = LocalAxis.Z;

        [Header("Lifetime - Time")]

        [SerializeField]
        [Tooltip("How long the projectile stays alive in seconds (0 = infinite)")]
        private float m_Lifetime = 0f;

        [Header("Lifetime - Distance")]

        [SerializeField]
        [Tooltip("Maximum distance the projectile can travel (0 = infinite)")]
        private float m_MaxDistance = 10f;

        [Header("Collision")]

        [SerializeField]
        [Tooltip("Layers that will cause the projectile to despawn on hit")]
        private LayerMask m_HitLayers = ~0;

        [SerializeField]
        [Tooltip("If true, projectile despawns immediately on hit")]
        private bool m_DespawnOnHit = true;

        [SerializeField, MMReadOnly]
        private Targetable m_Targetable;
        [SerializeField, MMReadOnly]
        private Shoter m_Shoter;

        [SerializeField]
        private UnityEvent<AbilityContext> m_OnShot;
        [SerializeField]
        private UnityEvent<GameObject> m_OnHit;
        private Vector3 m_MoveDirection;
        private float m_LifeTimer;
        private float m_TraveledDistance;

        public ProjectileTargetingMode TargetingMode => m_TargetingMode;
        public bool CanMove => m_CanMove;
        public bool IsActive => gameObject.activeInHierarchy;
        public float Speed => m_Speed;
        public float HomingTurnSpeed => m_HomingTurnSpeed;
        public float Lifetime => m_Lifetime;
        public float MaxDistance => m_MaxDistance;
        public Targetable Targetable => m_Targetable;
        public Shoter Shoter => m_Shoter;

        private AbilityContext m_AbilityContext;
        public AbilityContext AbilityContext => m_AbilityContext;
        public bool Initialized => m_AbilityContext.Initialized;

        private void Awake()
        {
            CacheMoveDirection();
        }

        private void OnEnable()
        {
            ResetLifetime();
            RegisterUpdater();
        }

        private void OnDisable()
        {
            UnregisterUpdater();
        }

        private void OnDestroy()
        {
            UnregisterUpdater();
        }


        private void CacheMoveDirection()
        {
            switch (m_MoveAxis)
            {
                case LocalAxis.X:
                    m_MoveDirection = Vector3.right;
                    break;
                case LocalAxis.Y:
                    m_MoveDirection = Vector3.up;
                    break;
                case LocalAxis.Z:
                    m_MoveDirection = Vector3.forward;
                    break;
            }
        }

        private float Move()
        {
            float distance = m_Speed * Time.deltaTime;

            transform.Translate(
                m_MoveDirection * distance,
                Space.Self
            );

            return distance;
        }

        

        private void UpdateLifetime(float deltaDistance)
        {
            if (m_Lifetime > 0f)
            {
                m_LifeTimer += Time.deltaTime;
                if (m_LifeTimer >= m_Lifetime)
                {
                    DisableProjectile();
                    return;
                }
            }

            if (m_MaxDistance > 0f)
            {
                m_TraveledDistance += deltaDistance;
                if (m_TraveledDistance >= m_MaxDistance)
                {
                    DisableProjectile();
                }
            }
        }

        private void ResetLifetime()
        {
            m_LifeTimer = 0f;
            m_TraveledDistance = 0f;
        }
        private void OnTriggerEnter(Collider other)
        {
            if (RushGameManager.Instance.GameConfig.PhysicsMode != PhysicsMode.Physics3D)
                return;

            if (!IsValidHit(other.gameObject))
                return;

            HandleHit(other.gameObject);
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (RushGameManager.Instance.GameConfig.PhysicsMode != PhysicsMode.Physics3D)
                return;

            if (!IsValidHit(collision.gameObject))
                return;

            HandleHit(collision.gameObject);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (RushGameManager.Instance.GameConfig.PhysicsMode != PhysicsMode.Physics2D)
                return;

            if (!IsValidHit(other.gameObject))
                return;

            HandleHit(other.gameObject);
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (RushGameManager.Instance.GameConfig.PhysicsMode != PhysicsMode.Physics2D)
                return;

            if (!IsValidHit(collision.gameObject))
                return;

            HandleHit(collision.gameObject);
        }

        private bool IsValidHit(GameObject target)
        {
            return (m_HitLayers.value & (1 << target.layer)) != 0;
        }

        private void HandleHit(GameObject target)
        {
            if (m_DespawnOnHit)
            {
                DisableProjectile();
            }
            m_OnHit?.Invoke(target);
        }

        private void DisableProjectile()
        {
            if (m_ExplodeOnDespawn)
            {
                Explode();
            }

            m_CanMove = false;
            gameObject.SetActive(false);
            m_Shoter.NotifyProjectileFinished(this);
            
        }

        public void SetSpeed(float speed)
        {
            m_Speed = speed;
        }
        public void SetHomingSmoothAngle(float angle)
        {
            m_HomingTurnSpeed = angle;
        }

        public void SetMoveAxis(LocalAxis axis)
        {
            m_MoveAxis = axis;
            CacheMoveDirection();
        }

        public void SetLifetime(float lifetime)
        {
            m_Lifetime = lifetime;
        }

        public void SetMaxDistance(float distance)
        {
            m_MaxDistance = distance;
        }

        public void Tick()
        {
            if (!m_CanMove) return;
            if (m_TargetingMode == ProjectileTargetingMode.Homing)
            {
                if (IsTargetInvalid())
                {
                    TryRetarget();
                }

                UpdateRotation();
            }
            float deltaDistance = Move();
            UpdateLifetime(deltaDistance);
            
        }
        private bool IsTargetInvalid()
        {
            if (m_Targetable == null) return true;
            if (!m_Targetable.IsAlive) return true;
            return false;
        }
        private void TryRetarget()
        {
            if (m_Shoter == null) return;

            Targetable newTarget = m_Shoter.GetNewTargetForProjectile();
            if (newTarget != null)
            {
                m_Targetable = newTarget;
            }
        }

        private void RegisterUpdater()
        {
            UpdateBank.Instance.RegisterUpdateTick(gameObject, this);
        }

        private void UnregisterUpdater()
        {
            UpdateBank.Instance.UnregisterUpdateTick(gameObject);
        }

        public void Init(AbilityContext context)
        {
            m_AbilityContext = context;
            AbilityDeliver deliver = context.AbilityDeliver;
            if (deliver is Shoter shoter)
            {
                m_Shoter = shoter;
            }
            ShotAbilityConfig shotConfig = m_Shoter.ShotAbilityConfig;
            m_TargetingMode = shotConfig.ProjectileTargetingMode;

            m_ExplodeOnDespawn = shotConfig.ExplodeSetup.ExplodeOnHit;
            m_ExplosionRadius = shotConfig.ExplodeSetup.ExplosionRadius;

            m_Speed = shotConfig.ProjectileSpeed;
            m_HomingTurnSpeed = shotConfig.ProjectileHomingTurnSpeed;
            m_Lifetime = shotConfig.ProjectileLifetime;
            m_MaxDistance = shotConfig.MaxDistance;
            m_DespawnOnHit = shotConfig.DespawnOnHit;

            m_HitLayers = shotConfig.TargetFilter;
        }

        public void Shot(Targetable targetable = null)
        {
            m_Targetable = targetable;
            switch (m_TargetingMode)
            {
                case ProjectileTargetingMode.None:
                    m_Targetable = null;
                    break;
                case ProjectileTargetingMode.Facing:
                    FacingAtFirstTarget2D(targetable);
                    break;
                case ProjectileTargetingMode.Homing:
                    break;
            }
            m_CanMove = true;
            m_OnShot?.Invoke(m_AbilityContext);
        }
        private void Explode()
        {
            AbilityConfig config = m_AbilityContext.AbilityDeliver.Config;

            if (RushGameManager.Instance.GameConfig.PhysicsMode == PhysicsMode.Physics2D)
            {
                Collider2D[] hits = Physics2D.OverlapCircleAll(
                    transform.position,
                    m_ExplosionRadius,
                    config.TargetFilter
                );

                for (int i = 0; i < hits.Length; i++)
                {
                    if (hits[i].TryGetComponent(out Targetable target))
                    {
                        if (!config.CanTargetDeathUnit && !target.IsAlive)
                            continue;
                        if (!AbilityUltility.IsTargetAllowedByTargetObject(m_AbilityContext.AbilityDeliver, target))
                            continue;
                    }
                    Debug.Log($"[Explosion] Hit Target (2D): {target.name}", target.gameObject);
                    target.Notify(m_AbilityContext);
                    
                    // HOOK:
                    // Ability event
                    // Damage system
                    // Status effect system
                }
            }
            else
            {
                Collider[] hits = Physics.OverlapSphere(
                    transform.position,
                    m_ExplosionRadius,
                    config.TargetFilter
                );

                for (int i = 0; i < hits.Length; i++)
                {
                    if (!hits[i].TryGetComponent(out Targetable target))
                        continue;

                    if (!config.CanTargetDeathUnit && !target.IsAlive)
                        continue;

                    Debug.Log($"[Explosion] Hit Target (3D): {target.name}", target.gameObject);

                    // HOOK:
                    // Ability event
                    // Damage system
                    // Status effect system
                }
            }

            Debug.Log($"[Explosion] Radius: {m_ExplosionRadius}", gameObject);
        }


        public void SetTargetingMode(ProjectileTargetingMode targetingMode)
        {
            m_TargetingMode = targetingMode;
        }
        /// <summary>
        /// Smoothly rotates projectile toward its target (2D homing).
        /// Rotation is applied on Z axis only.
        /// </summary>
        private void UpdateRotation()
        {
            if (m_Targetable == null) return;
            if (!m_CanMove) return;

            Vector3 targetPos = m_Targetable.transform.position;
            Vector3 dir = targetPos - transform.position;

            if (dir.sqrMagnitude <= 0.0001f)
                return;

            // angle toward target in degrees (2D)
            float targetAngle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

            // current Z rotation
            float currentAngle = transform.eulerAngles.z;

            // smooth rotate toward target angle
            float newAngle = Mathf.MoveTowardsAngle(
                currentAngle,
                targetAngle,
                m_HomingTurnSpeed * Time.deltaTime
            );

            transform.rotation = Quaternion.Euler(0f, 0f, newAngle);
        }
        private void FacingAtFirstTarget2D(Targetable targetable)
        {
            Targetable.LookAtFirstTarget2D(transform, targetable);
        }
    }
}
