using MoreMountains.Tools;
using System.Collections.Generic;
using UnityEngine;

namespace Rush
{
    public partial class Attacker : MonoBehaviour
    {
        [SerializeField]
        private int m_Damage = 10;
        [SerializeField]
        private bool m_IsTrueDamage = false;
        [SerializeField]
        private bool m_IsFatalDamage = false;
        public int Damage => m_Damage;
        public bool IsTrueDamage => m_IsTrueDamage;
        public bool IsFatalDamage => m_IsFatalDamage;

        private AbilityContext m_AbilityContext;
        public AbilityContext SkillContext => m_AbilityContext;
        [SerializeField]
        private List<Targetable> m_AttackTargets = new();
        public List<Targetable> AttackTargets => m_AttackTargets;
        public void Init(AbilityContext context)
        {
            m_AbilityContext = context;
            float damage = AbilityUltility.GetFinalEffectAmount(m_AbilityContext.SkillContext, m_AbilityContext.AbilityConfig);
            m_Damage = Mathf.RoundToInt(damage);
        }
        public void SearchTargets()
        {
            m_AttackTargets.Clear();
            List<Targetable> damageables = new (AbilityUltility.GetTargetables(m_AbilityContext.SkillContext, m_AbilityContext.AbilityConfig));
            m_AttackTargets.AddRange(damageables);
        }
    }
}
