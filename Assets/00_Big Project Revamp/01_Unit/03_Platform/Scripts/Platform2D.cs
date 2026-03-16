using MoreMountains.Tools;
using UnityEngine;
using UnityEngine.Events;

namespace Rush
{
    public class Platform2D : MonoBehaviour, IUpdater, ISkill
    {
        [SerializeField, MMReadOnly]
        private PlatformConfig m_PlatformConfig;
        [SerializeField]
        private ParticleSystem m_PersonalityVFX;
        [SerializeField]
        private SpriteRenderer m_SpriteView;
        [SerializeField]
        private PlatformAttack m_PlatformAttack;
        [SerializeField]
        private Transform m_TouchDownSpot;
        [SerializeField]
        private Transform m_Pivot;
        [SerializeField]
        private bool m_IsPaused = false;
        [SerializeField]
        private ProgressField m_Progression;
        [SerializeField]
        private TouchDownCheckField m_TouchDownCheck;

        [SerializeField, MMReadOnly]
        private PlatformDirection m_Direction = PlatformDirection.Left;
        [SerializeField, MMReadOnly]
        private SkillContext m_SkillContext;
        [SerializeField, MMReadOnly]
        private Vector2 m_FinalDestination;
        [SerializeField, MMReadOnly]
        private float m_OffSiteReachHorizontalPost;
        [SerializeField, MMReadOnly]
        private float m_FinalSpeed = 1.0f;
        [SerializeField]
        private UnityEvent m_OnReachDestination;
        [SerializeField]
        private UnityEvent<Unit> m_OnAbilityDelivered;

        [Header("Boost")]
        [SerializeField]
        private UnityEvent<float, int> m_OnBoostStart; // (duration, perfectComboCount)
        [SerializeField]
        private UnityEvent m_OnBoostEnd;
        [SerializeField]
        private UnityEvent m_OnBoostTick; // Di-invoke tiap 1 detik selama boosting

        public UnityEvent<float, int> OnBoostStart => m_OnBoostStart;
        public UnityEvent OnBoostEnd => m_OnBoostEnd;
        public UnityEvent OnBoostTick => m_OnBoostTick;

        public bool IsBoosting => m_IsBoosting;

        // --- Boost State ---
        private enum BoostState { Idle, Boosting, PostBoostDelay }
        private BoostState m_BoostState = BoostState.Idle;
        private bool m_IsBoosting;
        private PlatformBoostField m_ActiveBoostField;
        private float m_ActiveBoostDuration;
        private float m_BoostElapsed;
        private float m_BoostNextTickTime;
        private float m_PostBoostElapsed;
        private Collider2D m_PlatformCollider;
        private Collider2D m_PlayerCollider;
        private bool m_DidIgnoreCollision;
        public UnityEvent OnReachDestination => m_OnReachDestination;
        public PlatformConfig PlatformConfig => m_PlatformConfig;
        public SkillContext SkillContext => m_SkillContext;
        public SkillConfig SkillConfig => m_PlatformConfig.AttackSkill;
        public Transform TouchDownSpot => m_TouchDownSpot;
        public Transform Pivot => m_Pivot;
        public TouchDownCheckField TouchDownCheck => m_TouchDownCheck;
        public ProgressField Progression => m_Progression;
        public PlatformDirection Direction => m_Direction;
        public bool IsActive => gameObject.activeInHierarchy;

        private bool m_ReachedDestination;

        public UnityEvent<Unit> OnAbilityDelivered
        {
            get
            {
                return m_OnAbilityDelivered;
            }
        }

        private void OnEnable()
        {
            UpdateBank.Instance.RegisterUpdateTick(gameObject, this);
        }

        private void OnDisable()
        {
            UpdateBank.Instance.UnregisterUpdateTick(gameObject);
        }

