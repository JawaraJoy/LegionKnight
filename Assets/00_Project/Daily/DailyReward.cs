using MoreMountains.Tools;
using UnityEngine;
using System;
using UnityEngine.Events;

namespace LegionKnight
{
    public class DailyReward : MonoBehaviour
    {
        [SerializeField]
        private string m_BehaviourName = "Daily Sign In";
        [SerializeField]
        private TimerDefinition m_Timer;
        [SerializeField]
        private UnityEvent<DailyRewardData> m_OnRewardClaimed;
        [SerializeField]
        private DailyRewardData[] m_Rewards;
        public string BehaviourName => m_BehaviourName;
        public TimerDefinition Timer => m_Timer;
        public UnityEvent<DailyRewardData> OnRewardClaimed => m_OnRewardClaimed;

        private const string DailyRewardKeyInternal = "dailyreward";
        public static string DailyRewardKey => DailyRewardKeyInternal;

        private DailyRewardData GetDailyRewardDataInternal(LootDefinition loot)
        {
            foreach (var reward in m_Rewards)
            {
                Debug.Log(reward);

                if (reward.Reward == loot)
                {
                    return reward;
                }
            }
            return null;
        }

        public DailyRewardData GetDailyRewardData(LootDefinition loot)
        {
            return GetDailyRewardDataInternal(loot);
        }

        public void Refresh()
        {
            RefreshInternal();
        }

        [Obsolete("Not Reset on specific Day, call forceReset to other function")]
        private void RefreshInternal()
        {
            if (m_Timer == null)
            {
                Debug.LogWarning($"{DailyRewardKeyInternal}: Timer is not set.");
                return;
            }
            bool hasResetTime = UnityService.Instance.HasData(m_Timer.TimerId);

            if (hasResetTime)
            {
                bool isReset = m_Timer.IsTimeToReset();
                if (isReset)
                {
                    OnTimerResetInternal();
                    
                }
                else
                {
                    Debug.Log($"{DailyRewardKeyInternal}: Not time to reset yet.");
                }
            }
            else
            {
                // ✅ Just start the timer if none exists
                m_Timer.StartTimer();
                Debug.Log($"{DailyRewardKeyInternal}: First reset initialized.");
            }

            DailyCheckState();
        }

        private void DailyCheckState()
        {
            for (int i = 0; i < m_Rewards.Length; i++)
            {
                m_Rewards[i].CheckState();

                int dayCountPassed = m_Timer.DayCountPassedSinceReset();
                bool isDayToClaim = i == dayCountPassed;
                bool hasPassedDay = i < dayCountPassed;

                if (isDayToClaim)
                {
                    if (m_Rewards[i].State == DailyRewardState.OFF)
                    {
                        m_Rewards[i].On();
                        Debug.Log($"{DailyRewardKeyInternal}: Reward for day {i + 1} is now ON.");
                    }
                }
                else
                {
                    Debug.Log($"{DailyRewardKeyInternal}: Reward for day {i + 1} is not yet available.");
                }

                if (hasPassedDay && m_Rewards[i].State != DailyRewardState.CLAIMED)
                {
                    m_Rewards[i].Pass();
                    Debug.Log($"{DailyRewardKeyInternal}: Reward for day {i + 1} has been PASSED.");
                }
            }
        }

        private void OnTimerResetInternal()
        {
            foreach (var reward in m_Rewards)
            {
                reward.Off();
            }
            Debug.Log($"{DailyRewardKeyInternal}: Timer reset, daily rewards are reset.");
            m_Timer.StartTimer();
        }
        public void ForceReset()
        {
            //OnTimerResetInternal();
            Debug.Log($"{DailyRewardKeyInternal}: Force reset executed.");
        }
    }

    [System.Serializable]
    public class DailyRewardData
    {
        [SerializeField]
        private DailyRewardState m_State = DailyRewardState.OFF;
        [SerializeField]
        private LootDefinition m_Reward;

        public DailyRewardState State { get => m_State; set => m_State = value; }
        public LootDefinition Reward => m_Reward;

        public DailyRewardData(DailyRewardState state, LootDefinition reward)
        {
            m_State = state;
            m_Reward = reward;
        }

        private string Key => $"{DailyReward.DailyRewardKey}_{m_Reward.Id}";

        public void CheckState()
        {
            bool hasState = UnityService.Instance.HasData(Key);
            if (hasState)
            {
                m_State = (DailyRewardState)UnityService.Instance.GetData<int>(Key);
            }
        }

        public void Claim()
        {
            m_Reward.DirectTakeLoots();
            m_State = DailyRewardState.CLAIMED;
            UnityService.Instance.SaveData(Key, (int)m_State);
        }

        public void Pass()
        {
            m_State = DailyRewardState.PASSED;
            UnityService.Instance.SaveData(Key, (int)m_State);
        }

        public void On()
        {
            m_State = DailyRewardState.ON;
            UnityService.Instance.SaveData(Key, (int)m_State);
        }

        public void Off()
        {
            m_State = DailyRewardState.OFF;
            UnityService.Instance.SaveData(Key, (int)m_State);
        }
    }

    public enum DailyRewardState
    {
        OFF = 1,
        ON = 2,
        CLAIMED = 3,
        PASSED = 4
    }

}
