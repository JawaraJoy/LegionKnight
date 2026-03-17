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
        private bool m_CanMove = false;

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
        private float m_SpawnTime;
        private float m_HomingTimer;
        private Vector3 m_StartPosition;

        public bool IsActive => gameObject.activeSelf;

        private HashSet<ITargetable> m_HitTargets = new();

        // -------------------------------------------------------------------------
        // Unity Lifecycle
        // -------------------------------------------------------------------------

        private void OnEnable()
        {
            // Hanya register ke UpdateBank — semua reset state dilakukan di OnSpawnFromPool
            UpdateBank.Instance.RegisterUpdateTick(gameObject, this);
        }

        private void OnDisable()
        {
            UpdateBank.Instance.UnregisterUpdateTick(gameObject);
        }

        // -------------------------------------------------------------------------
        // Init
        // -------------------------------------------------------------------------

        public virtual void Init(AbilityContext context, AmmoConfig config)
        {
            m_AbilityContext = context;
            m_Config = config;

            if (context.AbilityDeliver is Shooter shooter)
                m_Shooter = shooter;

            CacheMoveDirection();
        }

        // -------------------------------------------------------------------------
        // Pool
        // -------------------------------------------------------------------------

        /// <summary>
        /// Dipanggil oleh Shooter.GetFromPool() setelah SetActive(true).
        /// Reset semua state runtime supaya ammo bersih saat dipakai ulang.
        /// m_CanMove sengaja false sampai Shot() dipanggil.
        /// </summary>
        public virtual void OnSpawnFromPool()
        {
            m_HitTargets.Clear();
            m_CurrentLifeTimer = 0f;
            m_TraveledDistance = 0f;
            m_HomingTimer = 0f;
            m_CanMove = false;
            m_Targetable = null;
            m_SpawnTime = Time.time;
            m_StartPosition = transform.position;
        }

        // -------------------------------------------------------------------------
        // Shot
        // -------------------------------------------------------------------------

        public virtual void Shot(ITargetable targetable)
        {
            m_Targetable = targetable;
            m_CanMove = true; // baru boleh gerak setelah Shot() dipanggil

            m_OnShot?.Invoke(m_AbilityContext);

            Unit unitTaker = m_AbilityContext.SkillContext.ModuleContext.Unit;
            if (unitTaker.HasBind(out SkillController hasSkill))
            {
                AbilityUltility.OnSkillEventActivates(hasSkill, ForceActiveState.OnDeclareAttack);
            }

            if (m_Config.LookAtTargetOnShot)
            {
                FaceTargetInstant(targetable);
            }
            else if (m_Config.TargetingDistributeMode == TargetingMode.Homing)
            {
                FaceTargetInstant(targetable);
            }

            if (m_Config.InitialWanderAngle > 0)
            {
                float randomAngle = Random.Range(
                    -m_Config.InitialWanderAngle,
                    m_Config.InitialWanderAngle
                );
                transform.Rotate(0, 0, randomAngle);
            }
        }

        // -------------------------------------------------------------------------
        // Tick
        // -------------------------------------------------------------------------

        public virtual void Tick()
        {
            if (!m_CanMove)
                return;

            m_HomingTimer += Time.deltaTime;

            if (m_Config.TargetingDistributeMode == TargetingMode.Homing &&
                m_HomingTimer >= m_Config.HomingDelay)
            {
                if (IsTargetInvalid())
                    TryRetarget();

                UpdateRotation();
            }

            float deltaDistance = Move();
            UpdateLifetime(deltaDistance);
        }

        // -------------------------------------------------------------------------
        // Movement
        // -------------------------------------------------------------------------

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
            Vector3 move = m_MoveDirection * distance;

            if (m_Config.SwayAmplitude > 0)
            {
                float sway = Mathf.Sin(Time.time * m_Config.SwayFrequency) * m_Config.SwayAmplitude;
                move += sway * Time.deltaTime * transform.right;
            }

            transform.Translate(move, Space.Self);

            if (m_Config.ArcHeight > 0)
            {
                float progress = m_TraveledDistance / Mathf.Max(1f, m_Config.MaxDistance);
                float arc = Mathf.Sin(progress * Mathf.PI) * m_Config.ArcHeight;
                Vector3 pos = transform.position;
                pos.y += arc * Time.deltaTime;
                transform.position = pos;
            }

            return distance;
        }

        private void UpdateRotation()
        {
            if (m_Targetable == null)
                return;

            Vector2 dir = m_Targetable.TargetTransform.position - transform.position;
            if (dir.sqrMagnitude <= 0.0001f)
                return;

            float targetAngle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg + SPRITE_ANGLE_OFFSET;
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

        // -------------------------------------------------------------------------
        // Lifetime
        // -------------------------------------------------------------------------

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
            m_Shooter.NotifyProjectileFinished(this);
        }

        // -------------------------------------------------------------------------
        // Targeting helpers
        // -------------------------------------------------------------------------

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