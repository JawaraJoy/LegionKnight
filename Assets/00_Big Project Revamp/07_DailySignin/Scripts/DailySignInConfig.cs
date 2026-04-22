using UnityEngine;

namespace Rush
{
    public enum SignInResetCycle
    {
        Weekly,  // resets every week
        Monthly  // resets every month
    }

    public enum SignInDayOfWeek
    {
        Monday = 1,
        Tuesday = 2,
        Wednesday = 3,
        Thursday = 4,
        Friday = 5,
        Saturday = 6,
        Sunday = 7
    }

    [CreateAssetMenu(fileName = "DailySignInConfig_", menuName = "Rush/Daily Sign In/Config")]
    public class DailySignInConfig : Configuration
    {
        [Header("Rewards")]
        [Tooltip("One entry per day. 7 entries = weekly, 28 entries = monthly, etc.")]
        [SerializeField] private DailySignInRewardEntry[] m_Rewards;

        [Header("Reset Cycle")]
        [SerializeField] private SignInResetCycle m_ResetCycle = SignInResetCycle.Weekly;

        [Tooltip("Loop back to day 1 after all days are claimed, or stop until next cycle")]
        [SerializeField] private bool m_LoopOnComplete = false;

        [Header("Reset Time")]
        [Tooltip("Hour of day when the cycle resets (0-23)")]
        [SerializeField, Range(0, 23)] private int m_ResetHour = 0;

        [Tooltip("Minute of hour when the cycle resets (0-59)")]
        [SerializeField, Range(0, 59)] private int m_ResetMinute = 0;

        [Header("Weekly Reset — only used if ResetCycle is Weekly")]
        [SerializeField] private SignInDayOfWeek m_WeeklyResetDay = SignInDayOfWeek.Monday;

        [Header("Monthly Reset — only used if ResetCycle is Monthly")]
        [Tooltip("Day of month when cycle resets (1-28). Clamped to 28 for safety.")]
        [SerializeField, Range(1, 28)] private int m_MonthlyResetDay = 1;

        public DailySignInRewardEntry[] Rewards => m_Rewards;
        public SignInResetCycle ResetCycle => m_ResetCycle;
        public bool LoopOnComplete => m_LoopOnComplete;
        public int ResetHour => m_ResetHour;
        public int ResetMinute => m_ResetMinute;
        public SignInDayOfWeek WeeklyResetDay => m_WeeklyResetDay;
        public int MonthlyResetDay => m_MonthlyResetDay;

        public int TotalDays => m_Rewards?.Length ?? 0;
    }
}