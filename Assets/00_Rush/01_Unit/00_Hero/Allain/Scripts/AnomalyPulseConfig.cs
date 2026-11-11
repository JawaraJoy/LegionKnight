using UnityEngine;

namespace Rush
{
    [CreateAssetMenu(menuName = "Rush/Combat/Allain/Anomaly Pulse")]
    public class AnomalyPulseConfig : DamageAbilityConfig
    {
        [SerializeField] private AnomalyPullField m_Pull = new AnomalyPullField();
        public AnomalyPullField Pull => m_Pull;
        protected override int GetDamageInternal(IAbilityContext context) => Mathf.CeilToInt(AbilityUltility.GetFinalPowerAmount(context));
    }

    [System.Serializable]
    public class AnomalyPullField
    {
        [SerializeField] private float m_Radius = 4f;
        [SerializeField] private float m_Distance = 2f;
        [SerializeField] private float m_Duration = 0.25f;
        public float Radius => Mathf.Max(0f, m_Radius);
        public float Distance => Mathf.Max(0f, m_Distance);
        public float Duration => Mathf.Max(0f, m_Duration);
    }
}
