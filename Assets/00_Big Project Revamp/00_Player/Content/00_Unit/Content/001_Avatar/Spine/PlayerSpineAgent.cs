using UnityEngine;

namespace LegionKnight
{
    public class PlayerSpineAgent : MonoBehaviour
    {
        public void ChangeSpine(CharacterDefinition defi)
        {
            Player.Instance.ChangeSpine(defi);
        }
        public void PlayJump()
        {
            Player.Instance.PlayJump();
        }
        public void PlayIdle()
        {
            Player.Instance.PlayIdle();
        }
        public void PlayAttack()
        {
            Player.Instance.PlayAttack();
        }
        public void PlayDeath()
        {
            Player.Instance.PlayDeath();
        }
        public void FlipX(bool left)
        {
            Player.Instance.FlipX(left);
        }
        public void SetAnim(SpineAnimDefinition anim)
        {
            Player.Instance.SetAnim(anim);
        }
    }
}
