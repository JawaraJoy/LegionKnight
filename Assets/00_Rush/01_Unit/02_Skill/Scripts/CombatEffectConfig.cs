using UnityEngine;

namespace Rush
{
    public abstract class CombatEffectConfig : Configuration, IHasRarityConfig
    {
        [SerializeField, Tooltip("For a hero-specific effect, assign the hero's rarity. Shared effects can inherit rarity from the source skill.")]
        private RarityConfig m_RarityConfig;

        public RarityConfig RarityConfig => m_RarityConfig;
    }
}
