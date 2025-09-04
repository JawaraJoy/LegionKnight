using UnityEngine;
using UnityEngine.Events;

namespace LegionKnight
{
    public class MissionController : MonoBehaviour
    {
        [SerializeField]
        private int m_MaxTaskPoint;
        [SerializeField]
        private int m_CurrentTaskPoint;
        [SerializeField]
        private TaskStatus[] m_Tasks;
        
        public TaskStatus[] Task => m_Tasks;
        private string TaskPointKey => TaskStatus.Key + "taskpoint";
        public void Init()
        {
            bool hasTaskPoint = UnityService.Instance.HasData(TaskPointKey);
            if (hasTaskPoint)
            {
                int savedPoint = UnityService.Instance.GetData<int>(TaskPointKey);
                SetTaskPointInternal(savedPoint);
            }
            int totalPoint = 0;
            foreach (var mission in m_Tasks)
            {
                mission.Init();
                totalPoint += mission.Definition.DifficultyScore;
            }
            m_MaxTaskPoint = totalPoint;
        }
        public TaskStatus GetTask(TaskDefinition defi)
        {
            foreach (var task in m_Tasks)
            {
                if (task.Definition == defi)
                {
                    return task;
                }
            }
            return null;
        }
        public void AddTaskPoint(int point)
        {
            m_CurrentTaskPoint += point;
            if (m_CurrentTaskPoint > m_MaxTaskPoint)
            {
                m_CurrentTaskPoint = m_MaxTaskPoint;
            }
            UpdateCurrentTaskPoint();
        }
        private void SetTaskPointInternal(int point)
        {
            m_CurrentTaskPoint = point;
            if (m_CurrentTaskPoint > m_MaxTaskPoint)
            {
                m_CurrentTaskPoint = m_MaxTaskPoint;
            }
            UpdateCurrentTaskPoint();
        }

        private void UpdateCurrentTaskPoint()
        {
            UnityService.Instance.SaveData(TaskPointKey , m_CurrentTaskPoint);
        }
    }

    [System.Serializable]
    public class TaskStatus
    {
        [SerializeField]
        private TaskDefinition m_Definition;
        [SerializeField]
        private int m_CurrentScore;
        [SerializeField]
        private TaskState m_CurrentState = TaskState.Locked;
        [SerializeField]
        private UnityEvent m_OnCompleted;
        public TaskDefinition Definition => m_Definition;
        public int CurrentScore => m_CurrentScore;
        public TaskState CurrentState => m_CurrentState;
        private bool IsCompletedInternal => m_CurrentScore >= m_Definition.TargetScore;
        public bool IsCompleted => IsCompletedInternal;

        private const string MISSION_KEY = "mission_";
        public static string Key => MISSION_KEY;
        private string KeyWithId => Key + m_Definition.Id;
        private string KeyWithIdAndScore => KeyWithId + "_score";
        private string KeyWithIdAndState => KeyWithId + "_state";
        public void Init()
        {
            m_CurrentState = m_Definition.InitialState;
            bool hasState = UnityService.Instance.HasData(KeyWithIdAndState);
            bool hasScore = UnityService.Instance.HasData(KeyWithIdAndScore);
            if (hasScore)
            {
                m_CurrentScore = UnityService.Instance.GetData<int>(KeyWithIdAndScore);
            }
            if (hasState)
            {
                m_CurrentState = (TaskState)UnityService.Instance.GetData<int>(KeyWithIdAndState);
            }

            TimerDefinition resetTime = m_Definition.ResetTime;
            if (resetTime != null)
            {
                bool hasTimeReset = m_Definition.ResetTime.IsTimeToReset();
                if (hasTimeReset)
                {
                    ResetToInitialStateInternal();
                    m_Definition.ResetTime.StartTimer();
                }
            }
        }
        public void AddScore(int score)
        {
            m_CurrentScore += score;
            if (m_CurrentScore > m_Definition.TargetScore)
            {
                m_CurrentScore = m_Definition.TargetScore;
            }
            UpdateScore();
        }
        private void UpdateScore()
        {
            if (IsCompletedInternal)
            {
                m_OnCompleted?.Invoke();
                SetStateInternal(TaskState.Completed);
            }
            else
            {
                Debug.Log($"{MISSION_KEY}{m_Definition.Id} progress: {m_CurrentScore}/{m_Definition.TargetScore}");
                SetStateInternal(TaskState.OnProgress);
            }
            UnityService.Instance.SaveData(KeyWithIdAndScore, m_CurrentScore);
        }
        private void SetCurrentScoreInternal(int score)
        {
            m_CurrentScore = score;
            if (m_CurrentScore > m_Definition.TargetScore)
            {
                m_CurrentScore = m_Definition.TargetScore;
            }
            UpdateScore();
        }
        private void ResetToInitialStateInternal()
        {
            SetCurrentScoreInternal(0);
            SetStateInternal(m_Definition.InitialState);
            UnityService.Instance.SaveData(KeyWithIdAndScore, m_CurrentScore);
        }
        private void SetStateInternal(TaskState state)
        {
            m_CurrentState = state;
            UnityService.Instance.SaveData(KeyWithIdAndState, (int)m_CurrentState);
        }
        public void SetState(TaskState state)
        {
            SetStateInternal(state);
        }
        public void ResetToIntialState()
        {
            ResetToInitialStateInternal();
        }
        public void SetScore(int score)
        {
            SetCurrentScoreInternal(score);
        }
        public void DirectClaimRewards()
        {
            if (m_CurrentState == TaskState.Completed)
            {
                LootField[] rewards = m_Definition.Rewards.LootFields;
                foreach (var loot in rewards)
                {
                    loot.DirectTakeLoot();
                }
                SetStateInternal(TaskState.Claimed);
            }
        }
    }

    public enum TaskState
    {
        Locked = 1,
        Unlocked = 2,
        Selected = 3,
        OnProgress = 4,
        Completed = 5,
        Claimed = 6
    }
}
