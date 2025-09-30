using MoreMountains.Tools;
using UnityEngine;

namespace LegionKnight
{
    public class DailyReward : MonoBehaviour
    {
        [SerializeField]
        private string m_BehaviourName = "Daily Sign In";
        [SerializeField]
        private TimerDefinition m_Timer;
        [SerializeField]
        private DailyRewardData[] m_Rewards;
        public string BehaviourName => m_BehaviourName;
        public TimerDefinition Timer => m_Timer;
        private const string DailyRewardKeyInternal = "dailyreward";
        private string ResetKey => DailyRewardKey + "reset";
        public static string DailyRewardKey => DailyRewardKeyInternal;

        [SerializeField, MMReadOnly]
        private int m_RewardLenght; 
        private DailyRewardData GetDailyRewardDataInternal(LootDefinition loot)
        {
            foreach (var reward in m_Rewards)
            {
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
        private void RefreshInternal()
        {
            if (m_Timer == null)
            {
                Debug.LogWarning($"{DailyRewardKeyInternal}: Timer is not set.");
                return;
            }
            m_RewardLenght = m_Rewards.Length;
            bool hasResetTime = UnityService.Instance.HasData(ResetKey);
            if (hasResetTime)
            {
                bool isReset = m_Timer.IsTimeToReset();
                if (isReset)
                {
                    OnTimerReset();
                    Debug.Log($"{DailyRewardKeyInternal}: Timer reset, daily rewards are reset.");
                }
                else
                {
                    Debug.Log($"{DailyRewardKeyInternal}: Not time to reset yet.");
                }
            }
            else
            {
                m_Timer.StartTimer();
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
        private void OnTimerReset()
        {
            foreach (var reward in m_Rewards)
            {
                reward.Off();
            }
            m_Timer.StartTimer();
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
