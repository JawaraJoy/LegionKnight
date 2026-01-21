using UnityEngine;
using UnityEngine.Events;

namespace LegionKnight
{
    [System.Serializable]
    public partial class SpendReward
    {
        [SerializeField]
        private Object m_Reward;
        [SerializeField]
        private int m_RewardAmount;
        [SerializeField]
        private bool m_IsRewardAvaible = false;
        public Object Reward => m_Reward;
        [SerializeField]
        private int m_CurrentShopingPoint;
        [SerializeField]
        private int m_MaxShopingPoint = 1000;
        public int RewardAmount => m_RewardAmount;
        public bool IsRewardAvaible => m_IsRewardAvaible;
        public int CurrentShopingPoint => m_CurrentShopingPoint;
        public int MaxShopingPoint => m_MaxShopingPoint;

        public Object GetReward()
        {
            return m_Reward;
        }
        [SerializeField]
        private UnityEvent<int> m_OnCurrentShopingPointChanged = new();
        [SerializeField]
        private UnityEvent<SpendReward> m_OnClaimSpendReward = new();
        [SerializeField]
        private UnityEvent<bool> m_OnShopingRewardAvaible = new();
        public void AddShopingPoint(int amount)
        {
            m_CurrentShopingPoint += amount;
            OnCurrentShopingPointChangedInvoke(m_CurrentShopingPoint);
        }
        public void ClaimShopingReward()
        {
            OnClaimSpendRewardInvoke(this);
        }
        private void OnCurrentShopingPointChangedInvoke(int amount)
        {
            m_OnCurrentShopingPointChanged?.Invoke(amount);
            m_IsRewardAvaible = m_CurrentShopingPoint >= m_MaxShopingPoint;

            OnShopingRewardAvaibleInvoke(m_IsRewardAvaible);
        }
        private void OnClaimSpendRewardInvoke(SpendReward reward)
        {
            m_OnClaimSpendReward?.Invoke(reward);
            int remaining = m_CurrentShopingPoint - m_MaxShopingPoint;
            m_CurrentShopingPoint = remaining;
        }
        private void OnShopingRewardAvaibleInvoke(bool available)
        {
            m_OnShopingRewardAvaible?.Invoke(available);
        }
    }
}
