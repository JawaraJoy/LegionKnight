using UnityEngine;

namespace LegionKnight
{
    public class BossDamageOvertime : DamageOvertime
    {
        
    }

    public partial class BosEnemy
    {
        [SerializeField]
        private BossDamageOvertime m_BossDamageOvertime;
        public void ApplyBossDamageOverTime(int damagePerSecond, float duration)
        {
            if (m_BossDamageOvertime != null)
            {
                m_BossDamageOvertime.ApplyDamageOverTime(damagePerSecond, duration);
            }
            else
            {
                Debug.LogWarning("BossDamageOvertime component is not assigned.");
            }
        }
        public void StopBossDamageOverTime()
        {
            if (m_BossDamageOvertime != null)
            {
                m_BossDamageOvertime.StopDamageOverTime();
            }
        }
    }
}
