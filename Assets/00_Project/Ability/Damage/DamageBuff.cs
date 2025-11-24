using UnityEngine;

namespace LegionKnight
{
    public class DamageBuff : MonoBehaviour
    {
        [SerializeField]
        private DamageStat m_DamageStat;

        public DamageStat GetDamageStat()
        {
            return m_DamageStat;
        }
        public void AddAttackRateTemp(float attackRate, float duration)
        {
            StartCoroutine(m_DamageStat.AddAttackRateTemping(attackRate, duration));
        }
    }
}
