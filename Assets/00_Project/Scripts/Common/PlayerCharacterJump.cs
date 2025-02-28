using UnityEngine;

namespace LegionKnight
{
    public partial class PlayerCharacterJump : CharacterJump
    {
        
    }

    public partial class Player
    {
        [SerializeField]
        private PlayerCharacterJump m_CharacterJump;

        public void JumpPress()
        {
            m_CharacterJump.JumpPress();
        }
        public void JumpUnPress()
        {
            m_CharacterJump.JumpUnPress();
        }
    }
}
