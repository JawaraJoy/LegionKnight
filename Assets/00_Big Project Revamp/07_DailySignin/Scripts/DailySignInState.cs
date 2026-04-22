using System;

namespace Rush
{
    public class DailySignInState
    {
        private readonly int m_CurrentDay;       // 0-based index
        private readonly bool m_CanClaimToday;
        private readonly bool m_CycleComplete;
        private readonly DateTime m_NextResetTime;
        private readonly int m_TotalDays;

        public int CurrentDay => m_CurrentDay;
        public bool CanClaimToday => m_CanClaimToday;
        public bool CycleComplete => m_CycleComplete;
        public DateTime NextResetTime => m_NextResetTime;
        public int TotalDays => m_TotalDays;

        // How many seconds until next reset
        public double SecondsUntilReset =>
            (m_NextResetTime - DateTime.Now).TotalSeconds;

        public DailySignInState(int currentDay, bool canClaimToday,
            bool cycleComplete, DateTime nextResetTime, int totalDays)
        {
            m_CurrentDay = currentDay;
            m_CanClaimToday = canClaimToday;
            m_CycleComplete = cycleComplete;
            m_NextResetTime = nextResetTime;
            m_TotalDays = totalDays;
        }
    }
}