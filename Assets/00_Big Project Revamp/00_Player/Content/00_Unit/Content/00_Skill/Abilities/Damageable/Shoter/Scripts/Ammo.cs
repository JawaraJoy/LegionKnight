using MoreMountains.Tools;
using UnityEngine;
using UnityEngine.Events;

namespace Rush
{
    public abstract class Ammo : Bindable, IUpdater
    {
        [SerializeField, MMReadOnly]
        protected AmmoConfig m_Config;

        [SerializeField, MMReadOnly]
        private bool m_CanMove = true;

        [SerializeField]
        protected UnityEvent<AbilityContext> m_OnShot;

        [SerializeField]
        protected UnityEvent<GameObject> m_OnHit;

        private Vector3 m_MoveDirection;
        private float m_CurrentLifeTimer;
        private float m_TraveledDistance;

        protected Shooter m_Shooter;
        protected Targetable m_Targetable;
        protected AbilityContext m_AbilityContext;

        public AmmoConfig Config => m_Config;
        public bool CanMove => m_CanMove;
        public bool IsActive => gameObject.activeSelf;

        private void OnEnable()
        {
            ResetLifetime();
            UpdateBank.Instance.RegisterUpdateTick(gameObject, this);
        }

        private void OnDisable()
        {
            UpdateBank.Instance.UnregisterUpdateTick(gameObject);
        }

        public virtual void Init(AbilityContext context, AmmoConfig config)
        {
            m_AbilityContext = context;
            m_Config = config;

            if (context.AbilityDeliver is Shooter shooter)
            {
                m_Shooter = shooter;
            }

            CacheMoveDirection();
        }

        public virtual void Shot(Targetable targetable)
        {
            m_Targetable = targetable;
            m_CanMove = true;

            m_OnShot?.Invoke(m_AbilityContext);

            FacingAtFirstTarget2D(targetable);
        }

        public virtual void Tick()
        {
            if (!m_CanMove)
                return;

            if (m_Config.TargetingDistributeMode == TargetingDistributeMode.Homing)
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

        protected virtual void CacheMoveDirection()
        {
            switch (m_Config.ForwardAxis)
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

        protected virtual float Move()
        {
            float distance = m_Config.Speed * Time.deltaTime;

            transform.Translate(
                m_MoveDirection * distance,
                Space.Self
            );

            return distance;
        }

        protected virtual void UpdateLifetime(float deltaDistance)
        {
            if (m_Config.Lifetime > 0f)
            {
                m_CurrentLifeTimer += Time.deltaTime;
                if (m_CurrentLifeTimer >= m_Config.Lifetime)
                {
                    DisableAmmo();
                    return;
                }
            }

            if (m_Config.MaxDistance > 0f)
            {
                m_TraveledDistance += deltaDistance;
                if (m_TraveledDistance >= m_Config.MaxDistance)
                {
                    DisableAmmo();
                }
            }
        }

        protected virtual bool IsValidHit(GameObject target)
        {
            return (m_Config.TargetLayer.value & (1 << target.layer)) != 0;
        }

        protected virtual void DisableAmmo()
        {
            m_CanMove = false;
            gameObject.SetActive(false);

            if (m_Shooter != null)
            {
                m_Shooter.NotifyProjectileFinished(this);
            }
        }

        private void ResetLifetime()
        {
            m_CurrentLifeTimer = 0f;
            m_TraveledDistance = 0f;
        }

        private bool IsTargetInvalid()
        {
            if (m_Targetable == null)
                return true;

            if (!m_Targetable.IsAlive)
                return true;

            return false;
        }

        private void TryRetarget()
        {
            if (m_Shooter == null)
                return;

            Targetable newTarget = m_Shooter.GetNewTargetForAmmo();
            if (newTarget != null)
            {
                m_Targetable = newTarget;
            }
        }

        private void UpdateRotation()
        {
            if (m_Targetable == null || !m_CanMove)
                return;

            Vector3 dir = m_Targetable.transform.position - transform.position;
            if (dir.sqrMagnitude <= 0.0001f)
                return;

            float targetAngle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            float currentAngle = transform.eulerAngles.z;

            float newAngle = Mathf.MoveTowardsAngle(
                currentAngle,
                targetAngle,
                m_Config.HomingTurnSpeed * Time.deltaTime
            );

            transform.rotation = Quaternion.Euler(0f, 0f, newAngle);
        }

        private void FacingAtFirstTarget2D(Targetable targetable)
        {
            Targetable.LookAtFirstTarget2D(transform, targetable);
        }
    }
}
