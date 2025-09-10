using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

namespace LegionKnight
{
    [CreateAssetMenu(fileName = "Task", menuName = "Legion Knight/Mission/Task", order = 1)]
    public class TaskDefinition : ScriptableObject
    {
        [SerializeField]
        private string m_Id;
        [SerializeField]
        private string m_Label;
        [SerializeField, TextArea]
        private string m_Description;
        [SerializeField]
        private TaskState m_InitialState = TaskState.Locked;
        [SerializeField]
        private MissionCategory m_MissionCategory = MissionCategory.Daily;
        [SerializeField]
        private int m_TargetScore = 1;
        [SerializeField]
        private int m_TaskPower = 10;
        [SerializeField]
        private TimerDefinition m_ResetTime;
        [SerializeField]
        private LootDefinition m_Rewards;
        public string Id => m_Id;
        public string Label => m_Label;
        public string Description => m_Description;
        public TaskState InitialState => m_InitialState;
        public int TargetScore => m_TargetScore;
        public int TaskPower => m_TaskPower;
        public TimerDefinition ResetTime => m_ResetTime;
        public LootDefinition Rewards => m_Rewards;

        public void AddDailyScore(int score)
        {
            TaskStatus status = GetTaskStatus();
            status?.AddScore(score);
        }
        public void SetDailyState(TaskState state)
        {
            TaskStatus status = GetTaskStatus();
            status?.SetState(state);
        }
        public void ResetDailyToIntialState()
        {
            TaskStatus status = GetTaskStatus();
            status?.ResetToIntialState();
        }
        public void SetDailyScore(int score)
        {
            TaskStatus status = GetTaskStatus();
            status?.SetScore(score);
        }
        public void DirectDailyClaimRewards()
        {
            TaskStatus status = GetTaskStatus();
            status?.DirectClaimRewards();
        }

        private TaskStatus GetTaskStatus()
        {
            TaskStatus status = Player.Instance.DailyMissionManager.GetTaskStatus(this);
            switch (m_MissionCategory)
            {
                case MissionCategory.Daily:
                    break;
                case MissionCategory.Weekly:
                    status = Player.Instance.WeeklyMissionManager.GetTaskStatus(this);
                    break;
            }
            return status;
        }
    }

    public enum MissionCategory
    {
        Daily = 0,
        Weekly = 1,
    }
}
