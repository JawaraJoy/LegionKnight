using UnityEngine;

namespace Rush
{
    [System.Serializable]
    public class StatusEffectContext
    {
        [SerializeField]
        private IAbilityContext m_AbilityContext;
        [SerializeField]
        private StatusEffector m_StatusEffector;
        public IAbilityContext AbilityContext => m_AbilityContext;
        public StatusEffector StatusEffector => m_StatusEffector;
        public bool IsInitialed => m_AbilityContext.Initialized && m_StatusEffector != null;
        public StatusEffectContext(IAbilityContext abilityContext, StatusEffector effector)
        {
            m_AbilityContext = abilityContext;
            m_StatusEffector = effector;
        }
    }
}
