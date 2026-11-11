using UnityEngine;

namespace LegionKnight
{
    public partial class GameOverPanelAgent : MonoBehaviour
    {
        public void ShowGameOverPanel()
        {
            CanvasManager.Instance.GetPanel<GameOverPanel>().Show();
        }
        public void HideGameOverPanel()
        {
            CanvasManager.Instance.GetPanel<GameOverPanel>().Hide();
        }
    }
}
