using Rush;
using UnityEngine;

namespace LegionKnight
{
    public partial class ButtonJumpCanvas : UIView
    {
        private void Start()
        {
            HideInternal();
        }
        public void JumpPress()
        {
            RushPlayer.Instance.Jump.JumpPress();
        }
        public void JumpUnPress()
        {
            RushPlayer.Instance.Jump.JumpUnPress();
        }
    }
}
