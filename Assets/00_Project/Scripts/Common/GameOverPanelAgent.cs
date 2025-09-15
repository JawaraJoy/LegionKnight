using UnityEngine;

namespace LegionKnight
{
    public partial class GameOverPanelAgent : MonoBehaviour
    {
        public void ShowGameOverPanel()
        {
            GameManager.Instance.ShowPanel(PanelId.GameOverPanelId);
        }
        public void HideGameOverPanel()
        {
            GameManager.Instance.HidePanel(PanelId.GameOverPanelId);
        }
    }
}
