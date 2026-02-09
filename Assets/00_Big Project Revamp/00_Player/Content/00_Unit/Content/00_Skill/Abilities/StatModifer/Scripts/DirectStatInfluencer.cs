using MoreMountains.Tools;
using UnityEngine;

namespace Rush
{
    public partial class DirectStatInfluencer : AbilityDeliver, IStatInfluencer
    {
        [SerializeField, MMReadOnly]
        private StatInfluencerConfig m_InfluencerConfig;
        public StatInfluencerConfig InfluencerConfig
        {
            get
            {
                if (m_InfluencerConfig == null)
                {
                    if (m_Config is StatInfluencerConfig influencerConfig)
                    {
                        m_InfluencerConfig = influencerConfig;
                    }
                    else
                    {
                        m_InfluencerConfig = null;
                    }
                }
                return m_InfluencerConfig;
            }
        }
        public override void Activate()
        {
            foreach (var target in GetTargetsInternal())
            {
                if (target.HasBind(out StatModifier statModifier))
                {
                    statModifier.AddModifier(m_AbilityContext);
                }
            }
            base.Activate();
        }
    }
}
