using System.Collections.Generic;
using UnityEngine;

namespace Rush
{
    public class PlatformAbilityDeliverField : IAbilityDeliver
    {
        [SerializeField]
        private AbilityConfig m_AbilityConfig;
        [SerializeField]
        private AbilityContext m_AbilityContext;
        public AbilityConfig AbilityConfig => m_AbilityConfig;
        private List<StatusEffectConfig> m_CustomStatusEffectOnDelivered;
        private List<StatusEffectConfig> m_CustomStatusEffectOnSelf;
        public IAbilityContext AbilityContext => m_AbilityContext;

        public Transform DeliverTransform => m_AbilityContext.SkillContext.ModuleContext.Module.transform;

        public void Activate()
        {
            m_AbilityContext.SkillContext.Skill.ForceActivate(m_AbilityConfig);
        }

        public List<StatusEffectConfig> GetStatusEffectsOnDelivered()
        {
            List<StatusEffectConfig> totalStatusEff = new List<StatusEffectConfig>(m_AbilityConfig.StatusEffectOnDelivered);
            if (m_CustomStatusEffectOnDelivered != null)
            {
                totalStatusEff.AddRange(m_CustomStatusEffectOnDelivered);
            }
            return totalStatusEff;
        }
        public List<StatusEffectConfig> GetStatusEffectsOnSelf()
        {
            List<StatusEffectConfig> totalStatusEff = new(m_AbilityConfig.StatusEffectOnSelf);
            if (m_CustomStatusEffectOnSelf != null)
            {
                totalStatusEff.AddRange(m_CustomStatusEffectOnSelf);
            }
            return totalStatusEff;

        }
        public void AddCustomStatusEffectOnDelivered(StatusEffectConfig config)
        {
            m_CustomStatusEffectOnDelivered ??= new List<StatusEffectConfig>();
            m_CustomStatusEffectOnDelivered.Add(config);
        }
        public void AddCustomStatusEffectOnSelf(StatusEffectConfig config)
        {
            m_CustomStatusEffectOnSelf ??= new List<StatusEffectConfig>();
            m_CustomStatusEffectOnSelf.Add(config);
        }
        private void ClearCustomStatusEffects()
        {
            m_CustomStatusEffectOnDelivered?.Clear();
            m_CustomStatusEffectOnSelf?.Clear();
        }

        public void Init(AbilityConfig config, ISkillContext skillContext)
        {
            m_AbilityConfig = config;
            m_AbilityContext = new AbilityContext(this, skillContext);
        }
        
    }
}
