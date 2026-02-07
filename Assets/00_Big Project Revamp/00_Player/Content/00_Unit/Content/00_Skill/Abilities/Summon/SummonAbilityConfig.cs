using UnityEngine;

namespace Rush
{
    public class SummonAbilityConfig : AbilityConfig
    {
        [SerializeField]
        private UnitConfig[] m_UnitToSpawns;
        [SerializeField]
        private SummonTargetMode m_SummonTargetMode = SummonTargetMode.AroundCasterPosition;
        [SerializeField]
        private SpawnShapeConfig m_SpawnShape;
        public SummonTargetMode SummonTargetMode => m_SummonTargetMode;
        public UnitConfig[] UnitToSpawns => m_UnitToSpawns;
        public SpawnShapeConfig SpawnShape => m_SpawnShape;
    }

    public enum SummonTargetMode
    {
        AroundCasterPosition,
        AroundTargetPosition,
    }
}
