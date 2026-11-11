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

        public int BoostThreshold => m_BoostThreshold;
        public float BoostSpeed => m_BoostSpeed;
        public float PostBoostSpawnDelay => m_PostBoostSpawnDelay;
        public int MaxBoostStock => m_MaxBoostStock;
        public float MinBoostDuration => m_MinBoostDuration;
        public float BoostDurationPerStack => m_BoostDurationPerStack;

        /// <summary>
        /// Jumlah combo button = 1 (minimum) + overflow perfect landing di atas threshold.
        /// </summary>
        public int CalculateComboCount(int overflow)
        {
            return 1 + overflow;
        }

        /// <summary>
        /// Durasi boost = max(MinBoostDuration, comboCount x BoostDurationPerStack)
        /// </summary>
        public float CalculateBoostDuration(int comboCount)
        {
            return Mathf.Max(m_MinBoostDuration, comboCount * m_BoostDurationPerStack);
        }
    }
}