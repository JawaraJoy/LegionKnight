using MoreMountains.Tools;
using UnityEngine;
using UnityEngine.Events;

namespace LegionKnight
{
    public class MissionController : MonoBehaviour
    {
        [SerializeField, MMReadOnly]
        private int m_MaxTaskPower;
        [SerializeField, MMReadOnly]
        private int m_CurrentTaskPower;
        [SerializeField]
        private TimerDefinition m_ResetTime;
        [SerializeField]
        private TaskThreshold[] m_Thresholds;
        [SerializeField]
        private TaskStatus[] m_Tasks;
        [SerializeField]
        private UnityEvent<MissionController> m_OnControllerUpdate;
        
        public TaskStatus[] Task => m_Tasks;
        public TaskThreshold[] TaskThresholds => m_Thresholds;
        public int MaxTaskPower => m_MaxTaskPower;
        public int CurrentTaskPower => m_CurrentTaskPower;
        public TimerDefinition ResetTime => m_ResetTime;
        private string TaskPowerKey => TaskStatus.Key + "taskpower";

        private const string DebugKey = "Mission";
        public void Init()
        {
            int savedPower = 0;
            int totalPoint = 0;
            foreach (var mission in m_Tasks)
            {
                mission.Init();
                mission.OnClaim.RemoveAllListeners();
                mission.OnClaim.AddListener(() => {
                    AddTaskPowerInternal(mission.Definition.TaskPower);
                });
                totalPoint += mission.Definition.TaskPower;

                if (mission.CurrentState == TaskState.Claimed)
                {
                    savedPower += mission.Definition.TaskPower;
                }
            }
            
            m_MaxTaskPower = totalPoint;
            SetTaskPowerInternal(savedPower);
            foreach (var threshold in m_Thresholds)
            {
                threshold.Initialize(this);
            }
            if (m_ResetTime != null)
            {
                bool hasResetTimer = UnityService.Instance.HasData(m_ResetTime.TimerId);
                if (!hasResetTimer)
                {
                    m_ResetTime.StartTimer();
                }
                else
                {
                    if (m_ResetTime.IsTimeToReset())
                    {
                        foreach (var mission in m_Tasks)
                        {
                            mission.Init();
                            mission.ResetToIntialState();
                        }
                        foreach (var thres in m_Thresholds)
                        {
                            thres.Reset();
                        }

                        SetTaskPowerInternal(0);
                        m_ResetTime.StartTimer();
                    }
                }
                
            }
            UpdateCurrentTaskPower();
            Debug.Log($"{DebugKey}: Init");
        }

        public TaskStatus GetTaskStatus(TaskDefinition defi)
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

        private void AddTaskPowerInternal(int point)
        {
            m_CurrentTaskPower += point;
            if (m_CurrentTaskPower > m_MaxTaskPower)
            {
                m_CurrentTaskPower = m_MaxTaskPower;
            }
            UpdateCurrentTaskPower();
        }
        public void AddTaskPoint(int point)
        {
            AddTaskPowerInternal(point);
        }
        private void SetTaskPowerInternal(int point)
        {
            m_CurrentTaskPower = point;
            if (m_CurrentTaskPower > m_MaxTaskPower)
            {
                m_CurrentTaskPower = m_MaxTaskPower;
            }
            UpdateCurrentTaskPower();
        }

        private void UpdateCurrentTaskPower()
        {
            float powerRate = (float)m_CurrentTaskPower / (float)m_MaxTaskPower;
            UnityService.Instance.SaveData(TaskPowerKey , m_CurrentTaskPower);
            m_OnControllerUpdate?.Invoke(this);
            Debug.Log($"{DebugKey}; Update Task currentTaskPower{m_CurrentTaskPower}");
        }
    }

    
}
