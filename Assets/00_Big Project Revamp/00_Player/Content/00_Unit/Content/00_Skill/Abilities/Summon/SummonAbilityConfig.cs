using UnityEngine;

namespace Rush
{
    public class SummonAbilityConfig : AbilityConfig
    {
        [SerializeField]
        private UnitConfig[] m_UnitToSpawns;
        [SerializeField]
        private SpawnShapeConfig m_SpawnShape;
        public UnitConfig[] UnitToSpawns => m_UnitToSpawns;
        public SpawnShapeConfig SpawnShape => m_SpawnShape;
    }
}
