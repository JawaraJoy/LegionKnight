using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace LegionKnight
{
    public abstract class TaskThresholdView : UIView
    {
        [SerializeField]
        private Image m_GrantedImage;
        [SerializeField]
        private Button m_ClaimButton;
        [SerializeField]
        private TextMeshProUGUI m_ThresholdAmountText;
        [SerializeField]
        private Sprite m_NonActiveSprite;
        [SerializeField]
        private Sprite m_ReadyToClaimSprite;
        [SerializeField]
        private Sprite m_ClaimedSprite;

        protected abstract MissionController GetControllerInternal();

        public void Init(TaskThreshold threshold)
        {
            GrantedState state = threshold.GrantedState;
            switch (state)
            {
                case GrantedState.NotReady:
                    m_GrantedImage.sprite = m_NonActiveSprite;
                    
                    break;
                case GrantedState.ReadyToClaim:
                    m_GrantedImage.sprite = m_ReadyToClaimSprite;
                    break;
                case GrantedState.Claimed:
                    m_GrantedImage.sprite = m_ClaimedSprite;
                    break;
            }
            m_ClaimButton.interactable = state == GrantedState.ReadyToClaim;
            m_ClaimButton.onClick.RemoveAllListeners();
            m_ClaimButton.onClick.AddListener(() => Claim(threshold));
            m_ThresholdAmountText.text = threshold.Threshold.ToString();
        }


        private void Claim(TaskThreshold threshold)
        {
            threshold.Claim();
            threshold.SetGranted(GrantedState.Claimed);
            m_GrantedImage.sprite = m_ClaimedSprite;
        }
    }
}
