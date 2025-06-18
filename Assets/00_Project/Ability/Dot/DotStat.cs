using UnityEngine;

namespace LegionKnight
{
    [System.Serializable]
    public class DotStat
    {
        [SerializeField]
        private int m_DamagePerTick = 1;
        [SerializeField]
        private float m_Duration = 5f;

        [SerializeField]
        private int m_DamagePerTickUpgrade = 1;
        [SerializeField]
        private float m_DurationUpgrade = 0.5f;

        public int DamagePerTick => m_DamagePerTick;
        public float Duration => m_Duration;
        public int DamagePerTickUpgrade => m_DamagePerTickUpgrade;
        public float DurationUpgrade => m_DurationUpgrade;

        public int GetFinalDamagePerTick(int level)
        {
            return m_DamagePerTick + m_DamagePerTickUpgrade * level;
        }
        public float GetFinalDuration(int level)
        {
            return m_Duration + m_DurationUpgrade * level;
        }

        public void SetDamagePerTick(int damagePerTick)
        {
            m_DamagePerTick = damagePerTick;
        }
        public void SetDuration(float duration)
        {
            m_Duration = duration;
        }
        public void SetDamagePerTickUpgrade(int damagePerTickUpgrade)
        {
            m_DamagePerTickUpgrade = damagePerTickUpgrade;
        }
        public void SetDurationUpgrade(float durationUpgrade)
        {
            m_DurationUpgrade = durationUpgrade;
        }
        public DotStat(int damagePerTick, float duration, int damagePerTickUpgrade = 1, float durationUpgrade = 0.5f)
        {
            m_DamagePerTick = damagePerTick;
            m_Duration = duration;
            m_DamagePerTickUpgrade = damagePerTickUpgrade;
            m_DurationUpgrade = durationUpgrade;
        }

        public void ApplyDamageOvertime(DamageOvertime damageOvertime)
        {
            if (damageOvertime == null) return;
            damageOvertime.ApplyDamageOverTime(m_DamagePerTick, m_Duration);
        }
    }

    public partial class AbilityDefinition
    {
        [SerializeField]
        private DotStat m_DotStat;
        public int DotDamagePerTick => m_DotStat.DamagePerTick;
        public float DotDuration => m_DotStat.Duration;
        public int DotDamagePerTickUpgrade => m_DotStat.DamagePerTickUpgrade;
        public float DotDurationUpgrade => m_DotStat.DurationUpgrade;
        public int GetFinalDotDamagePerTick(int level)
        {
            return m_DotStat.GetFinalDamagePerTick(level);
        }
        public float GetFinalDotDuration(int level)
        {
            return m_DotStat.GetFinalDuration(level);
        }
    }
}
