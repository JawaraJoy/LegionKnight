using UnityEngine;

namespace LegionKnight
{
    public class RevivePanel : PanelView
    {
        public override void Show()
        {
            base.Show();
            GameOverPanel gameOver = CanvasManager.Instance.GetPanel<GameOverPanel>();
            if (gameOver != null)
            {
                gameOver.Hide();
            }
        }
    }
}
