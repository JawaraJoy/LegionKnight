using UnityEngine;

namespace Rush
{
    [CreateAssetMenu(fileName = "Game Config", menuName = "Rush/GameConfig")]
    public partial class GameConfig : Configuration
    {
        [SerializeField]
        private PhysicsMode m_PhysicMode = PhysicsMode.Physics2D;
        [SerializeField]
        private Vector3 m_StartPosition = Vector3.zero;

        public PhysicsMode PhysicsMode => m_PhysicMode;
        public Vector3 StartPosition => m_StartPosition;
    }
    public enum PhysicsMode
    {
        Physics3D,
        Physics2D
    }
}
