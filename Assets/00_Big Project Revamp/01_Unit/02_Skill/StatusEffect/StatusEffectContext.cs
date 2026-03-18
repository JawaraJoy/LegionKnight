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
        public IAbilityContext AbilityContext => m_AbilityContext;
        public Unit Infected => m_Infected;
        public bool IsInitialed => m_AbilityContext.Initialized && m_Infected != null;
        public StatusEffectContext(IAbilityContext abilityContext, Unit infected)
        {
            m_AbilityContext = abilityContext;
            m_Infected = infected;
        }
    }
}
