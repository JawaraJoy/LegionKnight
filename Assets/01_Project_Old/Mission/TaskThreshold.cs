using MoreMountains.Tools;
using UnityEngine;
using UnityEngine.Events;
using Rush;

namespace LegionKnight
{
    [System.Serializable]
    public class TaskThreshold
    {
        [SerializeField]
        private string m_Id;
        [SerializeField]
        private int m_Threshold;
        [SerializeField]
        private LootField m_Rewards;
        [SerializeField]
        private UnityEvent<LootField[]> m_OnClaims;
        public int Threshold => m_Threshold;
        public LootField Rewards => m_Rewards;

        [SerializeField, MMReadOnly]
        private GrantedState m_GrantState;
        public GrantedState GrantedState => m_GrantState;

        private string Key => $"taskThreshold_";
        private string GrantedKey => $"{Key}{m_Id}";
        public void Initialize(MissionController controller)
        {
            bool hasData = UnityService.Instance.HasData(GrantedKey);
            if (hasData)
            {
                m_GrantState = (GrantedState)UnityService.Instance.GetData<int>(GrantedKey);
            }
            CheckThreshold(controller.CurrentTaskPower);
        }
        private void CheckThreshold(int currentScore)
        {
            if (m_GrantState == GrantedState.Claimed) return;
            if (currentScore >= m_Threshold)
            {
                SetGrantedInternal(GrantedState.ReadyToClaim);
                //Debug.Log($"{Key} SetGranted {m_GrantState}");
            }
            Debug.Log($"{Key} SetGranted {m_GrantState}");
        }
        public void Claim()
        {
            if (m_GrantState == GrantedState.ReadyToClaim)
            {
                
                LootField[] loots = new LootField[1] {m_Rewards};
                m_OnClaims?.Invoke(loots);
                m_Rewards.DirectTakeLoot();
                SetGrantedInternal(GrantedState.Claimed);
            }
        }
        public void SetGranted(GrantedState state)
        {
            SetGrantedInternal(state);
        }
        private void SetGrantedInternal(GrantedState stat)
        {
            m_GrantState = stat;
            UnityService.Instance.SaveData(GrantedKey, (int)m_GrantState);
        }
        public void Reset()
        {
            m_GrantState = GrantedState.NotReady;
        }
    }

    public enum GrantedState
    {
        NotReady,
        ReadyToClaim,
        Claimed
    }
}
