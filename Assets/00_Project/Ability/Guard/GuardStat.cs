using UnityEngine;

namespace LegionKnight
{
    [System.Serializable]
    public class GuardStat
    {
        [SerializeField]
        private float m_Duration = 5f;
        [SerializeField]
        private float m_DurationUpgrade = 1f;

        public float Duration => m_Duration;
        public float DurationUpgrade => m_DurationUpgrade;
        public float GetFinalDuration(int level)
        {
            return m_Duration + m_DurationUpgrade * level;
        }
        public void SetDuration(float duration)
        {
            m_Duration = duration;
        }
        public void SetDurationUpgrade(float durationUpgrade)
        {
            m_DurationUpgrade = durationUpgrade;
        }
        public GuardStat(float duration, float upgrade)
        {
            m_Duration = duration;
            m_DurationUpgrade = upgrade;
        }
    }

    public partial class AbilityDefinition
    {
        [SerializeField]
        private GuardStat m_GuardStat;
        public float GuardDuration => m_GuardStat.Duration;
        public float GuardDurationUpgrade => m_GuardStat.DurationUpgrade;
        public float GetFinalGuardDuration(int level)
        {
            return m_GuardStat.GetFinalDuration(level);
        }
    }
}
