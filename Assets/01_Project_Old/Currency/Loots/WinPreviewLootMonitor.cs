using System.Collections.Generic;
using UnityEngine;

namespace LegionKnight
{
    public class WinPreviewLootMonitor : PreviewLootMonitor
    {
        
    }
    public partial class CanvasManager
    {
        public WinPreviewLootMonitor GetWinPreviewLootMonitor()
        {
            WinPanel gameplayPanel = GetPanelInternal<WinPanel>();
            return gameplayPanel.GetBinding<WinPreviewLootMonitor>();
        }
    }
}
