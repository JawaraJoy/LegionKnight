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
            StartCoroutine(m_DamageStat.AddAttackRateTemping(attackRate, duration));
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
