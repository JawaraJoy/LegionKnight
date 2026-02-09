using MoreMountains.Tools;
using UnityEngine;

namespace Rush
{
    [System.Serializable]
    public partial class StatInfluencerContext
    {
        [SerializeField, MMReadOnly]
        private AbilityContext m_AbilityContext;
        [SerializeField, MMReadOnly]
        private StatInfluencer m_Influencer;
        public AbilityContext AbilityContext => m_AbilityContext;
        public StatInfluencer Influencer => m_Influencer;
        public bool Initialized => m_AbilityContext.Initialized && m_Influencer != null;
        public StatInfluencerContext(AbilityContext abilityContext, StatInfluencer influencer)
        {
            m_AbilityContext = abilityContext;
            m_Influencer = influencer;
        }
    }
}
