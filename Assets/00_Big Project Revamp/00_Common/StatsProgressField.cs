using UnityEngine;
using static UnityEngine.Rendering.STP;

namespace Rush
{
    [System.Serializable]
    public partial class StatsProgressField : ProgressField
    {
        [SerializeField]
        private StatField m_BaseStat;
        [SerializeField]
        private StatField m_FlatScaleByLevel;
        [SerializeField]
        private StatField m_RateScaleByLevel;
        public StatField BaseStat => m_BaseStat;
        public StatField FlatScaleByLevel => m_FlatScaleByLevel;
        public StatField RateScaleByLevel => m_RateScaleByLevel;

        public virtual StatField GetFinalStat()
        {
            return StatField.GetFinalStatsByLevel(m_BaseStat, m_FlatScaleByLevel, m_RateScaleByLevel, LevelInternal);
        }
        public virtual StatField GetFinalStatScaledByOthers(StatsProgressField other)
        {
            StatField scaleBaseStat = other.BaseStat + m_BaseStat;
            scaleBaseStat += StatField.GetFinalFlatStats(m_FlatScaleByLevel, LevelInternal);
            scaleBaseStat += StatField.GetFinalRateStats(m_RateScaleByLevel, LevelInternal) * other.GetFinalStat();
            return scaleBaseStat;
        }
    }
}
