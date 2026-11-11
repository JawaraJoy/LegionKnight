using UnityEngine;

namespace Rush
{
    [CreateAssetMenu(menuName = "Rush/Combat/Allain/Anomaly Incursion")]
    public class AnomalyIncursionConfig : AbilityConfig
    {
        [SerializeField] private AnomalyEntityField m_Entity = new AnomalyEntityField();
        public AnomalyEntityField Entity => m_Entity;
    }

    [System.Serializable]
    public class AnomalyEntityField
    {
        [SerializeField] private MinionUnitConfig m_Unit;
        [SerializeField] private AnomalyPulseConfig m_DeathExplosion;
        [SerializeField] private int m_Count = 3;
        [SerializeField] private int m_Barriers = 2;
        public MinionUnitConfig Unit => m_Unit;
        public AnomalyPulseConfig DeathExplosion => m_DeathExplosion;
        public int Count => Mathf.Max(0, m_Count);
        public int Barriers => Mathf.Max(0, m_Barriers);
    }
}
