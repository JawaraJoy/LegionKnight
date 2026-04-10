
using Rush;
using UnityEngine;

namespace LegionKnight
{
    public partial class PlayerAgent : MonoBehaviour
    {
        public void Init()
        {
            Player.Instance.Init();
        }
        public void JumpPress()
        {
            PlayerJump jump = RushPlayer.Instance.Jump;
            jump.JumpPress();
        }
        public void JumpUnPress()
        {
            PlayerJump jump = RushPlayer.Instance.Jump;
            jump.JumpUnPress();
        }
    }
}
