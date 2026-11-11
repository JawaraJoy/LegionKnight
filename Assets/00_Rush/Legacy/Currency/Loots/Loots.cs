using Rush;
using UnityEngine;
using UnityEngine.Events;

namespace LegionKnight
{
    public class Loots : MonoBehaviour
    {
        [SerializeField]
        private LootChestDefinition m_Definition;
        [SerializeField]
        private UnityEvent<LootField> m_OnTakeLoot;
        [SerializeField]
        private UnityEvent<LootField[]> m_OnTakeRandomLoots;

        [SerializeField]
        private UnityEvent<CollectibleConfig> m_OnLootTake;


        public void SetDefinition(LootChestDefinition definition)
        {
            SetDefinitionInternal(definition);
        }
        protected void SetDefinitionInternal(LootChestDefinition definition)
        {
            m_Definition = definition;
        }
        public void TakeOneLoot()
        {
            var loot = m_Definition.GetRandomOneLoot();
            if (loot != null)
            {
                m_OnTakeLoot?.Invoke(loot);
                m_OnLootTake?.Invoke(loot.ItemLoot);
            }
        }
        public void TakeLoots()
        {
            var loots = m_Definition.GetRandomLoots();
            if (loots.Count > 0)
            {
                m_OnTakeRandomLoots?.Invoke(loots.ToArray());
            }
            foreach (var loot in loots)
            {
                m_OnTakeLoot?.Invoke(loot);
                m_OnLootTake?.Invoke(loot.ItemLoot);
            }
        }
    }
}
