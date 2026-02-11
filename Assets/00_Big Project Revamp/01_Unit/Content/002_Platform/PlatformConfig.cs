using LegionKnight;
using UnityEngine;

namespace Rush
{
    [CreateAssetMenu(fileName = "Platform", menuName = "Rush/Unit/Platform", order = 1)]
    public class PlatformConfig : Configuration, IHasIcon
    {
        [SerializeField]
        private Platform m_PlatformPrefab;
        [SerializeField]
        private Sprite m_Icon;
        [SerializeField, Range(0.01f, 1f)]
        private float m_ChanceToSpawn = 1f;
        [SerializeField, Range(0.1f, 1f)]
        private float m_PerfectOffsite = 0.3f;
        [SerializeField]
        private PlatformAbilityField[] m_AbilitySets;
        public float ChanceToSpawn => m_ChanceToSpawn;
        public float PerfectOffsite => m_PerfectOffsite;
        public PlatformAbilityField[] AbilitySets => m_AbilitySets;
        public Sprite Icon => m_Icon;
        public Platform PlatformPrefab => m_PlatformPrefab;
        //public float ChanceToSpawn => m_ChanceToSpawn;
    }
}
