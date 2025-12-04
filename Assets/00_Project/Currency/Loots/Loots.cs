using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace LegionKnight
{
    public class Loots : MonoBehaviour
    {
        [SerializeField]
        private LootDefinition m_Definition;
        [SerializeField]
        private UnityEvent<LootField> m_OnTakeLoot;
        [SerializeField]
        private UnityEvent<LootField[]> m_OnTakeRandomLoots;

        [SerializeField]
        private UnityEvent<ScriptableObject> m_OnLootTake;


        public void SetDefinition(LootDefinition definition)
        {
            SetDefinitionInternal(definition);
        }
        protected void SetDefinitionInternal(LootDefinition definition)
        {
            m_Definition = definition;
        }
        public void TakeOneLoot()
        {
            var loot = m_Definition.GetRandomOneLoot();
            if (loot != null)
            {
                m_OnTakeLoot?.Invoke(loot);
                m_OnLootTake?.Invoke(loot.Item);
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
                m_OnLootTake?.Invoke(loot.Item);
            }
        }
    }
}
