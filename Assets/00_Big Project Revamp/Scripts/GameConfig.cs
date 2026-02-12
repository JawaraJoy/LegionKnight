using UnityEngine;

namespace Rush
{
    [CreateAssetMenu(fileName = "Game Config", menuName = "Rush/GameConfig")]
    public partial class GameConfig : Configuration
    {
        [SerializeField]
        private PhysicsMode m_PhysicMode = PhysicsMode.Physics2D;
        [SerializeField]
        private LayerMask m_PlayerLayer;
        [SerializeField]
        private LayerMask m_EnemyLayer;

        public PhysicsMode PhysicsMode => m_PhysicMode;
        public LayerMask PlayerLayer => m_PlayerLayer;
        public LayerMask EnemyLayer => m_EnemyLayer;
    }
    public enum PhysicsMode
    {
        Physics3D,
        Physics2D
    }
}
