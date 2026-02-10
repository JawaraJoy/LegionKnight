using UnityEngine;

namespace Rush
{
    [CreateAssetMenu(fileName = "Platform", menuName = "Rush/Unit/Platform", order = 1)]
    public class PlatformConfig : Configuration
    {
        [SerializeField, Range(0.01f, 1f)]
        private float m_ChanceToSpawn = 1f;
        [SerializeField, Range(0.1f, 1f)]
        private float m_PerfectOffsite = 0.3f;
        [SerializeField]
        private PlatformAbilityField m_AbilitySet;
        //public float ChanceToSpawn => m_ChanceToSpawn;
    }
}
