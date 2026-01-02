using UnityEngine;

namespace Rush
{
    [System.Serializable]
    public class StatsProgressField : ProgressField
    {
        [SerializeField]
        private StatField m_BaseStat;
        [SerializeField]
        private StatField m_FlatScaleByLevel;
        [SerializeField]
        private StatField m_PercentScaleByLevel;
        public StatField BaseStat => m_BaseStat;
        public StatField FlatScaleByLevel => m_FlatScaleByLevel;
        public StatField PercentScaleByLevel => m_PercentScaleByLevel;

        public virtual StatField GetFinalStat()
        {
            return StatField.GetFinalStatsByLevel(m_BaseStat, m_FlatScaleByLevel, m_PercentScaleByLevel, CurrentLevel);
        }
    }
}
