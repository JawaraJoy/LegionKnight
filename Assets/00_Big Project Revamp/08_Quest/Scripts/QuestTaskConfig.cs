using UnityEngine;

namespace Rush
{
    [CreateAssetMenu(fileName = "QuestTask_", menuName = "Rush/Quest/Task")]
    public class QuestTaskConfig : Configuration
    {
        [Header("Task")]
        [SerializeField] private int m_TargetCount = 1;

        [Header("Reward")]
        [SerializeField] private CollectibleConfig m_RewardCollectible;
        [SerializeField] private int m_RewardAmount = 1;

        [Header("Reset")]
        [SerializeField] private QuestResetCycle m_ResetCycle = QuestResetCycle.Daily;
        [SerializeField, Range(0, 23)] private int m_ResetHour = 0;
        [SerializeField, Range(0, 59)] private int m_ResetMinute = 0;
        [SerializeField] private SignInDayOfWeek m_WeeklyResetDay = SignInDayOfWeek.Monday;

        public int TargetCount => m_TargetCount;
        public CollectibleConfig RewardCollectible => m_RewardCollectible;
        public int RewardAmount => m_RewardAmount;
        public QuestResetCycle ResetCycle => m_ResetCycle;
        public int ResetHour => m_ResetHour;
        public int ResetMinute => m_ResetMinute;
        public SignInDayOfWeek WeeklyResetDay => m_WeeklyResetDay;
    }
}