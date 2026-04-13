using Rush;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace LegionKnight
{
    public partial class CardDetailView : UIView
    {
        [SerializeField] private TextMeshProUGUI m_CardNameText;
        [SerializeField] private TextMeshProUGUI m_CardDescriptionText;
        [SerializeField] private Image m_CardBigIcon;
        [SerializeField] private Button m_AddOrRemoveToDeckButton;
        [SerializeField] private TextMeshProUGUI m_AddOrRemoveText;

        // ✅ Tambah: feedback slot count "2 / 5"
        [SerializeField] private TextMeshProUGUI m_SlotCountText;

        private CardUnit m_SelectedCard;

        private void Awake()
        {
            Player.Instance.PlayerCardDeck.OnSelectedCard.AddListener(OnCardSelected);
            Player.Instance.PlayerCardDeck.OnUsedCardsChanged.AddListener(_ => RefreshButtonState());
            m_AddOrRemoveToDeckButton.onClick.AddListener(OnAddOrRemoveClicked);
        }

        // ── Dipanggil saat player tap card di list ────────────────────────────
        private void OnCardSelected(CardUnit cardUnit)
        {
            m_SelectedCard = cardUnit;

            m_CardBigIcon.sprite = cardUnit.CardConfig.CollectibleField.SplashImage;
            m_CardNameText.text = cardUnit.CardConfig.BaseInfo.Name;
            m_CardDescriptionText.text = cardUnit.CardConfig.BaseInfo.Description;

            RefreshButtonState();
            ShowInternal();
        }

        // ── Refresh teks Add/Remove dan interactable ──────────────────────────
        private void RefreshButtonState()
        {
            if (m_SelectedCard == null) return;

            bool isAdded = m_SelectedCard.IsAdded;
            bool isFull = Player.Instance.PlayerCardDeck.GetUsedCards().Count
                              >= Player.Instance.PlayerCardDeck.GetMaxUsedCardCount();
            bool isOwned = m_SelectedCard.IsOwned;

            // Teks button
            m_AddOrRemoveText.text = isAdded ? "Remove" : "Add";

            // Disable Add jika sudah full atau card tidak dimiliki
            m_AddOrRemoveToDeckButton.interactable = isAdded || (isOwned && !isFull);

            // Slot count feedback
            if (m_SlotCountText != null)
            {
                int used = Player.Instance.PlayerCardDeck.GetUsedCards().Count;
                int max = Player.Instance.PlayerCardDeck.GetMaxUsedCardCount();
                m_SlotCountText.text = $"{used} / {max}";
            }
        }

        // ── Add atau Remove ───────────────────────────────────────────────────
        private void OnAddOrRemoveClicked()
        {
            if (m_SelectedCard == null) return;

            if (m_SelectedCard.IsAdded)
                Player.Instance.PlayerCardDeck.RemoveUsedCardConfig(m_SelectedCard.CardConfig);
            else
                Player.Instance.PlayerCardDeck.SetUsedCardConfig();

            // RefreshButtonState sudah dipanggil via OnUsedCardsChanged event
        }
    }
}