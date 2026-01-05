using UnityEngine;

namespace Rush
{
    [System.Serializable]
    public partial class StatField
    {
        [SerializeField]
        private float m_Health;
        [SerializeField]
        private float m_Attack;
        [SerializeField]
        private float m_Defense;
        public float Health => m_Health;
        public float Attack => m_Attack;
        public float Defense => m_Defense;

        public static StatField operator +(StatField a, StatField b)
        {
            return new StatField
            {
                m_Health = a.m_Health + b.m_Health,
                m_Attack = a.m_Attack + b.m_Attack,
                m_Defense = a.m_Defense + b.m_Defense,
            };
        }
        public static StatField operator *(StatField a, StatField b)
        {
            return new StatField
            {
                m_Health = a.m_Health * b.m_Health,
                m_Attack = a.m_Attack * b.m_Attack,
                m_Defense = a.m_Defense * b.m_Defense,
            };
        }
        public static StatField operator *(StatField a, int scalar)
        {
            return new StatField
            {
                m_Health = a.m_Health * scalar,
                m_Attack = a.m_Attack * scalar,
                m_Defense = a.m_Defense * scalar,
            };
        }
        private static StatField GetFinalFlatStatsInternal(StatField flatScaleByLevel, int level)
        {
            return new StatField
            {
                m_Health = flatScaleByLevel.m_Health * (level - 1),
                m_Attack = flatScaleByLevel.m_Attack * (level - 1),
                m_Defense = flatScaleByLevel.m_Defense * (level - 1),
            };
        }
        private static StatField GetFinalRateStatsInternal(StatField rateScaleByLevel, int level)
        {
            return new StatField
            {
                m_Health = rateScaleByLevel.m_Health * (level - 1),
                m_Attack = rateScaleByLevel.m_Attack * (level - 1),
                m_Defense = rateScaleByLevel.m_Defense * (level - 1),
            };
        }
        public static StatField GetFinalFlatStats(StatField flatScaleByLevel, int level)
        {
            return GetFinalFlatStatsInternal(flatScaleByLevel, level);
        }
        public static StatField GetFinalRateStats(StatField rateScaleByLevel, int level)
        {
            return GetFinalRateStatsInternal(rateScaleByLevel, level);
        }
        public static StatField GetFinalStatsByLevel(StatField baseStats, StatField flatScaleByLevel, StatField rateScaleByLevel, int level)
        {
            StatField finalFlatStats = GetFinalFlatStatsInternal(flatScaleByLevel, level);
            StatField finalRateStats = GetFinalRateStatsInternal(rateScaleByLevel, level);
            StatField finalStats = baseStats + finalFlatStats + finalRateStats;
            return finalStats;
        }
    }
}
