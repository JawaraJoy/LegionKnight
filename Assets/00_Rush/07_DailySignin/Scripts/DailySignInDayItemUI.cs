using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Rush
{
    public class DailySignInDayItemUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI m_DayLabel;
        [SerializeField] private Image m_RewardIcon;
        [SerializeField] private Image m_RewardFrame;
        [SerializeField] private TextMeshProUGUI m_AmountText;
        [SerializeField] private TextMeshProUGUI m_RewardName;

        [SerializeField] private Button m_ClaimButton;
        [SerializeField] private Button m_MissedClaimButton;
        [SerializeField] private GameObject m_StateAvailable;
        [SerializeField] private GameObject m_StateClaimed;
        [SerializeField] private GameObject m_StateLocked;
        [SerializeField] private GameObject m_StateMissed;
        [SerializeField] private GameObject m_StateComplete;

        [SerializeField] private Image m_DayPaper;
        [SerializeField] private Color m_AvailableColor = Color.white;
        [SerializeField] private Color m_ClaimedColor = Color.white;
        [SerializeField] private Color m_MissedColor = Color.white;
        [SerializeField] private Color m_LockedColor = Color.white;

        private Action m_OnClaimed;
        private Action m_OnMissedClaimed;
        private bool m_HasReward;

        private void Awake()
        {
            if (m_ClaimButton != null) m_ClaimButton.onClick.AddListener(OnClaimClickedInternal);
            if (m_MissedClaimButton != null) m_MissedClaimButton.onClick.AddListener(OnMissedClaimClickedInternal);
        }

        public void Setup(int dayIndex, DailySignInRewardEntry entry, DayDisplayState state,
            Action onClaimed, Action onMissedClaimed)
        {
            m_OnClaimed = onClaimed;
            m_OnMissedClaimed = onMissedClaimed;
            CollectibleConfig reward = entry?.Collectible;
            m_HasReward = reward != null && entry.Amount > 0;
            if (m_DayLabel != null) m_DayLabel.text = $"Day {dayIndex + 1}";
            if (m_RewardIcon != null)
            {
                Sprite sprite = m_RewardName != null ? entry?.DisplaySplash : entry?.DisplayIcon;
                m_RewardIcon.sprite = sprite;
                m_RewardIcon.enabled = sprite != null;
            }
            if (m_RewardFrame != null)
            {
                RarityConfig rarity = reward?.CollectibleField?.RarityConfig;
                m_RewardFrame.color = rarity != null ? rarity.Color : Color.white;
            }
            if (m_RewardName != null) m_RewardName.text = reward != null ? reward.BaseInfo.Name : string.Empty;
            if (m_AmountText != null)
            {
                m_AmountText.gameObject.SetActive(m_HasReward);
                if (m_HasReward) m_AmountText.text = $"\u00d7{entry.Amount}";
            }
            RefreshState(state);
        }

        public void RefreshState(DayDisplayState state)
        {
            if (m_StateAvailable != null) m_StateAvailable.SetActive(state == DayDisplayState.Available);
            if (m_StateMissed != null) m_StateMissed.SetActive(state == DayDisplayState.Missed);
            if (m_StateClaimed != null) m_StateClaimed.SetActive(state == DayDisplayState.Claimed);
            if (m_StateLocked != null) m_StateLocked.SetActive(state == DayDisplayState.Locked);
            if (m_StateComplete != null) m_StateComplete.SetActive(state == DayDisplayState.Complete);
            if (m_ClaimButton != null) m_ClaimButton.interactable = m_HasReward && state == DayDisplayState.Available;
            if (m_MissedClaimButton != null) m_MissedClaimButton.interactable = m_HasReward && state == DayDisplayState.Missed;
            if (m_DayPaper != null)
            {
                m_DayPaper.color = state switch
                {
                    DayDisplayState.Available => m_AvailableColor,
                    DayDisplayState.Claimed => m_ClaimedColor,
                    DayDisplayState.Missed => m_MissedColor,
                    _ => m_LockedColor
                };
            }
        }

        private void OnClaimClickedInternal() => m_OnClaimed?.Invoke();
        private void OnMissedClaimClickedInternal() => m_OnMissedClaimed?.Invoke();

        private void OnDestroy()
        {
            if (m_ClaimButton != null) m_ClaimButton.onClick.RemoveListener(OnClaimClickedInternal);
            if (m_MissedClaimButton != null) m_MissedClaimButton.onClick.RemoveListener(OnMissedClaimClickedInternal);
        }
    }
}
