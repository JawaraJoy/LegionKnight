using UnityEngine;

namespace LegionKnight
{
    public class GameOverPreviewLootMonitorAgent : MonoBehaviour
    {
        public virtual void AddLootView(LootField loot)
        {
            GameManager.Instance.GetGameOverPreviewLootMonitor().AddLootView(loot);
        }
    }
}
