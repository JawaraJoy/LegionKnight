using Rush;
using System.Linq;
using UnityEngine;

namespace LegionKnight
{
    public class LevelUpPreviewLootMonitor : PreviewLootMonitor
    {
        public void ShowRewardLevelUp(LootChestDefinition lootDefinition)
        {
            ClearAllLootViews();
            AddLootsViewInternal(lootDefinition.LootFields.ToList());
            AddLoots(lootDefinition.LootFields);
        }
        private void AddLoots(LootField[] loots)
        {
            GetLootStorage().AddLoots(loots);
        }
    }
    public partial class LevelUpPanel
    {
        private LevelUpPreviewLootMonitor GetUpPreviewLootMonitor()
        {
            return GetBindingInternal<LevelUpPreviewLootMonitor>();
        }
        public void ShowRewardLevelUp(LootChestDefinition lootDefinition)
        {
            LevelUpPreviewLootMonitor lootMonitor = GetUpPreviewLootMonitor();
            if (lootMonitor != null)
            {
                lootMonitor.ShowRewardLevelUp(lootDefinition);
            }
        }
    }
}
