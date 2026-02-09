using UnityEngine;

namespace LegionKnight
{
    public class WinPreviewLootMonitorAgent : MonoBehaviour
    {
        public virtual void AddLootView(LootField loot)
        {
            CanvasManager.Instance.GetWinPreviewLootMonitor().AddLootView(loot);
        }
    }
}
