using UnityEngine;

namespace Rush
{
    public class PlayerJump : Jump
    {
        
    }
    public partial class Player
    {
        [SerializeField]
        private PlayerJump m_Jump;
        public PlayerJump Jump => m_Jump;
    }
}
