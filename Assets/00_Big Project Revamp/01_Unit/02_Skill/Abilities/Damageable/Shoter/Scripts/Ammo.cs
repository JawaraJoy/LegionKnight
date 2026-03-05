using MoreMountains.Tools;
using System.Collections.Generic;
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
        protected ITargetable m_Targetable;
        protected AbilityContext m_AbilityContext;

        // Because sprite faces UP (Y axis)
        private const float SPRITE_ANGLE_OFFSET = -90f;

        public bool IsActive => gameObject.activeSelf;
        private HashSet<ITargetable> m_HitTargets = new();


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
                m_Shooter = shooter;

            CacheMoveDirection();
        }

        public virtual void Shot(ITargetable targetable)
        {
            m_Targetable = targetable;
            m_CanMove = true;

            m_OnShot?.Invoke(m_AbilityContext);

            FaceTargetInstant(targetable);
        }

        public virtual void Tick()
        {
            if (!m_CanMove)
                return;

            if (m_Config.TargetingDistributeMode == TargetingMode.Homing)
            {
                if (IsTargetInvalid())
                    TryRetarget();

                UpdateRotation();
            }

            float deltaDistance = Move();
            UpdateLifetime(deltaDistance);
        }

        protected virtual void CacheMoveDirection()
        {
            m_MoveDirection = Vector3.up; // since ForwardAxis = Y
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

        private void UpdateRotation()
        {
            if (m_Targetable == null)
                return;

            Vector2 dir = m_Targetable.TargetTransform.position - transform.position;
            if (dir.sqrMagnitude <= 0.0001f)
                return;

            float targetAngle =
                Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg + SPRITE_ANGLE_OFFSET;

            float newAngle = Mathf.MoveTowardsAngle(
                transform.eulerAngles.z,
                targetAngle,
                m_Config.HomingTurnSpeed * Time.deltaTime
            );

            transform.rotation = Quaternion.Euler(0f, 0f, newAngle);
        }

        private void FaceTargetInstant(ITargetable targetable)
        {
            if (targetable == null) 
                return;

            Vector2 dir = targetable.TargetTransform.position - transform.position;
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg + SPRITE_ANGLE_OFFSET;

            transform.rotation = Quaternion.Euler(0f, 0f, angle);
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
                    DisableAmmo();
            }
        }

        protected virtual void DisableAmmo()
        {
            m_CanMove = false;
            gameObject.SetActive(false);
            m_Shooter?.NotifyProjectileFinished(this);
        }

        private void ResetLifetime()
        {
            m_CurrentLifeTimer = 0f;
            m_TraveledDistance = 0f;
        }

        private bool IsTargetInvalid()
        {
            return m_Targetable == null || !m_Targetable.IsAlive;
        }

        private void TryRetarget()
        {
            if (m_Shooter == null)
                return;

            ITargetable newTarget = m_Shooter.GetNewTargetForAmmo();
            if (newTarget != null)
                m_Targetable = newTarget;
        }
        public virtual void OnSpawnFromPool()
        {
            m_HitTargets.Clear();
        }
        protected virtual bool IsValidHit(GameObject other)
        {
            if (!other.TryGetComponent(out ITargetable target))
                return false;

            if (m_HitTargets.Contains(target))
                return false;

            AbilityConfig abilityConfig = m_AbilityContext.AbilityDeliver.AbilityConfig;

            if (!abilityConfig.CanTargetDeathUnit && !target.IsAlive)
                return false;

            if (!AbilityUltility.IsTargetAllowedByTargetObject(
                    m_AbilityContext.AbilityDeliver,
                    target))
                return false;

            m_HitTargets.Add(target);

            return true;
        }
    }
}