using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace LegionKnight
{
    public class LootStorage : MonoBehaviour
    {
        [SerializeField]
        private List<LootField> m_Looteds = new List<LootField>();
        [SerializeField]
        private UnityEvent<LootField> m_OnAddLoot;
        [SerializeField]
        private UnityEvent<List<LootField>> m_OnTakeLoots;
        [SerializeField]
        private bool m_AutoTakeDirectLoot = true;
        [SerializeField]
        private UnityEvent<LootField> m_OnDirectTakeLoot;
        
        public List<LootField> Looteds => m_Looteds;
        public void TakeLooteds()
        {
            if (m_Looteds.Count < 1)
            {
                return;
            }
            foreach (var loot in m_Looteds)
            {
                if (loot.Item is ScriptableObject item)
                {
                    int amount = loot.Amount;
                    CurrencyApplier(item, amount);
                    CharacterApplier(item);
                }
            }
            m_OnTakeLoots?.Invoke(m_Looteds);
            ClearLootsInternal();
        }
        public void DirectTakeLoot(LootField loot)
        {
            if (!m_AutoTakeDirectLoot)
            {
                return;
            }
            if (loot.Item is ScriptableObject item)
            {
                int amount = loot.Amount;
                CurrencyApplier(item, amount);
                CharacterApplier(item);
            }
            m_OnDirectTakeLoot?.Invoke(loot);
        }
        public void AddLoots(LootField[] loots)
        {
            m_Looteds.AddRange(loots);
        }
        public void AddLoot(LootField loot)
        {
            m_Looteds.Add(loot);
            DirectTakeLoot(loot);
            m_OnAddLoot?.Invoke(loot);
        }
        public void ClearLoots()
        {
            ClearLootsInternal();
        }

        private void ClearLootsInternal()
        {
            m_Looteds.Clear();
        }
        private void CurrencyApplier(ScriptableObject defi, int amount)
        {
            if (defi is CurrencyDefinition currency)
            {
                Player.Instance.AddCurrencyAmount(currency, amount);
            }
        }
        private void CharacterApplier(ScriptableObject defi)
        {
            if (defi is CharacterDefinition character)
            {
                bool owned = Player.Instance.GetCharacterUnit(character).Owned;
                if (owned)
                {
                    Player.Instance.AddCurrencyAmount(character.ShardConvert.CurrencyDefinition, character.ShardConvert.Amount);
                }
                else
                {
                    Player.Instance.SetOwned(character, true);
                }
            }
        }
    }
}
