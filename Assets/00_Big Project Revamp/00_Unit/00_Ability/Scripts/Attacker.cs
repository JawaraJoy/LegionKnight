using MoreMountains.Tools;
using System.Collections.Generic;
using UnityEngine;

namespace Rush
{
    public partial class Attacker : MonoBehaviour, IAbility
    {
        [SerializeField]
        private int m_Damage = 10;
        [SerializeField]
        private bool m_IsTrueDamage = false;
        public int Damage => m_Damage;
        public bool IsTrueDamage => m_IsTrueDamage;

        private AbilityContext m_AbilityContext;
        public AbilityContext AbilityContext => m_AbilityContext;
        [SerializeField]
        private List<Targetable> m_AttackTargets = new();
        public List<Targetable> AttackTargets => m_AttackTargets;
        public void Init(AbilityContext context)
        {
            m_AbilityContext = context;
            float damage = AbilityUltility.GetFinalEffectAmount(m_AbilityContext, AbilityPurpose.Damaging);
            m_Damage = Mathf.RoundToInt(damage);
        }
        public void SearchTargets()
        {
            m_AttackTargets.Clear();
            List<Targetable> damageables = new (AbilityUltility.GetTargetables(m_AbilityContext, AbilityPurpose.Damaging));
            m_AttackTargets.AddRange(damageables);
        }
    }
}
