using UnityEngine;

namespace Rush
{
    [System.Serializable]
    public partial class StatsField 
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
        protected virtual StatField GetFinalStatInternal(int level)
        {
            return StatField.GetFinalStatsByLevel(m_BaseStat, m_FlatScaleByLevel, m_RateScaleByLevel, level);
        }
        public StatField GetFinalStat(int level)
        {
            return StatField.GetFinalStatsByLevel(m_BaseStat, m_FlatScaleByLevel, m_RateScaleByLevel, level);
        }
        public virtual StatField GetFinalStatScaledByOthers(int ownLevel, StatsField[] otherStats)
        {
            StatField combinedOthersFinalStat = StatField.Zero;
            StatField ownFinalStat = GetFinalStatInternal(ownLevel);

            foreach (StatsField otherStat in otherStats)
            {
                StatField otherBaseStat = otherStat.m_BaseStat;
                StatField otherFlatScale = StatField.GetFinalFlatStats(otherStat.m_FlatScaleByLevel, ownLevel);
                StatField otherRateScale = StatField.GetFinalRateStats(otherStat.m_RateScaleByLevel, ownLevel);

                StatField combineFinalFlatScale = otherBaseStat + otherFlatScale;
                StatField totalAmountByRateScale = ownFinalStat * otherRateScale;
                combinedOthersFinalStat += combineFinalFlatScale + totalAmountByRateScale;
            }
            return combinedOthersFinalStat + ownFinalStat;
        }
    }
}
