using MoreMountains.Tools;
using System.Collections.Generic;
using UnityEngine;

namespace Rush
{
    public partial class StatModifierDeliver : AbilityDeliver, IStatModifierDeliver
    {
        [SerializeField, MMReadOnly]
        private StatModifierConfig m_InfluencerConfig;
        public StatModifierConfig ModifierConfig
        {
            get
            {
                if (m_InfluencerConfig == null)
                {
                    if (m_AbilityConfig is StatModifierConfig influencerConfig)
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
                if (target.ModuleContext.Unit.HasBind(out StatController statController))
                {
                    statController.AddModifier(m_AbilityContext, statController);
                }
            }
            base.Activate();
        }
        public override void ActiveOverrideTarget(List<ITargetable> overrideTargets)
        {
            foreach (var target in overrideTargets)
            {
                if (target.ModuleContext.Unit.HasBind(out StatController statController))
                {
                    statController.AddModifier(m_AbilityContext, statController);
                }
            }
            base.ActiveOverrideTarget(overrideTargets);
        }
    }
}
