using UnityEngine;
using LegionKnight;
using MoreMountains.Tools;
using TMPro;
using UnityEngine.UI;

namespace Rush
{
    public class RLCardView : UIView
    {
        [SerializeField, MMReadOnly]
        private CardConfig m_CardConfig;

        [SerializeField]
        private TextMeshProUGUI m_CardNameText;
        [SerializeField]
        private TextMeshProUGUI m_CardDescriptionText;
        [SerializeField]
        private Image m_CardIcon;
        [SerializeField]
        private Image m_CardRarityColor;

        [SerializeField]
        private Button m_SelectButton;
        public Button SelectButton => m_SelectButton;

        private void Start()
        {
            m_SelectButton.onClick.RemoveListener(OnSelectButtonClicked);
            m_SelectButton.onClick.AddListener(OnSelectButtonClicked);
        }
        public void ShowCard(CardConfig cardConfig)
        {
            m_CardConfig = cardConfig;

            m_CardNameText.text = m_CardConfig.BaseInfo.Name;
            m_CardDescriptionText.text = m_CardConfig.BaseInfo.Description;
            m_CardIcon.sprite = m_CardConfig.CollectibleField.Icon;
            m_CardRarityColor.color = m_CardConfig.CollectibleField.RarityConfig.Color;
            ShowInternal();
        }
        private void OnSelectButtonClicked()
        {
            m_CardConfig.Collect();
        }
    }
}
