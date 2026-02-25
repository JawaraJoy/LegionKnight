using Rush;
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
        private LootField GetLootedInternal(CollectibleConfig collectibleConfig)
        {
            LootField loot = m_Looteds.Find(x => x.ItemLoot == collectibleConfig);
            if (loot == null)
            {
                return null;
            }
            return loot;
        }
        private bool HasLootedInternal(CollectibleConfig collectibleConfig)
        {
            LootField loot = GetLootedInternal(collectibleConfig);
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
                int amount = loot.Amount;
                LootField.CurrencyApplier(loot.ItemLoot, amount);
                LootField.CharacterApplier(loot.ItemLoot);
                LootField.StandbyPlatformApplier(loot.ItemLoot, amount);
                LootField.EnergyApplier(loot.ItemLoot, amount);
                LootField.RandomApplier(loot.ItemLoot);
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
            int amount = loot.Amount;
            LootField.CurrencyApplier(loot.ItemLoot, amount);
            LootField.StandbyPlatformApplier(loot.ItemLoot, amount);
            LootField.EnergyApplier(loot.ItemLoot, amount);
            LootField.CharacterApplier(loot.ItemLoot);
            LootField.RandomApplier(loot.ItemLoot);
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
            if (HasLootedInternal(loot.ItemLoot))
            {
                if (loot.ItemLoot.CollectibleField.IsUnique)
                {
                    m_Looteds.Add(loot);
                    m_OnAddNewLoot?.Invoke(loot);
                }
                else
                {
                    loot = GetLootedInternal(loot.ItemLoot);
                    loot.AddAmount(loot.Amount);
                    Debug.Log($"Updated Loot: {loot.ItemLoot.name} x{loot.Amount}");
                    m_OnLootAmountUpdate?.Invoke(loot);
                }    
            }
            else
            {
                m_Looteds.Add(loot);
                m_OnAddNewLoot?.Invoke(loot);
            }
            m_OnLootUpdate?.Invoke(loot);
            DirectTakeLoot(loot);
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
            if (HasLootedInternal(loot.ItemLoot))
            {
                LootField existingLoot = GetLootedInternal(loot.ItemLoot);
                m_Looteds.Remove(existingLoot);
                Debug.Log($"Removed Loot: {existingLoot.ItemLoot.BaseInfo.Name}");
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
                    mirrorLoot.AddAmount(-1);
                    m_OnMirrorLootUpdate?.Invoke(mirrorLoot);

                    LootField oneLoot = new(mirrorLoot.ItemLoot, mirrorLoot.Amount, mirrorLoot.Chance);
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
