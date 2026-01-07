using MoreMountains.Tools;
using System.Collections.Generic;
using UnityEngine;

namespace Rush
{
    public partial class AbilityDeliver : MonoBehaviour
    {
        [SerializeField, MMReadOnly]
        private AbilityContext m_AbilityContext;
        [SerializeField, MMReadOnly]
        private List<Targetable> m_AttackTargets = new();
        public List<Targetable> AttackTargets => m_AttackTargets;
        public void Init(AbilityContext context)
        {
            m_AbilityContext = context;
            m_AbilityContext.SetDeliver(this);
        }
        public void Activate()
        {
            m_AttackTargets.Clear();
            List<Targetable> damageables = new(AbilityUltility.GetTargetables(m_AbilityContext.SkillContext, m_AbilityContext.AbilityConfig));
            m_AttackTargets.AddRange(damageables);
        }
    }
}
