using LegionKnight;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Rush
{
    public class ReviewPanel : PanelView
    {
        [SerializeField]
        private TMP_InputField m_ReviewInput;

        [SerializeField]
        private Button m_SubmitButton;
        [SerializeField]
        private Image m_IconReward;
        [SerializeField]
        private TextMeshProUGUI m_RewardAmountText;

        [SerializeField]
        private ReviewStarButton[] m_Stars;

        public ReviewStarButton[] Stars => m_Stars;

        private void Start()
        {
            m_SubmitButton.onClick.AddListener(
                SubmitReview);
        }

        public void OpenReview()
        {
            foreach (var star in m_Stars)
            {
                star.SetActive(false);
            }

            m_ReviewInput.text = string.Empty;

            m_SubmitButton.interactable = false;

            ShowInternal();

            Sprite icon = RushGameManager.Instance.ReviewManager.Reward.ItemConfig.CollectibleField.Icon;
            int amount = RushGameManager.Instance.ReviewManager.Reward.Amount;

            m_IconReward.sprite = icon;
            m_RewardAmountText.text = amount.ToString();
        }

        public void SetSubmitButton(bool active)
        {
            SetSubmitButtonInternal(active);
        }
        private void SetSubmitButtonInternal(bool active)
        {
            m_SubmitButton.interactable = active;
        }

        private void SubmitReview()
        {
            RushGameManager.Instance.ReviewManager.SubmitReview(m_ReviewInput.text);
            SetSubmitButtonInternal(false);
        }
    }
}