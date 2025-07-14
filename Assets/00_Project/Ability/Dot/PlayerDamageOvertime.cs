using UnityEngine;

namespace LegionKnight
{
    public class PlayerDamageOvertime : DamageOvertime
    {
        [SerializeField]
        private int m_AntidotCount;
        private int m_CurrentAntidotCount;

        public void AddAntidot(int count)
        {
            m_CurrentAntidotCount += count;
            if (m_CurrentAntidotCount > m_AntidotCount)
            {
                m_CurrentAntidotCount = 0;
                StopDamageOverTimeInternal();
            }
        }

        protected override void ApplyDamageOverTimeInternal(int damagePerSecond, float duration)
        {
            base.ApplyDamageOverTimeInternal(damagePerSecond, duration);
            m_CurrentAntidotCount = 0;
        }
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
        public void AddAntidot(int count)
        {
            if (m_PlayerDamageOvertime != null)
            {
                m_PlayerDamageOvertime.AddAntidot(count);
            }
            else
            {
                Debug.LogWarning("PlayerDamageOvertime component is not assigned.");
            }
        }
    }
}
