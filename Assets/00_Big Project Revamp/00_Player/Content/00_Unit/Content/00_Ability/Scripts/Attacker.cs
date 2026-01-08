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
        public void SetIsTrueDamage(bool isTrueDamage)
        {
            m_IsTrueDamage = isTrueDamage;
        }
        public void SetIsFatalDamage(bool isFatalDamage)
        {
            m_IsFatalDamage = isFatalDamage;
        }

        private AbilityContext m_AbilityContext;
        public AbilityContext SkillContext => m_AbilityContext;
        public void Init(AbilityContext context)
        {
            m_AbilityContext = context;
            float damage = AbilityUltility.GetFinalEffectAmount(m_AbilityContext);
            AbilityConfig config = context.AbilityDeliver.Config;
            if (config is DamageAbilityConfig damageConfig)
            {
                m_IsFatalDamage = damageConfig.IsFatalDamage;
                m_IsTrueDamage = damageConfig.IsTrueDamage;
            }
            m_Damage = Mathf.RoundToInt(damage);

        }
    }
}
