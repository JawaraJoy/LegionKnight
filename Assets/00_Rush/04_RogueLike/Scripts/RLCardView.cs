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
        [SerializeField] private LayoutElement m_RowLayout;
        [SerializeField] private TMP_Text m_Progression;
        [SerializeField] private TMP_Text m_Rarity;
        [SerializeField] private Image m_RarityBadge;
        public Button SelectButton => m_SelectButton;
        public CardConfig CardConfig => m_CardConfig;
        private RogueLikeCardPanel m_SelectionPanel;
        public void BindSelectionPanel(RogueLikeCardPanel panel) => m_SelectionPanel = panel;
        public void ClearCard()
        {
            m_CardConfig = null;
            if (m_RowLayout != null) m_RowLayout.ignoreLayout = true;
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
            var rarity = cardConfig.CollectibleField.RarityConfig;
            Color edge = rarity != null ? rarity.Color : Color.white;
            if (m_RowLayout != null)
            {
                m_RowLayout.ignoreLayout = false;
                m_CardDescriptionText.text = GameplayDescription.BattleCardDetails(cardConfig, controller, platformHandler, out string progression);
                m_CardDescriptionText.gameObject.SetActive(m_CardDescriptionText.text.Length > 0);
                m_Progression.text = progression;
                m_Progression.gameObject.SetActive(progression.Length > 0);
                m_Rarity.text = rarity == null ? string.Empty : rarity.BaseInfo.Name.ToUpperInvariant();
                m_RarityBadge.color = Color.Lerp(new Color(.14f, .14f, .23f), edge, .28f);
                m_RarityBadge.gameObject.SetActive(rarity != null);
            }
            else
            {
                m_CardDescriptionText.text = GameplayDescription.BattleCard(cardConfig, controller, platformHandler: platformHandler);
                Rect area = m_CardDescriptionText.rectTransform.rect;
                if (area.width > 0 && area.height > 0 &&
                    m_CardDescriptionText.GetPreferredValues(m_CardDescriptionText.text, area.width, float.PositiveInfinity).y > area.height)
                    m_CardDescriptionText.text = GameplayDescription.BattleCard(cardConfig, controller, 1, platformHandler);
            }
            m_CardNameText.text = cardConfig.BaseInfo.Name;
            m_CardIcon.sprite = cardConfig.CollectibleField.Icon;
            m_CardRarityColorOutline.color = edge;
            m_CardRarityColorBackground.color = edge;
            ShowInternal();
        }
        private void OnSelectButtonClicked()
        {
            if (m_SelectionPanel != null) m_SelectionPanel.SelectCard(this);
        }
    }
}
