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
            Player.Instance.JumpPress();
        }
        public void JumpUnPress()
        {
            Player.Instance.JumpUnPress();
        }
    }
    public partial class GameManager
    {
        [SerializeField]
        private ButtonJumpCanvas m_JumpCanvas;

        public void ShowJumpCanvas()
        {
            m_JumpCanvas.Show();
        }
        public void HideJumpCanvas()
        {
            m_JumpCanvas.Hide();
        }
    }
}
