using Rush;
using UnityEngine;

namespace LegionKnight
{
    public class WinPanelAgent : MonoBehaviour
    {
        public void OpenWinPanel()
        {
            StageConfig stageConfig = RushGameManager.Instance.StageManager.UsedStageConfig;
            bool Isfinite = stageConfig.StageMode == StageMode.Adventure;
            if (Isfinite)
            {
                WinPanel winPanel = CanvasManager.Instance.GetPanel<WinPanel>();
                winPanel.Show();
            }
        }
    }
}
