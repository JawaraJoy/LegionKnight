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
        public void SetJumpForce(float set)
        {
            m_CharacterJump.SetJumpForce(set);
        }

        public void SetFallSpeed(float set)
        {
            m_CharacterJump.SetFallSpeed(set);
        }
        public void SetMaxJumpDistance(float set)
        {
            m_CharacterJump.SetMaxJumpDistance(set);
        }
        public void SetUseHoldJump(bool set)
        {
            m_CharacterJump.SetUseHoldJump(set);
        }

    }
    public partial class PlayerAgent
    {
        public void SetJumpForce(float set)
        {
            Player.Instance.SetJumpForce(set);
        }

        public void SetFallSpeed(float set)
        {
            Player.Instance.SetFallSpeed(set);
        }
        public void SetMaxJumpDistance(float set)
        {
            Player.Instance.SetMaxJumpDistance(set);
        }
        public void SetUseHoldJump(bool set)
        {
            Player.Instance.SetUseHoldJump(set);
        }
    }
}
