using UnityEngine;

namespace LegionKnight
{
    public class EnemyManagerAgent : MonoBehaviour
    {
        public void SetCanSpawnEnemy(bool can)
        {
            GameManager.Instance.SetCanSpawnEnemy(can);
        }
        public void ResetAllToCantSpawn()
        {
            GameManager.Instance.ResetAllToCantSpawn();
        }
    }
}
