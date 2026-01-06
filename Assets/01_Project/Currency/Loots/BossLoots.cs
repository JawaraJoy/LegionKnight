using UnityEngine;

namespace LegionKnight
{
    public class BossLoots : Loots
    {
        public void SetBossDefinition(BosDefinition bossDefinition)
        {
            SetDefinitionInternal(bossDefinition.LootDefinition);
        }
    }
}
