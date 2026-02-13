using MoreMountains.Tools;
using UnityEngine;
using UnityEngine.Events;

namespace Rush
{
    [RequireComponent(typeof(BoxCollider2D))]
    public class Platform2D : MonoBehaviour, IUpdater
    {
        [SerializeField]
        private Transform m_TouchDownSpot;
        [SerializeField]
        private Transform m_Pivot;
        [SerializeField]
        private bool m_IsPaused = false;
        [SerializeField]
        private TouchDownCheckField m_TouchDownCheck;
        [SerializeField, MMReadOnly]
        private PlatformConfig m_Config;
        [SerializeField]
        private PlatformDirection m_Direction;
        [SerializeField, MMReadOnly]
        private PlatformContext m_Context;
        [SerializeField, MMReadOnly]
        private Vector2 m_FinalDestination;
        [SerializeField, MMReadOnly]
        private float m_OffSiteReachHorizontalPost;
        [SerializeField, MMReadOnly]
        private float m_FinalSpeed = 1.0f;
        [SerializeField]
        private UnityEvent m_OnReachDestination;
        public UnityEvent OnReachDestination => m_OnReachDestination;
        public PlatformConfig Config => m_Config;
        public PlatformContext Context => m_Context;
        public Transform TouchDownSpot => m_TouchDownSpot;
        public Transform Pivot => m_Pivot;
        public TouchDownCheckField TouchDown => m_TouchDownCheck;

        public bool IsActive => gameObject.activeInHierarchy;

        private void OnCollisionEnter2D(Collision2D collision)
        {
            LayerMask failLayer = RushGameManager.Instance.PlatformManager.Config.FailLayer;
            if (collision.gameObject.TryGetComponent(out Damageable damageable))
            {
                damageable.TakeDamage(m_Config.FailDamage);
            }
        }
        public void Init(PlatformConfig config, GameObject ownerObject)
        {
            m_Config = config;
            m_Context = new PlatformContext(this, ownerObject);
            if (ownerObject.TryGetComponent(out Unit unit))
            {
                if (unit.HasBind(out Skill skill))
                {
                    SkillActivatorConfig[] skillConfigsOnLeftNormal = m_Config.SkillOnLeftTouch.OnNormalTouchSkill;
                    SkillActivatorConfig[] skillConfigsOnRightNormal = m_Config.SkillOnRightTouch.OnNormalTouchSkill;
                    SkillActivatorConfig[] skillConfigsOnLeftPerfect = m_Config.SkillOnLeftTouch.OnPerfectTouchSkill;
                    SkillActivatorConfig[] skillConfigsOnRightPerfect = m_Config.SkillOnRightTouch.OnPerfectTouchSkill;

                    skill.AddNewSkills(skillConfigsOnLeftNormal);
                    skill.AddNewSkills(skillConfigsOnRightNormal);
                    skill.AddNewSkills(skillConfigsOnLeftPerfect);
                    skill.AddNewSkills(skillConfigsOnRightPerfect);
                }
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
            if (m_Config ==  null) return;
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
            if (m_Config == null) return;
            UpdateBank.Instance.RegisterUpdateTick(gameObject, this);
            SetIsPausedInternal(false);

            float minSpeedRate = RushGameManager.Instance.PlatformManager.MinGlobalSpeedRate;
            float maxSpeedRate = RushGameManager.Instance.PlatformManager.MaxGlobalSpeedRate;
            float randomSpeedRate = Random.Range(minSpeedRate, maxSpeedRate);

            m_FinalSpeed = m_Config.Speed * randomSpeedRate;

            m_OffSiteReachHorizontalPost =
                RushGameManager.Instance.PlatformManager.Config.OffSiteReachHorizontalPost;

            Vector2 contactPoint =
                RushGameManager.Instance.PlatformManager.LastContactPoint;

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
            bool isPaused = RushGameManager.Instance.PlatformManager.IsPaused;

            if (isPaused || m_IsPaused)
                return;

            if (IsReachedDestination())
            {
                
                OnReachDestinationInvoke();
                return;
            }

            Vector3 move = Vector3.right * m_FinalSpeed * Time.deltaTime;
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
            UpdateBank.Instance.UnregisterUpdateTick(gameObject);
            m_OnReachDestination?.Invoke();
        }
    }
}
