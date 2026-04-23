using System;

namespace Rush
{
    public class QuestTaskState
    {
        private readonly QuestTaskConfig m_Config;
        private readonly int m_CurrentCount;
        private readonly bool m_IsComplete;
        private readonly bool m_IsClaimed;
        private readonly DateTime m_NextResetTime;

        public QuestTaskConfig Config => m_Config;
        public int CurrentCount => m_CurrentCount;
        public int TargetCount => m_Config.TargetCount;
        public bool IsComplete => m_IsComplete;
        public bool IsClaimed => m_IsClaimed;
        public bool CanClaim => m_IsComplete && !m_IsClaimed;
        public DateTime NextResetTime => m_NextResetTime;

        public double SecondsUntilReset =>
            (m_NextResetTime - DateTime.Now).TotalSeconds;

        public QuestTaskState(QuestTaskConfig config, int currentCount,
            bool isComplete, bool isClaimed, DateTime nextResetTime)
        {
            m_Config = config;
            m_CurrentCount = currentCount;
            m_IsComplete = isComplete;
            m_IsClaimed = isClaimed;
            m_NextResetTime = nextResetTime;
        }
    }
}