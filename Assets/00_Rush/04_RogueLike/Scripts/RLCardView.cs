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
        private Image m_CardRarityColorOutline;
        [SerializeField]
        private Image m_CardRarityColorBackground;

        [SerializeField]
        private Button m_SelectButton;
        public Button SelectButton => m_SelectButton;
        public CardConfig CardConfig => m_CardConfig;
        private RogueLikeCardPanel m_SelectionPanel;
        public void BindSelectionPanel(RogueLikeCardPanel panel) => m_SelectionPanel = panel;
        public void ClearCard()
        {
            m_CardConfig = null;
            HideInternal();
        }
        private void OnDestroy()
        {
            if (m_SelectButton != null) m_SelectButton.onClick.RemoveListener(OnSelectButtonClicked);
        }

        private void Start()
        {
            m_SelectButton.onClick.RemoveListener(OnSelectButtonClicked);
            m_SelectButton.onClick.AddListener(OnSelectButtonClicked);
        }
        public void ShowCard(CardConfig cardConfig)
        {
            if (cardConfig == null) { ClearCard(); return; }
            m_CardConfig = cardConfig;
            RushPlayer.Instance.Unit.HasBind(out SkillController controller);
            PlatformHandler platformHandler = RushGameManager.Instance?.StageManager?.PlatformHandler;
            string description = GameplayDescription.BattleCard(m_CardConfig, controller, platformHandler: platformHandler);
            Rect textArea = m_CardDescriptionText.rectTransform.rect;
            if (textArea.width > 0 && textArea.height > 0 &&
                m_CardDescriptionText.GetPreferredValues(description, textArea.width, float.PositiveInfinity).y > textArea.height)
                description = GameplayDescription.BattleCard(m_CardConfig, controller, 1, platformHandler);
            m_CardDescriptionText.text = description;
            m_CardNameText.text = m_CardConfig.BaseInfo.Name;
            
            m_CardIcon.sprite = m_CardConfig.CollectibleField.Icon;
            Color rarityColor = m_CardConfig.CollectibleField.RarityConfig.Color;
            m_CardRarityColorOutline.color = rarityColor;
            m_CardRarityColorBackground.color = rarityColor;
            ShowInternal();
        }
        private void OnSelectButtonClicked()
        {
            if (m_SelectionPanel != null) m_SelectionPanel.SelectCard(this);
        }
    }
}
