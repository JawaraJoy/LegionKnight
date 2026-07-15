
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Rush
{
    public abstract partial class AbilityDeliver : MonoBehaviour, IAbilityDeliver
    {
        [SerializeField]
        protected AbilityConfig m_AbilityConfig;
        protected AbilityContext m_AbilityContext;
        [SerializeField]
        protected Transform m_DeliverTransform;
        [SerializeField]
        private UnityEvent<AbilityContext> m_OnInit;
        [SerializeField]
        protected UnityEvent<AbilityContext> m_OnActivate;
        public AbilityConfig AbilityConfig => m_AbilityConfig;
        public IAbilityContext AbilityContext => m_AbilityContext;
        public Transform DeliverTransform => m_DeliverTransform;
        private List<StatusEffectConfig> m_CustomStatusEffectOnDelivered;
        private List<StatusEffectConfig> m_CustomStatusEffectOnSelf;
        public List<StatusEffectConfig> GetStatusEffectsOnDelivered()
        {
            List<StatusEffectConfig> totalStatusEff = new(m_AbilityConfig.StatusEffectOnDelivered);
            if (m_CustomStatusEffectOnDelivered != null)
            {
                totalStatusEff.AddRange(m_CustomStatusEffectOnDelivered);
            }
            return totalStatusEff;
        }
        public List<StatusEffectConfig> GetStatusEffectsOnSelf()
        {
            List<StatusEffectConfig> totalStatusEff = new (m_AbilityConfig.StatusEffectOnSelf);
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
        protected List<ITargetable> GetTargetsInternal()
        {
            List<ITargetable> damageables = new(AbilityUltility.ApplyTargetPriority(m_AbilityContext));
            return damageables;
        }
        public virtual void Init(AbilityConfig config, ISkillContext context)
        {
            m_AbilityContext = new AbilityContext(this, context);
            m_AbilityConfig = config;
            m_OnInit?.Invoke(m_AbilityContext);
        }
        public virtual void Activate()
        {
            m_OnActivate?.Invoke(m_AbilityContext);
        }


    }
}
