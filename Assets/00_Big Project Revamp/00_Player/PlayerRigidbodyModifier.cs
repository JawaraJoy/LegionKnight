using UnityEngine;

namespace Rush
{
    public class PlayerRigidbodyModifier : RigidBodyModifier
    {
        
    }
    public partial class Player
    {
        [SerializeField]
        private PlayerRigidbodyModifier m_RigidbodyModifier;
        public PlayerRigidbodyModifier RigidbodyModifier => m_RigidbodyModifier;
    }
}
