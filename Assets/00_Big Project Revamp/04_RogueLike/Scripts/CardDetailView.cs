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

        [SerializeField]
        private Button m_SellButton;

        private CardUnit m_SelectedCard;
        private CardDetailMode m_Mode = CardDetailMode.Normal;

        private void Awake()
        {
            Player.Instance.PlayerCardDeck.OnSelectedCard.AddListener(OnCardSelected);
            Player.Instance.PlayerCardDeck.OnUsedCardsChanged.AddListener(_ => RefreshButtonState());
            m_AddOrRemoveToDeckButton.onClick.AddListener(OnAddOrRemoveClicked);

            m_SellButton.onClick.AddListener(OpenSell);
        }

        private void SellCard(int remove)
        {
            Player.Instance.PlayerCardDeck.AddCardAmount(m_SelectedCard.CardConfig, -remove);
        }
        private void OpenSell()
        {
            SellPanel sellPanel = CanvasManager.Instance.GetPanel<SellPanel>();
            sellPanel.OpenSell(m_SelectedCard.CardConfig, m_SelectedCard.Amount, SellCard);
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
            m_SelectedCard = cardUnit;
            m_Mode = mode;

            m_CardIcon.sprite = cardUnit.CardConfig.CollectibleField.Icon;
            m_CardNameText.text = cardUnit.CardConfig.BaseInfo.Name;
            m_CardDescriptionText.text = cardUnit.CardConfig.BaseInfo.Description;

            // Sembunyikan button di read-only mode

            m_AddOrRemoveToDeckButton.gameObject.SetActive(mode == CardDetailMode.Normal);
            m_SellButton.gameObject.SetActive(mode == CardDetailMode.Normal);

            if (mode == CardDetailMode.Normal)
                RefreshButtonState();

            ShowInternal();
        }

        // ── Refresh Add/Remove button state ───────────────────────────────────
        private void RefreshButtonState()
        {
            if (m_SelectedCard == null) return;
            if (m_Mode == CardDetailMode.ReadOnly) return;

            bool isAdded = m_SelectedCard.IsAdded;
            bool isFull = Player.Instance.PlayerCardDeck.GetUsedCards().Count >= Player.Instance.PlayerCardDeck.GetMaxUsedCardCount();
            bool isOwned = m_SelectedCard.IsOwned;

            m_AddOrRemoveText.text = isAdded ? "Remove" : "Add";
            m_AddOrRemoveToDeckButton.interactable = isAdded || (isOwned && !isFull);
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