        // initialize first
        public void IniPlatform(PlatformConfig platformConfig)
        {
            m_PlatformConfig = platformConfig;
            if (m_PersonalityVFX != null)
            {
                var main = m_PersonalityVFX.main;
                main.startColor = m_PlatformConfig.PersonalityColor;
            }
            if (m_SpriteView != null)
            {
                m_SpriteView.sprite = m_PlatformConfig.CollectibleField.SplashImage;
            }
        }
        // initialize second
        public void Init(SkillConfig config, IModuleContext moduleContext)
        {
            if (m_PlatformConfig == null) return;
            m_SkillContext = new SkillContext(this, moduleContext);
            Unit unit = m_SkillContext.ModuleContext.Unit;
            if (unit.HasBind(out SkillController skillController))
            {
                SkillConfig[] skillConfigs = PlatformUtility.GetPlatformSkillConfigs(m_PlatformConfig).ToArray();
                skillController.AddNewSkillsWithoutAddLevel(skillConfigs);
                Debug.Log("Skill platform Added");
                Skill attackSkill = skillController.GetSkillActivator(m_PlatformConfig.AttackSkill);
                if (attackSkill.HasAbility(attackSkill.SkillConfig.AbilitySets[0].BaseInfo.Id, out AbilityDeliver abilityDeliver))
                {
                    m_PlatformAttack.Init(abilityDeliver.AbilityContext);
                }
            }
            RefreshInternal();
        }
        private bool IsReachedDestination()
        {
            switch (m_Direction)
            {
                case PlatformDirection.Right:
                    return transform.position.x >= m_FinalDestination.x || m_ReachedDestination;

                case PlatformDirection.Left:
                    return transform.position.x <= m_FinalDestination.x || m_ReachedDestination;
            }

            return false;
        }

        public void StartMove(Vector2 startPost)
        {
            if (m_PlatformConfig == null) return;
            m_ReachedDestination = false;
            SetIsPausedInternal(false);
            transform.position = startPost;

            RefreshInternal();
        }
        public void StopMove()
        {
            StopMoveInternal();
        }
        private void StopMoveInternal()
        {
            SetIsPausedInternal(true);
        }
        private void RefreshInternal()
        {
            if (m_PlatformConfig == null) return;

            SetIsPausedInternal(false);

            float minSpeedRate = RushGameManager.Instance.StageManager.PlatformHandler.MinGlobalSpeedRate;
            float maxSpeedRate = RushGameManager.Instance.StageManager.PlatformHandler.MaxGlobalSpeedRate;
            float randomSpeedRate = Random.Range(minSpeedRate, maxSpeedRate);

            m_FinalSpeed = m_PlatformConfig.Speed * randomSpeedRate;

            m_OffSiteReachHorizontalPost =
                RushGameManager.Instance.StageManager.PlatformHandler.Config.OffSiteReachHorizontalPost;

            Vector2 contactPoint =
                RushGameManager.Instance.StageManager.PlatformHandler.LastContactPoint;

            // Tentukan arah dari spawn ke contact point
            m_Direction = PlatformUtility.GetPlatformDirection(m_Pivot.position, contactPoint);

            switch (m_Direction)
            {
                case PlatformDirection.Left:
                    // Spawn di kanan → bergerak ke kiri
                    m_FinalDestination = new Vector2(
                        0 - m_OffSiteReachHorizontalPost,
                        m_Pivot.position.y);

                    m_FinalSpeed = -Mathf.Abs(m_FinalSpeed);
                    break;

                case PlatformDirection.Right:
                    // Spawn di kiri → bergerak ke kanan
                    m_FinalDestination = new Vector2(
                        0 + m_OffSiteReachHorizontalPost,
                        m_Pivot.position.y);

                    m_FinalSpeed = Mathf.Abs(m_FinalSpeed);
                    break;
            }
        }


        public void Tick()
        {
            bool isPaused = RushGameManager.Instance.StageManager.PlatformHandler.IsPaused;

            if (isPaused)
                return;

            TickBoost();

            // Gerakan horizontal hanya jalan kalau tidak sedang boost
            if (m_IsBoosting || m_IsPaused)
                return;

            if (IsReachedDestination())
            {
                OnReachDestinationInvokeInternal();
                return;
            }

            Vector3 move = m_FinalSpeed * Time.deltaTime * Vector3.right;
            transform.Translate(move);
        }

        private void TickBoost()
        {
            switch (m_BoostState)
            {
                case BoostState.Boosting:
                    TickBoosting();
                    break;

                case BoostState.PostBoostDelay:
                    TickPostBoostDelay();
                    break;
            }
        }

