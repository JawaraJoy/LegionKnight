using LegionKnight;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Rush
{
    public class LevelUpMonitor : UIView
    {
        [SerializeField]
        private LootItemView m_LootItemViewPrefab; // LootItemView prefab untuk di-pool
        [SerializeField]
        private Transform m_LootContainer; // Container untuk meletakkan LootItemView

        private Queue<LootItemView> m_LootItemViewPool = new(); // Pool untuk menyimpan LootItemView
        private List<LootItemView> m_SpawnedLootItems = new(); // Loot items yang sudah di-spawn

        [SerializeField]
        private int m_InitialPoolSize = 5; // Ukuran pool awal

        // Inisialisasi pool dengan sejumlah LootItemView yang sudah dibuat
        public void Init()
        {
            Player.Instance.Progression.OnLevelUpExpTable.AddListener(UpdateLevelUpMonitor);
            for (int i = 0; i < m_InitialPoolSize; i++)
            {
                LootItemView lootItemView = Instantiate(m_LootItemViewPrefab, m_LootContainer);
                lootItemView.gameObject.SetActive(false); // Nonaktifkan objek yang di-pool
                m_LootItemViewPool.Enqueue(lootItemView); // Tambahkan ke pool
            }
        }

        // Fungsi untuk mendapatkan LootItemView dari pool
        private LootItemView GetLootItemViewFromPool()
        {
            if (m_LootItemViewPool.Count > 0)
            {
                LootItemView lootItemView = m_LootItemViewPool.Dequeue(); // Ambil dari pool
                lootItemView.gameObject.SetActive(true); // Aktifkan objek saat digunakan
                return lootItemView;
            }
            else
            {
                // Jika pool kosong, instantiate objek baru
                LootItemView lootItemView = Instantiate(m_LootItemViewPrefab, m_LootContainer);
                return lootItemView;
            }
        }

        // Fungsi untuk mengembalikan LootItemView ke pool
        private void ReturnLootItemViewToPool(LootItemView lootItemView)
        {
            lootItemView.gameObject.SetActive(false); // Nonaktifkan objek saat dikembalikan
            m_LootItemViewPool.Enqueue(lootItemView); // Kembalikan ke pool
        }

        protected virtual void UpdateLevelUpMonitor(ExpTable expTable)
        {
            ClearLootItems();
            List<LootField> loots = expTable.RewardLevelReached.LootFields.ToList();

            // Untuk setiap loot, ambil LootItemView dari pool dan spawn
            foreach (var loot in loots)
            {
                LootItemView lootItemView = GetLootItemViewFromPool(); // Ambil dari pool
                lootItemView.Init(loot); // Inisialisasi LootItemView
                m_SpawnedLootItems.Add(lootItemView); // Tambahkan ke list yang sudah di-spawn
            }
        }

        // Fungsi untuk menghapus semua LootItemView yang sudah di-spawn
        private void ClearLootItems()
        {
            foreach (var lootItemView in m_SpawnedLootItems)
            {
                ReturnLootItemViewToPool(lootItemView); // Kembalikan ke pool
            }
            m_SpawnedLootItems.Clear(); // Kosongkan list
        }
    }
}