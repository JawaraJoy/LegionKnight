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

        private Vector3 m_Velocity;

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

        public void LateTick()
        {
            if (m_PostToFollow == null || m_PostToSpawn == null)
                return;

            Vector3 targetPosition = m_PostToFollow.position;

            m_PostToSpawn.position = Vector3.SmoothDamp(m_PostToSpawn.position, targetPosition, ref m_Velocity, m_SmoothTime);
        }

        private void Start()
        {
            m_PostToFollow = RushPlayer.Instance.EnemySpawnPost;
        }
    }
}