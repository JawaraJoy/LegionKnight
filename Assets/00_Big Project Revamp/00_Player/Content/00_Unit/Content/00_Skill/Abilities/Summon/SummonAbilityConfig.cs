using LegionKnight;
using UnityEngine;

namespace Rush
{
    public class SummonAbilityConfig : AbilityConfig
    {
        [SerializeField]
        private UnitConfig m_UnitToSpawn;
        [SerializeField]
        private SummonTargetMode m_SummonTargetMode = SummonTargetMode.AroundCasterPosition;
        
        [SerializeField]
        private SpawnShapeConfig m_SpawnShape;
        [SerializeField]
        private SpawnDuration m_SpawnDuration;
        [SerializeField]
        private SpawnSetupField m_SpawnSetup;
        public SummonTargetMode SummonTargetMode => m_SummonTargetMode;
        public UnitConfig UnitToSpawn => m_UnitToSpawn;
        public SpawnShapeConfig SpawnShape => m_SpawnShape;
        public SpawnDuration SpawnDuration => m_SpawnDuration;
        public SpawnSetupField SpawnSetup => m_SpawnSetup;
    }

    public enum SummonTargetMode
    {
        AroundCasterPosition,
        AroundTargetPosition,
        AroundPointedPosition,
    }

    [System.Serializable]
    public class SpawnDuration
    {
        [SerializeField]
        private bool m_HasDuration;
        [SerializeField]
        private float m_Duration;
        public bool HasDuration => m_HasDuration;
        public float Duration => m_Duration;

        public SpawnDuration(bool hasDuration, float duration)
        {
            m_HasDuration = hasDuration;
            m_Duration = duration;
        }
    }
}
