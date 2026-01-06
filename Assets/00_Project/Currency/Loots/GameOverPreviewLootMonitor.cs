using UnityEngine;

namespace LegionKnight
{
    public class GameOverPreviewLootMonitor : PreviewLootMonitor
    {

    }
    public partial class CanvasManager
    {
        public GameOverPreviewLootMonitor GetGameOverPreviewLootMonitor()
        {
            GameOverPanel gameplayPanel = GetPanelInternal<GameOverPanel>();
            return gameplayPanel.GetBinding<GameOverPreviewLootMonitor>();
        }
    }
}
