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
        [SerializeField]
        private string m_Description;
        [SerializeField]
        private TaskState m_InitialState = TaskState.Locked;
        [SerializeField]
        private int m_TargetScore = 1;
        [SerializeField]
        private int m_DifficultyScore = 10;
        [SerializeField]
        private TimerDefinition m_ResetTime;
        [SerializeField]
        private LootDefinition m_Rewards;
        public string Id => m_Id;
        public string Label => m_Label;
        public string Description => m_Description;
        public TaskState InitialState => m_InitialState;
        public int TargetScore => m_TargetScore;
        public int DifficultyScore => m_DifficultyScore;
        public TimerDefinition ResetTime => m_ResetTime;
        public LootDefinition Rewards => m_Rewards;

        public void AddScore(int score)
        {
            TaskStatus status = Player.Instance.DailyMissionManager.GetTask(this);
            status?.AddScore(score);
        }
        public void SetState(TaskState state)
        {
            TaskStatus status = Player.Instance.DailyMissionManager.GetTask(this);
            status?.SetState(state);
        }
        public void ResetToIntialState()
        {
            TaskStatus status = Player.Instance.DailyMissionManager.GetTask(this);
            status?.ResetToIntialState();
        }
        public void SetScore(int score)
        {
            TaskStatus status = Player.Instance.DailyMissionManager.GetTask(this);
            status?.SetScore(score);
        }
        public void DirectClaimRewards()
        {
            TaskStatus status = Player.Instance.DailyMissionManager.GetTask(this);
            status?.DirectClaimRewards();
        }
    }
}
