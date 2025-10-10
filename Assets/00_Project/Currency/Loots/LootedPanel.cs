using System.Linq;
using UnityEngine;

namespace LegionKnight
{
    public class LootedPanel : PanelView
    {
        private LootMonitor m_LootMonitor;

        private LootMonitor GetLootMonitor()
        {
            if (m_LootMonitor == null)
            {
                m_LootMonitor = GetBindingInternal<LootMonitor>();
            }
            return m_LootMonitor;
        }
        public void ShowLoot(LootField[] loots)
        {
            Show();
            GetLootMonitor().ClearAllLootViews();
            GetLootMonitor().AddLootsView(loots.ToList());
        }
        
    }
}
