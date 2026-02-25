using UnityEngine;

namespace Rush
{
    public class RushPlayerExtenstion_BossSpawnPost
    {
        
    }
    public partial class RushPlayer
    {
        [SerializeField]
        private Transform m_EnemySpawnPost;
        public Transform EnemySpawnPost => m_EnemySpawnPost;
    }
}
