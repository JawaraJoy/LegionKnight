
using UnityEngine;

namespace Rush
{
    public class EnemyWaveSpawnPost : MonoBehaviour, IReseter
    {
        [SerializeField]
        private Vector3 m_InitialPosition;
        [SerializeField]
        private Transform m_PostToSpawn;
        public Transform PostToSpawn => m_PostToSpawn;

        private void Start()
        {
            transform.position = m_InitialPosition;
        }
        public void ResetProgression()
        {
            transform.position = m_InitialPosition;
        }

        private void OnEnable()
        {
            EnemyWaveHandler waveHandler = RushGameManager.Instance.StageManager.EnemyWaveHandler;
            if (waveHandler != null)
                waveHandler.SetEnemyWavePost(this);
        }
    }
}