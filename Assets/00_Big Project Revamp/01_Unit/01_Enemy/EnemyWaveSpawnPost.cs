using MoreMountains.Tools;
using UnityEngine;

namespace Rush
{
    public class EnemyWaveSpawnPost : MonoBehaviour, ILateUpdater
    {
        [SerializeField]
        private Transform m_PostToSpawn;

        [SerializeField, MMReadOnly]
        private Transform m_PostToFollow;

        [SerializeField]
        private float m_SmoothTime = 0.2f;

        [SerializeField]
        private float m_FollowDelay = 1f; // Delay after target moves

        private Vector3 m_Velocity;
        private Vector3 m_LastTargetPosition;

        private float m_DelayTimer;
        private bool m_IsWaitingForDelay;

        public bool IsActive => m_PostToFollow != null;
        public Transform PostToSpawn => m_PostToSpawn;

        private void OnEnable()
        {
            UpdateBank.Instance.RegisterLateUpdateTick(gameObject, this);

            EnemyWaveHandler waveHandler = RushGameManager.Instance.StageManager.EnemyWaveHandler;
            if (waveHandler != null)
            {
                waveHandler.SetEnemyWavePost(this);
            }
        }

        private void OnDisable()
        {
            UpdateBank.Instance.UnregisterLateUpdateTick(gameObject);
        }

        private void Start()
        {
            m_PostToFollow = RushPlayer.Instance.EnemySpawnPost;

            if (m_PostToFollow != null)
                m_LastTargetPosition = m_PostToFollow.position;
        }

        public void LateTick()
        {
            if (m_PostToFollow == null || m_PostToSpawn == null)
                return;

            Vector3 currentTargetPos = m_PostToFollow.position;

            // Detect position change
            if (currentTargetPos != m_LastTargetPosition)
            {
                m_LastTargetPosition = currentTargetPos;
                m_DelayTimer = 0f;
                m_IsWaitingForDelay = true;
            }

            // Handle delay
            if (m_IsWaitingForDelay)
            {
                m_DelayTimer += Time.deltaTime;

                if (m_DelayTimer >= m_FollowDelay)
                {
                    m_IsWaitingForDelay = false;
                }
                else
                {
                    return; // Still waiting
                }
            }

            // Follow after delay
            m_PostToSpawn.position = Vector3.SmoothDamp(
                m_PostToSpawn.position,
                currentTargetPos,
                ref m_Velocity,
                m_SmoothTime
            );
        }
    }
}