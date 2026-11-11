using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Rush
{
    public class QuestTaskItemUI : MonoBehaviour
    {

        [System.Serializable]
        private sealed class ArtField
        {
            [SerializeField] private Image m_Objective;
            [SerializeField] private Sprite m_DefaultObjective;
            [SerializeField] private Image m_RewardFrame;
            [SerializeField] private Image m_ProgressFill;
            [SerializeField] private Color m_InProgressColor;
            [SerializeField] private Color m_CompleteColor;

            public void SetTask(QuestTaskConfig task)
            {
                if (m_Objective != null)
                    m_Objective.sprite = task.ObjectiveIcon != null ? task.ObjectiveIcon : m_DefaultObjective;

                var rarity = task.RewardCollectible?.CollectibleField?.RarityConfig;
                if (m_RewardFrame != null)
                    m_RewardFrame.color = rarity != null ? rarity.Color : Color.white;
            }

            public void SetCompletion(bool complete)
            {
                if (m_ProgressFill != null)
                    m_ProgressFill.color = complete ? m_CompleteColor : m_InProgressColor;
            }
        }

        [SerializeField] private ArtField m_Art = new();
        private const string c_QuantityPrefix = "\u00D7";

        [SerializeField] private TextMeshProUGUI m_TaskNameText;
        [SerializeField] private TextMeshProUGUI m_TaskDescText;
        [SerializeField] private TextMeshProUGUI m_ProgressText;   // e.g. "3/5"
        [SerializeField] private Slider m_ProgressSlider;

        [SerializeField] private Image m_RewardIcon;
        [SerializeField] private TextMeshProUGUI m_RewardAmountText;

        [SerializeField] private Button m_ClaimButton;
        [SerializeField] private TextMeshProUGUI m_ClaimButtonText;

        [SerializeField] private GameObject m_StateInProgress;
        [SerializeField] private GameObject m_StateComplete;  // complete, not yet claimed
        [SerializeField] private GameObject m_StateClaimed;   // reward claimed

        private QuestTaskConfig m_Task;
        public QuestTaskConfig Task => m_Task;

        // ── Setup ─────────────────────────────────────────────────────────────

        public void Setup(QuestTaskState state)
        {
            m_Task = state.Config;

            if (m_TaskNameText != null) m_TaskNameText.text = state.Config.BaseInfo.Name;
            if (m_TaskDescText != null) m_TaskDescText.text = state.Config.BaseInfo.Description;

            m_Art.SetTask(state.Config);
            RefreshRewardInternal(state.Config);
            RefreshProgressInternal(state);
            RefreshStateInternal(state);

            if (m_ClaimButton != null)
            {
                m_ClaimButton.onClick.RemoveListener(OnClaimClickedInternal);
                m_ClaimButton.onClick.AddListener(OnClaimClickedInternal);
            }
        }

        public void Refresh(QuestTaskState state)
        {
            RefreshProgressInternal(state);
            RefreshStateInternal(state);
        }

        // ── Refresh ───────────────────────────────────────────────────────────

        private void RefreshRewardInternal(QuestTaskConfig config)
        {
            if (m_RewardIcon != null)
            {
                m_RewardIcon.sprite = config.RewardCollectible?.CollectibleField?.Icon;
                m_RewardIcon.enabled = m_RewardIcon.sprite != null;
            }

            if (m_RewardAmountText != null)
            {
                bool show = config.RewardAmount >= 2;
                m_RewardAmountText.gameObject.SetActive(show);
                if (show) m_RewardAmountText.text = c_QuantityPrefix + config.RewardAmount;
            }
        }

        private void RefreshProgressInternal(QuestTaskState state)
        {
            m_Art.SetCompletion(state.IsComplete);
            if (m_ProgressText != null)
                m_ProgressText.text = $"{state.CurrentCount}/{state.TargetCount}";

            if (m_ProgressSlider != null)
            {
                m_ProgressSlider.minValue = 0;
                m_ProgressSlider.maxValue = state.TargetCount;
                m_ProgressSlider.value = state.CurrentCount;
            }
        }

        private void RefreshStateInternal(QuestTaskState state)
        {
            if (m_StateInProgress != null)
                m_StateInProgress.SetActive(!state.IsComplete && !state.IsClaimed);
            if (m_StateComplete != null)
                m_StateComplete.SetActive(state.CanClaim);
            if (m_StateClaimed != null)
                m_StateClaimed.SetActive(state.IsClaimed);

            if (m_ClaimButton != null)
                m_ClaimButton.interactable = state.CanClaim;

            if (m_ClaimButtonText != null)
                m_ClaimButtonText.text = state.IsClaimed ? "Claimed" : "Claim";
        }

        // ── Callbacks ─────────────────────────────────────────────────────────

        private void OnClaimClickedInternal() =>
            RushPlayer.Instance.QuestManager.Claim(m_Task);
    

        private void OnDestroy()
        {
            if (m_ClaimButton != null)
            {
                m_ClaimButton.onClick.RemoveListener(OnClaimClickedInternal);
            }
        }
    }
}