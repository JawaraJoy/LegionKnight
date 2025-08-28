using UnityEngine;

namespace LegionKnight
{
    public class WinPreviewLootMonitorAgent : MonoBehaviour
    {
        public virtual void AddLootView(LootField loot)
        {
            GameManager.Instance.GetWinPreviewLootMonitor().AddLootView(loot);
        }
    }
}
