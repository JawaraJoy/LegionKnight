using UnityEngine;

namespace Rush
{
    public class PlayerJump : Jump
    {
        
    }
    public partial class RushPlayer
    {
        [SerializeField]
        private PlayerJump m_Jump;
        public PlayerJump Jump => m_Jump;
    }
}
