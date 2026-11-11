using UnityEngine;

namespace LegionKnight.Deleted
{
    public partial class PlayerCharacterJump : CharacterJump
    {
        
    }

    public partial class Player
    {
        [SerializeField]
        private PlayerCharacterJump m_Jump;
        public PlayerCharacterJump Jump => m_Jump;
    }
    
}
