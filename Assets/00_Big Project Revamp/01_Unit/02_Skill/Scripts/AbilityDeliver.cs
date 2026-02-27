using MoreMountains.Tools;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Rush
{
    public abstract partial class AbilityDeliver : MonoBehaviour, IAbilityDeliver
    {
        [SerializeField]
        protected AbilityConfig m_AbilityConfig;
        [SerializeField, MMReadOnly]
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

        [SerializeField, MMReadOnly]
        private int m_ActivateCount;
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
            m_ActivateCount++;
        }


    }
}
