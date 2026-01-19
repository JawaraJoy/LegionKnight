using MoreMountains.Tools;
using UnityEngine;

namespace Rush
{
    public class Summoner : AbilityDeliver
    {
        [SerializeField, MMReadOnly]
        private SummonAbilityConfig m_SummonConfig;
        public SummonAbilityConfig SummonConfig => m_SummonConfig;
        public override void Init(AbilityConfig config, SkillContext context)
        {
            base.Init(config, context);
            if (m_Config is SummonAbilityConfig summonConfig)
            {
                m_SummonConfig = summonConfig;
            }
        }
    }
}
