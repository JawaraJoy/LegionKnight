using UnityEngine;

namespace LegionKnight
{
    public class PlayerSpine : CharacterSpine
    {
        
    }

    public partial class Player
    {
        [SerializeField]
        private PlayerSpine m_PlayerSpine;

        public void ChangeSpine(CharacterDefinition defi)
        {
            m_PlayerSpine.ChangeSpine(defi);
        }
        public void PlayJump()
        {
            m_PlayerSpine.PlayJump();
        }
        public void PlayIdle()
        {
            m_PlayerSpine.PlayIdle();
        }
        public void PlayAttack()
        {
            m_PlayerSpine.PlayAttack();
        }
        public void PlayDeath()
        {
            m_PlayerSpine.PlayDeath();
        }
        public void FlipX(bool left)
        {
            m_PlayerSpine.FlipX(left);
        }
        public void SetAnim(SpineAnimDefinition anim)
        {
            m_PlayerSpine.SetAnim(anim);
        }
    }
}
