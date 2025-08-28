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
        private UnityEvent<LootField> m_OnAddNewLoot;
        [SerializeField]
        private UnityEvent<LootField> m_OnLootUpdate;
        [SerializeField]
        private UnityEvent<LootField> m_OnLootAmountUpdate;
        [SerializeField]
        private UnityEvent<List<LootField>> m_OnTakeLoots;
        [SerializeField]
        private bool m_AutoTakeDirectLoot = true;
        [SerializeField]
        private UnityEvent<LootField> m_OnDirectTakeLoot;
        
        public List<LootField> Looteds => m_Looteds;
        private LootField GetLootedInternal(ScriptableObject item)
        {
            LootField loot = m_Looteds.Find(x => x.Item == item);
            if (loot == null)
            {
                return null;
            }
            return loot;
        }
        private bool HasLootedInternal(ScriptableObject item)
        {
            LootField loot = GetLootedInternal(item);
            return loot != null;
        }
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
                StandbyPlatformApplier(item, amount);
                CharacterApplier(item);
            }
            m_OnDirectTakeLoot?.Invoke(loot);
        }
        public void AddLoots(LootField[] loots)
        {
            foreach (var loot in loots)
            {
                AddLootInternal(loot);
            }
        }

        private void AddLootInternal(LootField loot)
        {
            LootField newLoot = new (loot.Item, loot.IsUnique, loot.Amount, loot.Chance);
            if (HasLootedInternal(newLoot.Item))
            {
                if (newLoot.IsUnique)
                {
                    m_Looteds.Add(newLoot);
                    m_OnAddNewLoot?.Invoke(newLoot);
                }
                else
                {
                    newLoot = GetLootedInternal(newLoot.Item);
                    newLoot.AddAmount(loot.Amount);
                    Debug.Log($"Updated Loot: {newLoot.Item.name} x{newLoot.Amount}");
                    m_OnLootAmountUpdate?.Invoke(newLoot);
                }    
            }
            else
            {
                m_Looteds.Add(newLoot);
                m_OnAddNewLoot?.Invoke(newLoot);
            }
            m_OnLootUpdate?.Invoke(newLoot);
            DirectTakeLoot(newLoot);
        }
        public void AddLoot(LootField loot)
        {
            AddLootInternal(loot);
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
        private void StandbyPlatformApplier(ScriptableObject defi, int amount)
        {
            if (defi is StandbyPlatformDefinition platform)
            {
                Player.Instance.AddPlatformAmount(platform, amount);
            }
        }
    }
}
