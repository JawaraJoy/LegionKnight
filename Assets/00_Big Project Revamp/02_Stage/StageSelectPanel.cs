using Rush;
using System.Collections.Generic;
using UnityEngine;

namespace LegionKnight
{
    // ─── Panel ────────────────────────────────────────────────────────────────
    /// <summary>
    /// Panel yang meng-spawn daftar StageSelectView secara dinamis.
    /// Source data diambil dari StageHandler.StageSelections (sudah include StageState).
    /// Filter berdasarkan StageMode yang dikonfigurasi di Inspector atau via SetFilterMode().
    /// </summary>
    public partial class StageSelectPanel : PanelView
    {
        [Header("Filter")]
        [Tooltip("Hanya StageConfig dengan StageMode ini yang akan di-spawn.")]
        [SerializeField]
        private StageMode m_FilterMode = StageMode.Classic;

        [Header("Spawn")]
        [SerializeField]
        private StageSelectView m_ItemPrefab;
        [SerializeField]
        private Transform m_ItemContainer;

        private readonly List<StageSelectView> m_SpawnedItems = new();

        private readonly bool m_Spawned = false;

        // ── Public API ────────────────────────────────────────────────────────

        /// <summary>
        /// Tampilkan panel dan spawn list sesuai filter mode saat ini.
        /// </summary>
        protected override void ShowInternal()
        {
            base.ShowInternal();
            SpawnItems();
        }

        /// <summary>
        /// Ganti filter mode lalu refresh list.
        /// </summary>
        public void SetFilterMode(StageMode mode)
        {
            m_FilterMode = mode;
            SpawnItems();
        }

        private void Refresh()
        {
            if (m_SpawnedItems.Count <= 0) return;
            foreach (var item in m_SpawnedItems)
            {
                item.Show();
                item.Refresh();
            }
        }

        // ── Spawn ─────────────────────────────────────────────────────────────
        private void SpawnItems()
        {
            Refresh();

            if (m_Spawned) return;
            ClearItems();

            if (m_ItemPrefab == null)
            {
                Debug.LogError("[StageSelectPanel] m_ItemPrefab belum di-assign.");
                return;
            }

            if (m_ItemContainer == null)
            {
                Debug.LogError("[StageSelectPanel] m_ItemContainer belum di-assign.");
                return;
            }

            // Ambil langsung dari StageHandler — sudah include StageState hasil load save data
            StageSelectionField[] selections = RushGameManager.Instance
                .StageManager
                .StageSelections;

            if (selections == null || selections.Length == 0)
            {
                Debug.LogWarning("[StageSelectPanel] StageSelections kosong di StageHandler.");
                return;
            }

            foreach (StageSelectionField field in selections)
            {
                if (field == null || field.StageConfig == null) continue;

                // Filter: hanya spawn jika StageMode cocok
                if (field.StageConfig.StageMode != m_FilterMode) continue;

                StageSelectView item = Instantiate(m_ItemPrefab, m_ItemContainer);
                item.Setup(field);
                item.Show();
                m_SpawnedItems.Add(item);
            }
        }

        private void ClearItems()
        {
            foreach (StageSelectView item in m_SpawnedItems)
            {
                if (item != null)
                    Destroy(item.gameObject);
            }
            m_SpawnedItems.Clear();
        }
    }
}