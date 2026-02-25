using Rush;
using UnityEngine;

namespace LegionKnight
{
    public class GameOverPreviewLootMonitorAgent : MonoBehaviour
    {
        public virtual void AddLootView(LootField loot)
        {
            CanvasManager.Instance.GetGameOverPreviewLootMonitor().AddLootView(loot);
        }
    }
}
