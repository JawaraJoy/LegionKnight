using Rush;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace LegionKnight
{
    public enum CardDetailMode
    {
        Normal,   // Add/Remove button aktif
        ReadOnly  // Hanya lihat detail, button tersembunyi
    }

    public partial class CardDetailView : UIView    
    {
        [SerializeField] private TextMeshProUGUI m_CardNameText;
        [SerializeField] private TextMeshProUGUI m_CardDescriptionText;
        [SerializeField] private Image m_CardIcon;
        [SerializeField] private Button m_AddOrRemoveToDeckButton;
        
        [SerializeField] private TextMeshProUGUI m_AddOrRemoveText;

        [SerializeField] private Button m_ReadOnlyClose;
        [SerializeField] private TMP_Text m_RarityText;

        [SerializeField] private CardUpgradeView m_UpgradeView;
        [SerializeField] private PlatformAlbum m_Album;
        [SerializeField] private Button m_UpgradeButton;
        private CardDeck m_CardDeck;
        private CardUnit m_SelectedCard;
        private CardDetailMode m_Mode = CardDetailMode.Normal;

        private void Awake()
        {
            m_CardDeck = Player.Instance.PlayerCardDeck;
            m_CardDeck.OnSelectedCard.AddListener(OnCardSelected);
            m_CardDeck.OnUsedCardsChanged.AddListener(OnUsedCardsChanged);
            m_CardDeck.OnCardAdded.AddListener(OnCardAmountChanged);
            m_AddOrRemoveToDeckButton.onClick.AddListener(OnAddOrRemoveClicked);

            m_UpgradeButton.onClick.AddListener(OpenUpgrade);
        }

        private void OnUsedCardsChanged(System.Collections.Generic.List<CardUnit> cards) => RefreshButtonState();
        private void OnCardAmountChanged(CardUnit card)
        {
            if (card == m_SelectedCard) RefreshButtonState();
        }
        private void OnDestroy()
        {
            if (m_CardDeck != null)
            {
                m_CardDeck.OnSelectedCard.RemoveListener(OnCardSelected);
                m_CardDeck.OnUsedCardsChanged.RemoveListener(OnUsedCardsChanged);
                m_CardDeck.OnCardAdded.RemoveListener(OnCardAmountChanged);
            }
            if (m_AddOrRemoveToDeckButton != null) m_AddOrRemoveToDeckButton.onClick.RemoveListener(OnAddOrRemoveClicked);
            if (m_UpgradeButton != null) m_UpgradeButton.onClick.RemoveListener(OpenUpgrade);
        }
        private void OpenUpgrade()
        {
            if (m_SelectedCard?.IsOwned == true) m_UpgradeView.Display(m_SelectedCard, m_CardDeck);
        }

        // ── Show normal mode (dari CardSelectView / CardSlotView) ─────────────
        private void OnCardSelected(CardUnit cardUnit)
        {
            ShowCard(cardUnit, CardDetailMode.Normal);
        }

        // ── Show read-only mode (dari DefaultCardDeckView) ────────────────────
        public void ShowReadOnly(CardUnit cardUnit)
        {
            ShowCard(cardUnit, CardDetailMode.ReadOnly);
        }

        // ── Core show ─────────────────────────────────────────────────────────
        private void ShowCard(CardUnit cardUnit, CardDetailMode mode)
        {
            if (cardUnit == null || cardUnit.CardConfig == null) return;
            m_SelectedCard = cardUnit;
            m_Mode = mode;

            var collectible = cardUnit.CardConfig.CollectibleField;
            m_CardIcon.sprite = collectible.Icon;
            bool showDeckAction = mode == CardDetailMode.Normal && !m_Album.Browsing;
            if (m_ReadOnlyClose != null) m_ReadOnlyClose.gameObject.SetActive(!showDeckAction);
            if (m_RarityText != null)
            {
                m_RarityText.text = collectible.RarityConfig.BaseInfo.Name;
                m_RarityText.color = collectible.RarityConfig.Color;
            }
            m_CardNameText.text = $"{cardUnit.CardConfig.BaseInfo.Name}  +{cardUnit.Upgrade}";
            m_CardDescriptionText.text = cardUnit.CardConfig.HasHeroCardText ? cardUnit.CardConfig.SimpleDescription : cardUnit.CardConfig.Description;

            m_AddOrRemoveToDeckButton.gameObject.SetActive(showDeckAction);
            m_UpgradeButton.gameObject.SetActive(!cardUnit.CardConfig.IsBattlePoolOnly);

            RefreshButtonState();

            ShowInternal();
        }

        // ── Refresh Add/Remove button state ───────────────────────────────────
        private void RefreshButtonState()
        {
            if (m_SelectedCard == null) return;
            m_CardNameText.text = $"{m_SelectedCard.CardConfig.BaseInfo.Name}  +{m_SelectedCard.Upgrade}";
            m_UpgradeButton.interactable = m_SelectedCard.IsOwned;
            if (m_Mode == CardDetailMode.ReadOnly) return;

            bool isAdded = m_SelectedCard.IsAdded;
            bool isFull = Player.Instance.PlayerCardDeck.GetUsedCards().Count >= Player.Instance.PlayerCardDeck.GetMaxUsedCardCount();
            bool isOwned = m_SelectedCard.IsOwned;

            bool isAvailable = m_SelectedCard.CardConfig.IsAvailableForHero(Player.Instance.HeroesCollection.UsedHero);
            m_AddOrRemoveText.text = isAdded ? "Remove" : "Add";
            m_AddOrRemoveToDeckButton.interactable = isAdded || (isOwned && isAvailable && !isFull);

        }

        // ── Add / Remove ──────────────────────────────────────────────────────
        private void OnAddOrRemoveClicked()
        {
            if (m_SelectedCard == null) return;
            if (m_Mode == CardDetailMode.ReadOnly) return;

            if (m_SelectedCard.IsAdded)
                Player.Instance.PlayerCardDeck.RemoveUsedCardConfig(m_SelectedCard.CardConfig);
            else
                Player.Instance.PlayerCardDeck.SetUsedCardConfig();
        }
    }
}
