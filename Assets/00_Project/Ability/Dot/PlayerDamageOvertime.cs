using UnityEngine;

namespace LegionKnight
{
    public class PlayerDamageOvertime : DamageOvertime
    {
        
    }

    public partial class Player
    {
        [SerializeField]
        private PlayerDamageOvertime m_PlayerDamageOvertime;
        public void ApplyPlayerDamageOverTime(int damagePerSecond, float duration)
        {
            if (m_PlayerDamageOvertime != null)
            {
                m_PlayerDamageOvertime.ApplyDamageOverTime(damagePerSecond, duration);
            }
            else
            {
                Debug.LogWarning("PlayerDamageOvertime component is not assigned.");
            }
        }
        public void StopPlayerDamageOverTime()
        {
            if (m_PlayerDamageOvertime != null)
            {
                m_PlayerDamageOvertime.StopDamageOverTime();
            }
        }
    }
}
