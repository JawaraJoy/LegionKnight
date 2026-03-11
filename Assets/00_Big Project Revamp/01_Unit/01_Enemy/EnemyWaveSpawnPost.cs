using MoreMountains.Tools;
using UnityEngine;

namespace Rush
{
    public class EnemyWaveSpawnPost : MonoBehaviour
    {
        [SerializeField]
        private Transform m_PostToSpawn;
        public Transform PostToSpawn => m_PostToSpawn;

        private void OnEnable()
        {
            EnemyWaveHandler waveHandler = RushGameManager.Instance.StageManager.EnemyWaveHandler;
            if (waveHandler != null)
                waveHandler.SetEnemyWavePost(this);
        }
    }
}