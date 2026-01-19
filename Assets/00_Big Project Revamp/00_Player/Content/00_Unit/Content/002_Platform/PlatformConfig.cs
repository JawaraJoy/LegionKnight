using UnityEngine;

namespace Rush
{
    public class PlatformConfig : UnitConfig
    {
        [SerializeField, Range(0.01f, 1f)]
        private float m_ChanceToSpawn = 1f;
        public float ChanceToSpawn => m_ChanceToSpawn;
    }
}
