using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace LegionKnight
{
    public class DailyRewardView : UIView
    {
        [SerializeField]
        private Image m_IconImage;
        [SerializeField]
        private LootDefinition m_Reward;
        [SerializeField]
        private Button m_ClaimButton;
        [SerializeField]
        private UnityEvent<LootDefinition> m_OnClaimed;
        [SerializeField]
        private GameObject m_OffContent;
        [SerializeField]
        private GameObject m_OnContent;
        [SerializeField]
        private GameObject m_ClaimedContent;
        [SerializeField]
        private GameObject m_PassedContent;

        private void OnEnable()
        {
            
            Init();
        }

        private void Init()
        {
            
            ShowInternal();
            Sprite icon = m_Reward.MainIconReward;
            DailyRewardState state = GameManager.Instance.DailyRewardManager.GetDailyRewardData(m_Reward).State;
            m_IconImage.sprite = icon;

            bool hasOff = state == DailyRewardState.OFF;
            bool hasOn = state == DailyRewardState.ON;
            bool hasClaimed = state == DailyRewardState.CLAIMED;
            bool hasPassed = state == DailyRewardState.PASSED;

            m_OffContent.SetActive(hasOff);
            m_OnContent.SetActive(hasOn);
            m_ClaimedContent.SetActive(hasClaimed);
            m_PassedContent.SetActive(hasPassed);
            m_ClaimButton.onClick.RemoveAllListeners();
            m_ClaimButton.onClick.AddListener(Claim);
        }

        private void Claim()
        {
            m_OnClaimed?.Invoke(m_Reward);
            MissionPanel panel = GameManager.Instance.GetPanel<MissionPanel>();
            DailyRewardMonitor monitor = panel.GetBinding<DailyRewardMonitor>();
            monitor.ShowClaimedDailyReward(m_Reward);

            DailyRewardData data = GameManager.Instance.DailyRewardManager.GetDailyRewardData(m_Reward);
            data.Claim();
            //GameManager.Instance.DailyRewardManager.AddDayLoginCount(1);
            Init();
        }
    }
}
