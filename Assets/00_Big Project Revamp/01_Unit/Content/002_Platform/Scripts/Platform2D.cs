using MoreMountains.Tools;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

namespace Rush
{
    public class Platform2D : MonoBehaviour, IUpdater, ISkill
    {
        [SerializeField, MMReadOnly]
        private PlatformConfig m_PlatformConfig;
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
        
        [SerializeField]
        private PlatformDirection m_Direction;
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
        public UnityEvent OnReachDestination => m_OnReachDestination;
        public PlatformConfig PlatformConfig => m_PlatformConfig;
        public SkillContext SkillContext => m_SkillContext;
        public SkillConfig SkillConfig => m_PlatformConfig;
        public Transform TouchDownSpot => m_TouchDownSpot;
        public Transform Pivot => m_Pivot;
        public TouchDownCheckField TouchDownCheck => m_TouchDownCheck;
        public ProgressField Progression => m_Progression;

        public bool IsActive => gameObject.activeInHierarchy;

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
        public void Init(SkillConfig config, IModuleContext moduleContext)
        {
            if (config is PlatformConfig platformConfig)
            {
                m_PlatformConfig = platformConfig;

                m_SkillContext = new SkillContext(this, moduleContext);
                GameObject module = m_SkillContext.ModuleContext.Module;
                if (module.TryGetComponent(out SkillController skillController))
                {
                    SkillConfig[] skillConfigs = PlatformUtility.GetPlatformSkillConfigs(m_PlatformConfig).ToArray();
                    skillController.AddNewSkills(skillConfigs);

                    Skill platformSkill = skillController.GetSkillActivator(m_PlatformConfig);

                    if (platformSkill.HasAbility(platformSkill.SkillConfig.AbilitySets[0].BaseInfo.Id, out AbilityDeliver abilityDeliver))
                    {
                        m_PlatformAttack.Init(abilityDeliver.AbilityContext);
                    }
                }
            }
            else
            {
                return;
            }

            RefreshInternal();
        }
        private bool IsReachedDestination()
        {
            switch (m_Direction)
            {
                case PlatformDirection.Right:
                    return transform.position.x >= m_FinalDestination.x;

                case PlatformDirection.Left:
                    return transform.position.x <= m_FinalDestination.x;
            }

            return false;
        }

        public void StartMove(Vector2 startPost)
        {
            if (m_PlatformConfig ==  null) return;
            SetIsPausedInternal(true);
            transform.position = startPost;
            RefreshInternal();
        }
        private void StopMove()
        {
            SetIsPausedInternal(false);
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
                        contactPoint.x - m_OffSiteReachHorizontalPost,
                        m_Pivot.position.y);

                    m_FinalSpeed = -Mathf.Abs(m_FinalSpeed);
                    break;

                case PlatformDirection.Right:
                    // Spawn di kiri → bergerak ke kanan
                    m_FinalDestination = new Vector2(
                        contactPoint.x + m_OffSiteReachHorizontalPost,
                        m_Pivot.position.y);

                    m_FinalSpeed = Mathf.Abs(m_FinalSpeed);
                    break;
            }
        }


        public void Tick()
        {
            bool isPaused = RushGameManager.Instance.StageManager.PlatformHandler.IsPaused;

            if (isPaused || m_IsPaused)
                return;

            if (IsReachedDestination())
            {
                
                OnReachDestinationInvoke();
                return;
            }

            Vector3 move = m_FinalSpeed * Time.deltaTime * Vector3.right;
            transform.Translate(move);
        }

        private void SetIsPausedInternal(bool isPaused)
        {
            m_IsPaused = isPaused;
        }
        public void SetIsPaused(bool isPaused)
        {
            SetIsPausedInternal(isPaused);
        }

        private void OnReachDestinationInvoke()
        {
            StopMove();
            
            m_OnReachDestination?.Invoke();
        }

        public void ForceActivateAll()
        {
            m_SkillContext.Skill.ForceActivateAll();
        }

        public void ForceActivate(AbilityConfig config)
        {
            m_SkillContext.Skill.ForceActivate(config);
        }
    }
}
