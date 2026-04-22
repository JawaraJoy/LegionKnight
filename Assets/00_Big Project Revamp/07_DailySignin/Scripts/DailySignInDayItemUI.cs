using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Rush
{
    public class DailySignInDayItemUI : MonoBehaviour
    {
        [Header("Info")]
        [SerializeField] private TextMeshProUGUI m_DayLabel;
        [SerializeField] private Image m_RewardIcon;
        [SerializeField] private TextMeshProUGUI m_AmountText;

        [Header("States")]
        [SerializeField] private GameObject m_StateAvailable; // can claim today
        [SerializeField] private GameObject m_StateClaimed;   // already claimed
        [SerializeField] private GameObject m_StateLocked;    // future day
        [SerializeField] private GameObject m_StateComplete;  // cycle complete, no loop

        public void Setup(int dayIndex, DailySignInRewardEntry entry,
            DayDisplayState state)
        {
            if (m_DayLabel != null) m_DayLabel.text = $"Day {dayIndex + 1}";
            if (m_RewardIcon != null) m_RewardIcon.sprite = entry.DisplayIcon;

            if (m_AmountText != null)
            {
                bool show = entry.Amount >= 2;
                m_AmountText.gameObject.SetActive(show);
                if (show) m_AmountText.text = $"x{entry.Amount}";
            }

            if (m_StateAvailable != null) m_StateAvailable.SetActive(state == DayDisplayState.Available);
            if (m_StateClaimed != null) m_StateClaimed.SetActive(state == DayDisplayState.Claimed);
            if (m_StateLocked != null) m_StateLocked.SetActive(state == DayDisplayState.Locked);
            if (m_StateComplete != null) m_StateComplete.SetActive(state == DayDisplayState.Complete);
        }
    }

    public enum DayDisplayState
    {
        Claimed,    // already claimed
        Available,  // today, not yet claimed
        Locked,     // future day
        Complete    // past the cycle end, no loop
    }
}