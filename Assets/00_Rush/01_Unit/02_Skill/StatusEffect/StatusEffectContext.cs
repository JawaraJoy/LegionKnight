using UnityEngine;

namespace Rush
{
    [System.Serializable]
    public class StatusEffectContext
    {
        [SerializeField]
        private IAbilityContext m_AbilityContext;
        [SerializeField]
        private Unit m_Infected;
        private readonly int m_SourceGeneration;
        internal bool HasCurrentSource
        {
            get
            {
                Unit source = m_AbilityContext?.SkillContext?.ModuleContext?.Unit;
                return source != null && (!(source.Config is EnemyUnitConfig) ||
                    (source.gameObject.activeInHierarchy && source.SpawnGeneration == m_SourceGeneration));
            }
        }

        public IAbilityContext AbilityContext => m_AbilityContext;
        public Unit Infected => m_Infected;
        internal int AppliedShield { get; set; }
        internal float AppliedReductionRate { get; set; }
        public int CurrentStack { get; internal set; }
        public bool IsInitialed => m_AbilityContext.Initialized && m_Infected != null;
        public StatusEffectContext(IAbilityContext abilityContext, Unit infected)
        {
            m_AbilityContext = abilityContext;
            m_SourceGeneration = abilityContext?.SkillContext?.ModuleContext?.Unit?.SpawnGeneration ?? 0;
            m_Infected = infected;
        }
    }
}
