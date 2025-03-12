using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace LegionKnight
{
    public class BosAttack : MonoBehaviour
    {
        [SerializeField]
        private int m_Damage = 1;
        [SerializeField]
        private int m_Health = 5;
        [SerializeField]
        private UnityEvent m_OnDeath = new();

        [SerializeField]
        private UnityEvent m_OnActiveSkill = new();

        [SerializeField]
        private List<Damageable> m_Damageables = new();

        private void Start()
        {
            GameManager.Instance.AddBossAttack(this);
            foreach (Damageable damageable in m_Damageables)
            {
                damageable.Init(m_Damage, m_Health);
            }
        }

        public void TakeDamage(int damage)
        {
            m_Health -= damage;
            Death();
        }
        private void OnDeathInvoke()
        {
            GameManager.Instance.RemoveBossAttack(this);
            m_OnDeath?.Invoke();
        }
        private void Death()
        {
            if (m_Health < 1)
            {
                OnDeathInvoke();
            }
        }

        public virtual void ActiveSkill()
        {
            OnActiveSkillInvoke();
        }

        protected virtual void OnActiveSkillInvoke()
        {
            m_OnActiveSkill?.Invoke();
        }
    }
}
