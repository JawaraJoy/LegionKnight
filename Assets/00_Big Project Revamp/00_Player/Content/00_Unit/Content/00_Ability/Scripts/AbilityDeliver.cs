using MoreMountains.Tools;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Rush
{
    public abstract partial class AbilityDeliver : MonoBehaviour
    {
        [SerializeField]
        protected AbilityConfig m_Config;
        [SerializeField, MMReadOnly]
        protected AbilityContext m_AbilityContext;
        [SerializeField]
        protected Transform m_VfxSpawnPost;
        [SerializeField, MMReadOnly]
        protected AbilityPurpose m_Purpose;
        [SerializeField]
        private UnityEvent<AbilityContext> m_OnInit;
        [SerializeField]
        private UnityEvent<AbilityContext> m_OnActivate;
        public AbilityConfig Config => m_Config;
        public AbilityContext AbilityContext => m_AbilityContext;
        public Transform VfxSpawnPost => m_VfxSpawnPost;
        public AbilityPurpose Purpose => m_Purpose;
        protected List<Targetable> GetTargetsInternal()
        {
            List<Targetable> damageables = new(AbilityUltility.GetTargetables(m_AbilityContext));
            return damageables;
        }
        public virtual void Init(AbilityConfig config, SkillContext context)
        {
            m_AbilityContext = new AbilityContext(this, context);
            m_Config = config;
            m_OnInit?.Invoke(m_AbilityContext);
        }
        protected void SetPurposeInternal(AbilityPurpose purpose)
        {
            m_Purpose = purpose;
        }
        public virtual void Activate()
        {
            m_OnActivate?.Invoke(m_AbilityContext);
        }
    }
}
