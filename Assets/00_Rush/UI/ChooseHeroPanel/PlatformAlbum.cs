using System.Collections.Generic;
using LegionKnight;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Rush
{
    public sealed class PlatformAlbum : MonoBehaviour
    {
        [SerializeField] private GameObject m_Collection, m_HeroDeck, m_Cover, m_Custom;
        [SerializeField] private TextMeshProUGUI m_Title, m_HeroName, m_DeckCount, m_SlotCount, m_Destination;
        [SerializeField] private Image m_Portrait;
        [SerializeField] private CardSelectTabView m_Tab;
        [SerializeField] private ScrollRect m_CollectionScroll;
        [SerializeField] private RarityConfig[] m_Rarities;
        [SerializeField] private Button[] m_FilterButtons;
        [SerializeField] private PlatformAlbumGrid m_CollectionGrid;
        [SerializeField] private GameObject m_CollectionClose;
        [SerializeField] private CanvasGroup m_CardListInput;
        [SerializeField] private RectTransform m_CompactViewport;
        [SerializeField, Range(0f, 1f)] private float m_ExpandedBottom = .015f;
        [SerializeField] private GameObject m_BrowseButton;
        private const int c_BrowseSlot = -2;
        private CardDeck m_Deck;
        private HeroesCollection m_Heroes;
        private CardUnit m_Selected;
        private int m_Filter, m_Slot = -1;
        private bool m_CardsChanged, m_WasEquipped;
        public CardUnit Selected => m_Selected;

        private void OnEnable()
        {
            m_Deck = Player.Instance.PlayerCardDeck;
            m_Heroes = Player.Instance.HeroesCollection;
            m_Heroes.OnCharacterUsed.AddListener(RefreshHeroInternal);
            m_Deck.OnSelectedCard.AddListener(OnCardSelectedInternal);
            m_Deck.OnCardAdded.AddListener(OnStockChangedInternal);
            m_Deck.OnUsedCardsChanged.AddListener(OnSlotsChangedInternal);
            m_CollectionGrid.CardsChanged += OnCardsChangedInternal;
            m_Tab.CardDetailView.OnShow.AddListener(RefreshCollectionLayoutInternal);
            m_Tab.CardDetailView.OnHide.AddListener(RefreshCollectionLayoutInternal);
            RefreshHeroInternal(m_Heroes.UsedHero);
        }

        private void OnDisable()
        {
            m_Tab.CardDetailView.OnShow.RemoveListener(RefreshCollectionLayoutInternal);
            m_Tab.CardDetailView.OnHide.RemoveListener(RefreshCollectionLayoutInternal);
            m_CollectionGrid.CardsChanged -= OnCardsChangedInternal;
            if (m_Deck != null)
            {
                m_Deck.OnSelectedCard.RemoveListener(OnCardSelectedInternal);
                m_Deck.OnUsedCardsChanged.RemoveListener(OnSlotsChangedInternal);
                m_Deck.OnCardAdded.RemoveListener(OnStockChangedInternal);
            }
            if (m_Heroes != null) m_Heroes.OnCharacterUsed.RemoveListener(RefreshHeroInternal);
            if (m_Selected != null && !m_Selected.IsAdded) m_Selected.AssignSlot(-1);
            m_Selected = null;
            m_Slot = -1;
        }

        private void OnCardsChangedInternal() => m_CardsChanged = true;
        private void LateUpdate()
        {
            if (m_Slot == -1 || !m_CardsChanged) return;
            m_CardsChanged = false;
            FilterInternal(m_Filter, false);
        }

        public void CloseCollection() => ShowDeckInternal();
        public void SetFilter(int index) => FilterInternal(index, true);

        public void OpenCollection()
        {
            m_Slot = c_BrowseSlot;
            m_Selected = null;
            m_Destination.text = "Choose a card to inspect or upgrade.";
            m_CardListInput.interactable = m_CardListInput.blocksRaycasts = true;
            SetPageInternal(false);
            m_Tab.CardDetailView.Hide();
            FilterInternal(0, true);
        }
        internal bool Browsing => m_Slot == c_BrowseSlot;
        public void OpenSlot(int index, CardUnit card)
        {
            if (index < 0 || index >= m_Deck.GetMaxUsedCardCount()) return;
            m_Selected = null;
            m_Slot = index;
            m_Destination.text = card == null ? "Choose a card for Custom Slot " + (index + 1) : "Manage Custom Slot " + (index + 1);
            m_CardListInput.interactable = m_CardListInput.blocksRaycasts = card == null;
            SetPageInternal(false);
            RefreshSelectionInternal();
            m_Tab.CardDetailView.Hide();
            FilterInternal(0, true);
            if (card != null) m_Deck.SelectStandbyCardConfig(card.CardConfig);
        }

        private void ShowDeckInternal()
        {
            if (m_Selected != null && !m_Selected.IsAdded) m_Selected.AssignSlot(-1);
            m_Selected = null;
            m_Slot = -1;
            m_Tab.CardDetailView.Hide();
            SetPageInternal(true);
        }

        private void RefreshCollectionLayoutInternal()
        {
            bool detailShown = m_Tab.CardDetailView.IsShown;
            if (m_BrowseButton != null)
                m_BrowseButton.SetActive(m_Slot == -1 && !detailShown);
            if (m_Slot == -1) return;

            var viewport = m_CollectionScroll.viewport;
            float bottom = detailShown ? m_CompactViewport.anchorMin.y : m_ExpandedBottom;
            if (Mathf.Approximately(viewport.anchorMin.y, bottom)) return;

            m_CollectionScroll.StopMovement();
            viewport.anchorMin = new Vector2(viewport.anchorMin.x, bottom);
            var content = m_CollectionScroll.content;
            LayoutRebuilder.ForceRebuildLayoutImmediate(content);
            float maxOffset = Mathf.Max(0f, content.rect.height - viewport.rect.height);
            var offset = content.anchoredPosition;
            content.anchoredPosition = new Vector2(offset.x, Mathf.Clamp(offset.y, 0f, maxOffset));
        }

        private void SetPageInternal(bool deck)
        {
            m_Collection.SetActive(!deck);
            RefreshCollectionLayoutInternal();
            m_HeroDeck.SetActive(deck);
            m_Cover.SetActive(deck);
            m_Custom.SetActive(deck);
            m_DeckCount.gameObject.SetActive(deck);
            m_CollectionClose.SetActive(!deck);
            m_Title.text = deck ? "Hero Default Cards" : "Card Collection";
            m_Title.rectTransform.anchorMin = new Vector2(.035f, deck ? .735f : .90f);
            m_Title.rectTransform.anchorMax = new Vector2(.83f, deck ? .81f : .99f);
        }

        private void OnCardSelectedInternal(CardUnit card)
        {
            if (m_Slot == -1 || card == null) return;
            if (m_Selected != null && m_Selected != card && !m_Selected.IsAdded) m_Selected.AssignSlot(-1);
            m_Selected = card;
            m_WasEquipped = card.IsAdded;
            if (!card.IsAdded && m_Slot >= 0) card.AssignSlot(m_Slot);
            RefreshSelectionInternal();
        }

        private void RefreshSelectionInternal()
        {
            foreach (Transform item in m_CollectionGrid.transform)
                if (item.TryGetComponent(out AlbumCardCaption caption)) caption.SetSelected(m_Selected?.CardConfig);
        }

        private void OnStockChangedInternal(CardUnit card)
        {
            m_CardsChanged = true;
            if (m_Selected == card && !card.IsOwned) ShowDeckInternal();
        }

        private void OnSlotsChangedInternal(List<CardUnit> cards)
        {
            m_SlotCount.text = cards.Count + " / " + m_Deck.GetMaxUsedCardCount();
            if (m_Selected != null && m_Selected.IsAdded != m_WasEquipped) ShowDeckInternal();
        }

        private void FilterInternal(int index, bool top)
        {
            if (index < 0 || index > m_Rarities.Length) return;
            m_Filter = index;
            var hero = m_Heroes.UsedHero;
            foreach (Transform child in m_CollectionGrid.transform)
            {
                if (!child.TryGetComponent(out CardSelectView view) || view.CardConfig == null) continue;
                var c = view.CardConfig;
                var unit = m_Deck.GetCardOwned(c);
                bool visible = unit?.IsOwned == true && !c.IsBattlePoolOnly &&
                    (Browsing || (c.IsAvailableForHero(hero) && !RushGameManager.Instance.RogueLikeManager.IsAutomaticCard(c, hero))) &&
                    (index == 0 || c.CollectibleField.RarityConfig == m_Rarities[index - 1]);
                if (visible)
                {
                    view.Init(unit);
                    if (view.TryGetComponent(out AlbumCardCaption caption)) caption.Display(unit);
                    view.Show();
                }
                child.gameObject.SetActive(visible);
            }
            for (int i = 0; i < m_FilterButtons.Length; i++)
                m_FilterButtons[i].interactable = i != index;
            if (top) m_CollectionScroll.verticalNormalizedPosition = 1f;
        }

        private void RefreshHeroInternal(HeroUnitConfig hero)
        {
            if (hero != null)
            {
                m_HeroName.text = hero.BaseInfo.Name;
                m_Portrait.sprite = hero.CollectibleField.Icon;
                int count = hero.HeroDeckConfig != null ? hero.HeroDeckConfig.CardConfigs.Length : 0;
                m_DeckCount.text = count + " / 4";
                foreach (var card in m_Deck.GetUsedCards().ToArray())
                    if (card != null && RushGameManager.Instance.RogueLikeManager.IsAutomaticCard(card.CardConfig, hero))
                        m_Deck.RemoveUsedCardConfig(card.CardConfig);
            }
            OnSlotsChangedInternal(m_Deck.GetUsedCards());
            ShowDeckInternal();
        }
    }
}

