using Rush;
using UnityEngine;

namespace LegionKnight
{
    [System.Serializable]
    public class SpawnSetupField
    {
        [SerializeField]
        private int m_PreWarmCount = 5;
        [SerializeField]
        private FireMode m_FireMode = FireMode.Instant;
        [SerializeField]
        private int m_FireCount = 5;
        [SerializeField]
        private float m_FireInterval = 0.2f;
        [SerializeField]
        private int m_BurstCount = 3;
        [SerializeField]
        private float m_BurstInterval = 0.3f;
        public FireMode FireMode => m_FireMode;
        public int BurstCount => m_BurstCount;
        public float BurstInterval => m_BurstInterval;
        public int PreWarmCount => m_PreWarmCount;
        public int FireCount => m_FireCount;
        public float FireInterval => m_FireInterval;
    }
}