        private void TickBoosting()
        {
            m_BoostElapsed += Time.deltaTime;

            transform.Translate(Vector3.up * m_ActiveBoostField.BoostSpeed * Time.deltaTime);

            // OnBoostTick tiap 1 detik
            if (m_BoostElapsed >= m_BoostNextTickTime)
            {
                m_OnBoostTick?.Invoke();
                m_BoostNextTickTime += 1f;
            }

            if (m_BoostElapsed >= m_ActiveBoostDuration)
            {
                EndBoosting();
            }
        }

        private void EndBoosting()
        {
            // Restore collision
            if (m_DidIgnoreCollision)
            {
                Physics2D.IgnoreCollision(m_PlatformCollider, m_PlayerCollider, false);
                m_DidIgnoreCollision = false;
            }

            // Update contact point ke posisi baru setelah naik
            PlatformHandler handler = RushGameManager.Instance.StageManager.PlatformHandler;
            handler.SetLastContactPoint(m_TouchDownSpot.position);

            FinishBoost();

            // Mulai post-boost delay
            m_PostBoostElapsed = 0f;
            m_BoostState = BoostState.PostBoostDelay;
        }

        private void TickPostBoostDelay()
        {
            m_PostBoostElapsed += Time.deltaTime;

            if (m_PostBoostElapsed >= m_ActiveBoostField.PostBoostSpawnDelay)
            {
                PlatformHandler handler = RushGameManager.Instance.StageManager.PlatformHandler;
                handler.SetIsBoostActive(false);
                handler.SpawnNextPlatformFromWaitingList(handler.Config.NextSpawnDelay);

                m_BoostState = BoostState.Idle;
                m_ActiveBoostField = null;
            }
        }

        private void SetIsPausedInternal(bool isPaused)
        {
            m_IsPaused = isPaused;
        }
        public void SetIsPaused(bool isPaused)
        {
            SetIsPausedInternal(isPaused);
        }

        public void OnReachDestinationInvoke()
        {
            OnReachDestinationInvokeInternal();
        }

        private void OnReachDestinationInvokeInternal()
        {
            if (m_ReachedDestination)
                return;

            m_ReachedDestination = true;

            StopMoveInternal();
            m_OnReachDestination?.Invoke();
        }

        public void ForceActivateAll()
        {

        }

        public void ForceActivate(AbilityConfig config)
        {

        }

        /// <summary>
        /// Panggil method ini dari event manapun di luar class ini
        /// untuk mengaktifkan boost pada platform.
        /// </summary>
        public void Boost(PlatformBoostField boostField, float duration, int comboCount)
        {
            if (boostField == null)
            {
                Debug.LogWarning("[Platform2D] BoostField is null, boost dibatalkan.");
                return;
            }

            if (m_IsBoosting)
                return;

            m_ActiveBoostField = boostField;
            m_ActiveBoostDuration = duration;
            m_IsBoosting = true;
            m_BoostElapsed = 0f;
            m_BoostNextTickTime = 1f;
            m_BoostState = BoostState.Boosting;

            StopMove();

            // Ignore collision dengan player selama boost
            m_PlatformCollider = GetComponent<Collider2D>();
            m_PlayerCollider = RushPlayer.Instance.GetComponent<Collider2D>();
            m_DidIgnoreCollision = m_PlatformCollider != null && m_PlayerCollider != null;
            if (m_DidIgnoreCollision)
                Physics2D.IgnoreCollision(m_PlatformCollider, m_PlayerCollider, true);

            PlatformHandler handler = RushGameManager.Instance.StageManager.PlatformHandler;
            handler.SetIsBoostActive(true);

            m_OnBoostStart?.Invoke(duration, comboCount);
        }

        /// <summary>
        /// Hentikan boost paksa dari luar jika diperlukan.
        /// </summary>
        public void StopBoost()
        {
            if (!m_IsBoosting) return;

            if (m_DidIgnoreCollision)
            {
                Physics2D.IgnoreCollision(m_PlatformCollider, m_PlayerCollider, false);
                m_DidIgnoreCollision = false;
            }

            m_BoostState = BoostState.Idle;
            m_ActiveBoostField = null;
            FinishBoost();
        }

        private void FinishBoost()
        {
            m_IsBoosting = false;
            m_OnBoostEnd?.Invoke();
        }
    }
}