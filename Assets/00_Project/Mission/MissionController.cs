using AppsFlyerSDK;
using MoreMountains.Tools;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace LegionKnight
{
    public class MissionController : MonoBehaviour
    {
        [SerializeField]
        private string m_BehaviourName = "Daily Mission";
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
        
        public string BehaviourName => m_BehaviourName;
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

            Debug.Log("xxxINITxxx");

            foreach (var mission in m_Tasks)
            {
                Debug.Log(mission.Definition.Label);

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
            CheckTime();
            UpdateCurrentTaskPower();
            Debug.Log($"{DebugKey}: Init");
        }

        private void CheckTime()
        {
            if (m_ResetTime == null) return;
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
                    GameManager.Instance.DailyRewardManager?.ForceReset();
                }
            }
        }

        public virtual TaskStatus GetTaskStatus(TaskDefinition defi)
        {
            //Debug.Log("xxxStatusxxx");

            foreach (var task in m_Tasks)
            {
                //Debug.Log(task.Definition.Label);

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
            
            Debug.Log($"{DebugKey}; Update Task currentTaskPower{m_CurrentTaskPower}");
            foreach (var threshold in m_Thresholds)
            {
                threshold.Initialize(this);
            }
            m_OnControllerUpdate?.Invoke(this);
        }
    }

    
}
