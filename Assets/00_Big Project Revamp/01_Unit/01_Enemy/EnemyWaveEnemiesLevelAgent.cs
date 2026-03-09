using UnityEngine;

namespace Rush
{
    public class EnemyWaveEnemiesLevelAgent : MonoBehaviour
    {
        private EnemyWaveHandler m_EnemyWaveHandler;

        private EnemyWaveHandler EnemyWaveHandler
        {
            get
            {
                if (m_EnemyWaveHandler == null)
                {
                    m_EnemyWaveHandler = RushGameManager.Instance.StageManager.EnemyWaveHandler;
                }
                return m_EnemyWaveHandler;
            }
        }

        public void AddLevel(int amount)
        {
            foreach (Unit spawner in EnemyWaveHandler.GetActiveEnemies())
            {
                spawner.Progression.AddLevel(amount);
            }
        }
        public void SetLevel(int level)
        {
            foreach (Unit spawner in EnemyWaveHandler.GetActiveEnemies())
            {
                spawner.Progression.SetLevel(level);
            }
        }
    }
}
