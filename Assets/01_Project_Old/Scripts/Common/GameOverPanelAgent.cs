using UnityEngine;

namespace LegionKnight
{
    public partial class GameOverPanelAgent : MonoBehaviour
    {
        public void ShowGameOverPanel()
        {
            CanvasManager.Instance.ShowPanel(PanelId.GameOverPanelId);
        }
        public void HideGameOverPanel()
        {
            CanvasManager.Instance.HidePanel(PanelId.GameOverPanelId);
        }
    }
}
