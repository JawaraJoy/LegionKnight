using UnityEngine;

namespace LegionKnight
{
    public class BossSpawningMinionAgent : MonoBehaviour
    {
        private BosEnemy m_BosEnemy;

        private BosEnemy BosEnemy
        {
            get
            {
                if (m_BosEnemy == null)
                {
                    m_BosEnemy = GameManager.Instance.GetSpawnedBosEnemy();
                }
                return m_BosEnemy;
            }
        }

        private SpawnMinionAbility m_BossSpawningMinion;

        private SpawnMinionAbility BossSpawningMinionAbility
        {
            get
            {
                if (m_BossSpawningMinion == null)
                {
                    if (BosEnemy.BossSpine.CurrentSpineObject.TryGetComponent(out SpawnMinionAbility ability))
                    {
                        m_BossSpawningMinion = ability;
                    }
                }
                return m_BossSpawningMinion;
            }
        }
        private EnemyManager m_EnemyManager;

        private EnemyManager EnemyManager
        {
            get
            {
                if (m_EnemyManager == null)
                {
                    m_EnemyManager = GameManager.Instance.EnemyManager;
                }
                return m_EnemyManager;
            }
        }
        public void TriggerSpawnMinion()
        {
            EnemyManager.SetCanSpawnEnemy(true);
            if (BossSpawningMinionAbility != null)
            {
                BossSpawningMinionAbility.CustomSpawnMinions();
                Debug.Log("Boss triggered spawning minions.");
            }
            else
            {
                Debug.LogWarning("BossSpawningMinion ability is not assigned.");
            }
            EnemyManager.SetCanSpawnEnemy(false);
            Debug.Log("Boss spawn minion triggered");
        }
    }
}
