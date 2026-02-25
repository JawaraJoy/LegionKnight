using UnityEngine;

namespace Rush
{
    public class EnemyWaveAgent : MonoBehaviour
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

        public void DespawnUnit(Unit unit)
        {
            EnemyWaveHandler.DespawnUnit(unit);
        }
    }
}
