using System.Collections;
using UnityEngine;
using UnityEngine.Events;

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
        [SerializeField]
        private UnityEvent<AbilityContext> m_OnAttackStart;
        public UnityEvent<AbilityContext> OnAttackStart => m_OnAttackStart;
        [SerializeField]
        private UnityEvent<AbilityContext> m_OnAttackDone;
        public UnityEvent<AbilityContext> OnAttackDone => m_OnAttackDone;
        private AbilityContext m_AbilityContext;
        public AbilityContext AbilityContext => m_AbilityContext;
        public void SetIsTrueDamage(bool isTrueDamage)
        {
            m_IsTrueDamage = isTrueDamage;
        }
        public void SetIsFatalDamage(bool isFatalDamage)
        {
            m_IsFatalDamage = isFatalDamage;
        }
        
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
        private void OnAttackStartInvoke()
        {
            m_OnAttackStart?.Invoke(m_AbilityContext);
        }
        private void OnAttackDoneInvoke()
        {
            m_OnAttackDone?.Invoke(m_AbilityContext);
        }
    }
}
