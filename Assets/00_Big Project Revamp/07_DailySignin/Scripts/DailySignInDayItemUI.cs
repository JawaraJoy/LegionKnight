using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Rush
{
    public class DailySignInDayItemUI : MonoBehaviour
    {
        [SerializeField] private GameObject m_StateMissed;
        [SerializeField] private Button m_MissedClaimButton;
        [Header("Info")]
        [SerializeField] private TextMeshProUGUI m_DayLabel;
        [SerializeField] private Image m_RewardIcon;
        [SerializeField] private TextMeshProUGUI m_AmountText;

        [Header("Claim Button — only interactable on Available state")]
        [SerializeField] private Button m_ClaimButton;

        [Header("States — activate the matching GameObject per state")]
        [SerializeField] private GameObject m_StateAvailable;
        [SerializeField] private GameObject m_StateClaimed;
        [SerializeField] private GameObject m_StateLocked;
        [SerializeField] private GameObject m_StateComplete;


        private Action m_OnMissedClaimed;
        private Action m_OnClaimed;

        public void Setup(int dayIndex, DailySignInRewardEntry entry, DayDisplayState state, Action onClaimed, Action onMissedClaimed)
        {
            m_OnClaimed = onClaimed;
            m_OnMissedClaimed = onMissedClaimed;

            if (m_DayLabel != null) m_DayLabel.text = $"Day {dayIndex + 1}";
            if (m_RewardIcon != null) m_RewardIcon.sprite = entry.DisplayIcon;

            if (m_AmountText != null)
            {
                bool show = entry.Amount >= 1;
                m_AmountText.gameObject.SetActive(show);
                if (show) m_AmountText.text = $"x{entry.Amount}";
            }

            RefreshStateInternal(state);
        }

        public void RefreshState(DayDisplayState state)
        {
            RefreshStateInternal(state);
        }

        private void RefreshStateInternal(DayDisplayState state)
        {
            if (m_StateAvailable != null) m_StateAvailable.SetActive(state == DayDisplayState.Available);
            if (m_StateClaimed != null) m_StateClaimed.SetActive(state == DayDisplayState.Claimed);
            if (m_StateLocked != null) m_StateLocked.SetActive(state == DayDisplayState.Locked);
            if (m_StateComplete != null) m_StateComplete.SetActive(state == DayDisplayState.Complete);
            if (m_StateMissed != null) m_StateMissed.SetActive(state == DayDisplayState.Missed);

            if (m_ClaimButton != null)
            {
                m_ClaimButton.interactable = state == DayDisplayState.Available;

                m_ClaimButton.onClick.RemoveAllListeners();

                if (state == DayDisplayState.Available)
                    m_ClaimButton.onClick.AddListener(OnClaimClickedInternal);
            }

            if (m_MissedClaimButton != null)
            {
                m_MissedClaimButton.interactable = state == DayDisplayState.Missed;

                m_MissedClaimButton.onClick.RemoveAllListeners();

                if (state == DayDisplayState.Missed)
                    m_MissedClaimButton.onClick.AddListener(OnMissedClaimClickedInternal);
            }
        }

        private void OnClaimClickedInternal() => m_OnClaimed?.Invoke();
        private void OnMissedClaimClickedInternal()
        {
            m_OnMissedClaimed?.Invoke();
        }
    }
}