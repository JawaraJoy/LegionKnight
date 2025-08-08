using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace LegionKnight
{
    public class EnemyManager : EnemyController
    {
        
    }

    public partial class GameManager
    {
        [SerializeField]
        private EnemyManager m_EnemyManager;
        public void AddEnemy(IEnemy enemy)
        {
            m_EnemyManager.AddEnemy(enemy);
        }
        public void RemoveEnemy(IEnemy enemy)
        {
            m_EnemyManager.AddEnemy(enemy);
        }
        public void SetSpawningSpot(Transform spot)
        {
            m_EnemyManager.SetSpawningSpot(spot);
        }
        public void SpawnMinion(MinionDefinition defi)
        {
            m_EnemyManager.SpawnMinion(defi);
        }
        public void SetCanSpawnEnemy(bool can)
        {
            m_EnemyManager.SetCanSpawnEnemy(can);
        }
    }
}
