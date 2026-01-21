using System.Collections;
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
        private List<LootField> m_MirrorLoots = new List<LootField>();
        [SerializeField]
        private UnityEvent<LootField> m_OnAddNewLoot;
        [SerializeField]
        private UnityEvent<LootField> m_OnLootUpdate;
        [SerializeField]
        private UnityEvent<LootField> m_OnMirrorLootUpdate;
        [SerializeField]
        private UnityEvent<LootField> m_OnLootAmountUpdate;
        [SerializeField]
        private UnityEvent<LootField> m_OnRemoveLoot;
        [SerializeField]
        private UnityEvent<List<LootField>> m_OnTakeLoots;
        [SerializeField]
        private bool m_AutoTakeDirectLoot = true;
        [SerializeField]
        private UnityEvent<LootField> m_OnDirectTakeLoot;
        [SerializeField]
        private UnityEvent<List<LootField>> m_OnMirrorLootsChanged;
        [SerializeField]
        private UnityEvent<List<LootField>> m_OnLootedsChanged;

        public List<LootField> Looteds => m_Looteds;
        public List<LootField> MirrorLoots => m_MirrorLoots;

        private bool m_IsTransferring = false;
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
                    LootField.CurrencyApplier(item, amount);
                    LootField.CharacterApplier(item);
                    LootField.StandbyPlatformApplier(item, amount);
                    LootField.EnergyApplier(item, amount);
                    LootField.RandomApplier(item);
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
                LootField.CurrencyApplier(item, amount);
                LootField.StandbyPlatformApplier(item, amount);
                LootField.EnergyApplier(item, amount);
                LootField.CharacterApplier(item);
                LootField.RandomApplier(item);
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
        public void RemoveLoot(LootField loot)
        {
            RemoveLootInternal(loot);
        }
        private void RemoveLootInternal(LootField loot)
        {
            if (HasLootedInternal(loot.Item))
            {
                LootField existingLoot = GetLootedInternal(loot.Item);
                m_Looteds.Remove(existingLoot);
                Debug.Log($"Removed Loot: {existingLoot.Item.name}");
                m_OnRemoveLoot?.Invoke(existingLoot);
            }
        }
        public void ClearLoots()
        {
            ClearLootsInternal();
        }

        private void ClearLootsInternal()
        {
            m_Looteds.Clear();
        }

        public void CopyMirrorFromLooted()
        {
            m_MirrorLoots.Clear();
            m_MirrorLoots = new List<LootField>(m_Looteds);
            m_OnMirrorLootsChanged?.Invoke(m_MirrorLoots);
        }

        public IEnumerator TransferMirrorToLooteds()
        {
            m_IsTransferring = true;
            yield return new WaitForEndOfFrame();

            // IMPORTANT: copy dulu mirror loots agar tidak terkena modifikasi AddLootInternal()
            List<LootField> mirrorsCopy = new List<LootField>(m_MirrorLoots);

            foreach (var mirrorLoot in mirrorsCopy)
            {
                int count = mirrorLoot.Amount;

                for (int i = 0; i < count; i++)
                {
                    // Kurangi 1 dari mirror loot asli
                    mirrorLoot.AddAmount(-1);
                    m_OnMirrorLootUpdate?.Invoke(mirrorLoot);

                    // Transfer 1 amount
                    LootField oneLoot = new LootField(
                        mirrorLoot.Item,
                        mirrorLoot.IsUnique,
                        1,
                        mirrorLoot.Chance
                    );

                    AddLootInternal(oneLoot);

                    yield return new WaitForSeconds(0.1f);
                }
            }

            // Setelah semuanya selesai
            m_MirrorLoots.Clear();
            m_IsTransferring = false;

            m_OnLootedsChanged?.Invoke(m_Looteds);
            m_OnMirrorLootsChanged?.Invoke(m_MirrorLoots);
        }
    }
}
