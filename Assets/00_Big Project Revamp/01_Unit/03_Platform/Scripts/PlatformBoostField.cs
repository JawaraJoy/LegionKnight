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

        [SerializeField, Tooltip("Durasi platform terbang ke atas (detik)")]
        private float m_BoostDuration = 1.5f;

        [SerializeField, Tooltip("Delay setelah boost selesai sebelum next platform spawn (detik)")]
        private float m_PostBoostSpawnDelay = 0.5f;

        public float BoostSpeed => m_BoostSpeed;
        public float BoostDuration => m_BoostDuration;
        public float PostBoostSpawnDelay => m_PostBoostSpawnDelay;

        public void TryApplyBoost(int currentStayPerfectCount, Platform2D platform2D)
        {
            bool shouldApplyBoost = currentStayPerfectCount >= m_BoostThreshold;
            if (shouldApplyBoost)
            {
                platform2D.Boost(this);
            }
        }
    }
}
