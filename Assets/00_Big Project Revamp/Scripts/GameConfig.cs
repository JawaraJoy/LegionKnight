using UnityEngine;

namespace Rush
{
    [CreateAssetMenu(fileName = "Game Config", menuName = "Rush/GameConfig")]
    public partial class GameConfig : Configuration
    {
        [SerializeField]
        private PhysicsMode m_PhysicMode = PhysicsMode.Physics2D;
        public PhysicsMode PhysicsMode => m_PhysicMode;
    }
    public enum PhysicsMode
    {
        Physics3D,
        Physics2D
    }
}
