using UnityEngine;

namespace Rush
{
    [CreateAssetMenu(menuName = "Rush/Combat/Allain/Aegis Conversion")]
    public class AegisConversionConfig : StatModifierConfig, ILifeStealModifier
    {
        [SerializeField, Range(0f, 1f)] private float m_LifeStealPerStack = 0.01f;
        [SerializeField, Min(0f), Tooltip("Growth per skill level as a fraction of the level-one rate.")]
        private float m_GrowthPerLevel = .15f;

        public float LifeStealPerStackAtLevel(int skillLevel) =>
            m_LifeStealPerStack * (1f + Mathf.Max(0, skillLevel - 1) * m_GrowthPerLevel);
    }
}
