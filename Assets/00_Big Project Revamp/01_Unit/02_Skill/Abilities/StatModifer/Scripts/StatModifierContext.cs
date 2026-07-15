
using UnityEngine;

namespace Rush
{
    [System.Serializable]
    public partial class StatModifierContext
    {
        private AbilityContext m_AbilityContext;
        private StatModifier m_Influencer;
        public AbilityContext AbilityContext => m_AbilityContext;
        public StatModifier Influencer => m_Influencer;
        public bool Initialized => m_AbilityContext.Initialized && m_Influencer != null;
        public StatModifierContext(AbilityContext abilityContext, StatModifier influencer)
        {
            m_AbilityContext = abilityContext;
            m_Influencer = influencer;
        }
    }
}
