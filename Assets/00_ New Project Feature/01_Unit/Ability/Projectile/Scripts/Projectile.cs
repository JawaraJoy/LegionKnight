using UnityEngine;

namespace Rush
{
    /// <summary>
    /// Projectile that moves straight along a local axis,
    /// supports time/distance lifetime, and despawns early on collision.
    /// </summary>
    [DisallowMultipleComponent]
    public class Projectile : MonoBehaviour, IUpdater, IProjectile
    {
        public enum LocalAxis
        {
            X,
            Y,
            Z
        }
        public enum PhysicsMode
        {
            Physics3D,
            Physics2D
        }
        [SerializeField]
        private PhysicsMode m_PhysicsMode = PhysicsMode.Physics3D;
        [Header("Movement")]

        [SerializeField]
        [Tooltip("Movement speed in units per second")]
        private float m_Speed = 10f;

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

        private Vector3 m_MoveDirection;
        private float m_LifeTimer;
        private float m_TraveledDistance;

        public bool IsActive => gameObject.activeInHierarchy;
        public float Speed => m_Speed;
        public float Lifetime => m_Lifetime;
        public float MaxDistance => m_MaxDistance;
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

        #region Movement

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

        #endregion

        #region Lifetime

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

        #endregion

        #region Collision

        private void OnTriggerEnter(Collider other)
        {
            if (m_PhysicsMode != PhysicsMode.Physics3D)
                return;

            if (!IsValidHit(other.gameObject))
                return;

            HandleHit(other.gameObject);
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (m_PhysicsMode != PhysicsMode.Physics3D)
                return;

            if (!IsValidHit(collision.gameObject))
                return;

            HandleHit(collision.gameObject);
        }
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (m_PhysicsMode != PhysicsMode.Physics2D)
                return;

            if (!IsValidHit(other.gameObject))
                return;

            HandleHit(other.gameObject);
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (m_PhysicsMode != PhysicsMode.Physics2D)
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
            // Hook point:
            // - Apply damage
            // - Spawn hit VFX
            // - Notify other systems

            if (m_DespawnOnHit)
            {
                DisableProjectile();
            }
        }

        #endregion

        #region Pool

        private void DisableProjectile()
        {
            if (TryGetComponent(out PoolObject poolObject))
            {
                PoolManager.Instance.Despawn(poolObject.Definition.Id, poolObject.gameObject);
            }
            else
            {
                gameObject.SetActive(false);
            }
        }

        #endregion

        #region Public API

        public void SetSpeed(float speed)
        {
            m_Speed = speed;
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
            float deltaDistance = Move();
            UpdateLifetime(deltaDistance);
        }

        private void RegisterUpdater()
        {
            UpdateBank.Instance.RegisterUpdateTick(gameObject, this);
        }

        private void UnregisterUpdater()
        {
            UpdateBank.Instance.UnregisterUpdateTick(gameObject);
        }

        public void OnSpawned()
        {
            Debug.Log("Projectile spawned");
        }

        public void OnDespawned()
        {
            Debug.Log("Projectile despawned");
        }

        #endregion
    }
}
