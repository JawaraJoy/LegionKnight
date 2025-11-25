using UnityEngine;

namespace LegionKnight
{
    public class DamageBuff : MonoBehaviour
    {
        [SerializeField]
        private DamageStat m_DamageStat;

        private Coroutine m_AttackRateTempCoroutine;
        public DamageStat GetDamageStat()
        {
            return m_DamageStat;
        }
        public void AddAttackRateTemp(float attackRate, float duration)
        {
            m_AttackRateTempCoroutine = StartCoroutine(m_DamageStat.AddAttackRateTemping(attackRate, duration));
        }
    }
}
