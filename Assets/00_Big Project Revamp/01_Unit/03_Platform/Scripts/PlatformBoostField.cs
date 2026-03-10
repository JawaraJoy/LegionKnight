using UnityEngine;

namespace Rush
{
    [System.Serializable]
    public class PlatformBoostField
    {
        [SerializeField]
        private int m_BoostThreshold;

        [Header("Boost Movement")]
        [SerializeField, Tooltip("Kecepatan platform terbang ke atas (unit/detik)")]
        private float m_BoostSpeed = 8f;

        [SerializeField, Tooltip("Delay setelah boost selesai sebelum next platform spawn (detik)")]
        private float m_PostBoostSpawnDelay = 0.5f;

        [Header("Boost Stock")]
        [SerializeField, Tooltip("Maksimal jumlah boost yang bisa disimpan")]
        private int m_MaxBoostStock = 3;

        [SerializeField, Tooltip("Durasi minimum boost (detik)")]
        private float m_MinBoostDuration = 1f;
        [SerializeField, Tooltip("Tambahan durasi boost per 1 perfect combo stack (detik)")]
        private float m_BoostDurationPerStack = 1f;

        [Header("Combo")]
        [SerializeField, Tooltip("Minimal jumlah combo button saat boost aktif")]
        private int m_MinComboCount = 1;
        [SerializeField, Tooltip("Tambahan combo per 1 overflow perfect")]
        private int m_ComboPerOverflow = 1;

        public int BoostThreshold => m_BoostThreshold;
        public int MinComboCount => m_MinComboCount;
        public int ComboPerOverflow => m_ComboPerOverflow;

        public int CalculateComboCount(int overflow)
        {
            return m_MinComboCount + (overflow * m_ComboPerOverflow);
        }
        public float BoostSpeed => m_BoostSpeed;
        public float PostBoostSpawnDelay => m_PostBoostSpawnDelay;
        public int MaxBoostStock => m_MaxBoostStock;
        public float MinBoostDuration => m_MinBoostDuration;
        public float BoostDurationPerStack => m_BoostDurationPerStack;

        /// <summary>
        /// Hitung durasi boost berdasarkan combo count.
        /// Durasi = max(MinBoostDuration, comboCount x BoostDurationPerStack)
        /// </summary>
        public float CalculateBoostDuration(int comboCount)
        {
            return Mathf.Max(m_MinBoostDuration, comboCount * m_BoostDurationPerStack);
        }
    }
}