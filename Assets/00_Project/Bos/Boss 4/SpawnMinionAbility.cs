using UnityEngine;

namespace LegionKnight
{
    public class SpawnMinionAbility : MonoBehaviour
    {
        [SerializeField]
        private Transform m_SpawningSpot;
        [SerializeField]
        private MinionDefinition m_MinionToSpawn;
        [SerializeField]
        private float m_Radius = 5f;
        [SerializeField]
        private int m_AmountToSpawn = 1;
        private EnemyController m_EnemyController;

        private EnemyController EnemyController
        {
            get
            {
                if (m_EnemyController == null)
                {
                    m_EnemyController = GameManager.Instance.EnemyManager;
                }
                return m_EnemyController;
            }
        }
        public void CustomSpawnMinions()
        {
            EnemyController.CustomSpawnMinions(m_MinionToSpawn, m_AmountToSpawn, m_SpawningSpot, m_Radius);
        }
    }
}
