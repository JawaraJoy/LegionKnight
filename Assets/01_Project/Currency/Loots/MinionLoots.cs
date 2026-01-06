using UnityEngine;

namespace LegionKnight
{
    public class MinionLoots : Loots
    {
        public void SetBossDefinition(MinionDefinition minionDefinition)
        {
            SetDefinitionInternal(minionDefinition.LootDefinition);
        }
    }
}
