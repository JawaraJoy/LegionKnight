using UnityEngine;

namespace Rush
{
    public class PlayerRigidbodyModifier : RigidBodyModifier
    {
        
    }
    public partial class RushPlayer
    {
        [SerializeField]
        private PlayerRigidbodyModifier m_RigidbodyModifier;
        public PlayerRigidbodyModifier RigidbodyModifier => m_RigidbodyModifier;
    }
}
