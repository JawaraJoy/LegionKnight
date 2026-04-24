using UnityEngine;

namespace Rush
{
    [CreateAssetMenu(fileName = "QuestCatalog_", menuName = "Rush/Quest/Catalog")]
    public class QuestCatalogConfig : Configuration
    {
        [Header("Tasks")]
        [SerializeField] private QuestTaskConfig[] m_Tasks;

        [Header("Reset — applies to all tasks in this catalog")]
        [SerializeField] private QuestResetCycle m_ResetCycle = QuestResetCycle.Daily;
        [SerializeField, Range(0, 23)] private int m_ResetHour = 0;
        [SerializeField, Range(0, 59)] private int m_ResetMinute = 0;

        [Tooltip("Only used if ResetCycle is Weekly")]
        [SerializeField] private SignInDayOfWeek m_WeeklyResetDay = SignInDayOfWeek.Monday;
        public QuestTaskConfig[] Tasks => m_Tasks;
        public QuestResetCycle ResetCycle => m_ResetCycle;
        public int ResetHour => m_ResetHour;
        public int ResetMinute => m_ResetMinute;
        public SignInDayOfWeek WeeklyResetDay => m_WeeklyResetDay;

        private void OnValidate()
        {
            ValidateCatalogToTask();
        }

        private void ValidateCatalogToTask()
        {
            foreach (var task in m_Tasks)
            {
                task.SetCatalog(this);
            }
        }
    }
}