using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace LegionKnight
{
    public class DamageBuff : MonoBehaviour
    {
        [SerializeField]
        private AbilityDefinition m_DamageDefinition;
        [SerializeField]
        private DamageStat m_DamageStat;

        [SerializeField]
        private UnityEvent m_OnBuffStart;
        [SerializeField]
        private UnityEvent m_OnBuffEnd;

        [SerializeField]
        private ParticleSystem m_AttackRateBuffVFX;
        public DamageStat GetDamageStat()
        {
            return m_DamageStat;
        }
        public void AddAttackRateTemp(float attackRate, float duration)
        {
            StartCoroutine(AddAttackRateTemping(attackRate, duration));
        }
        public void AddAttackTemp(int atk, float duration)
        {
            StartCoroutine(AddAttackTemping(atk, duration));
        }
        private IEnumerator AddAttackRateTemping(float attackRate, float duration)
        {
            m_DamageStat.AddAttackRate(attackRate);
            OnBuffStartInvoke();
            yield return new WaitForSeconds(duration);
            m_DamageStat.AddAttackRate(-attackRate);
            OnBuffEndInvoke();
        }
        public IEnumerator AddAttackTemping(int attack, float duration)
        {
            m_DamageStat.AddAttack(attack);
            OnBuffStartInvoke();
            yield return new WaitForSeconds(duration);
            m_DamageStat.AddAttack(-attack);
            OnBuffEndInvoke();
        }

        private void OnBuffStartInvoke()
        {
            m_OnBuffStart?.Invoke();
        }
        private void OnBuffEndInvoke()
        {
            m_OnBuffEnd?.Invoke();
        }
    }
}
