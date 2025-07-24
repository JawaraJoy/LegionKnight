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
    }
}
