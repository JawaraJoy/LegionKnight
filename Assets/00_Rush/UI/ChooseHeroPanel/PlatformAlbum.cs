using System.Collections.Generic;
using LegionKnight;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Rush
{
    public sealed class PlatformAlbum : MonoBehaviour
    {
        [SerializeField] private GameObject m_Collection;
        [SerializeField] private GameObject m_HeroDeck;
        [SerializeField] private TextMeshProUGUI m_Title;
        [SerializeField] private TextMeshProUGUI m_HeroName;
        [SerializeField] private TextMeshProUGUI m_DeckCount;
        [SerializeField] private TextMeshProUGUI m_SlotCount;
        [SerializeField] private Image m_Portrait;
        [SerializeField] private RectTransform m_Chevron;
        [SerializeField] private CardSelectTabView m_Tab;
        [SerializeField] private ScrollRect m_CollectionScroll;
        [SerializeField] private RarityConfig[] m_Rarities;
        [SerializeField] private Button[] m_FilterButtons;
        private CardDeck m_Deck;
        private HeroesCollection m_Heroes;
        [SerializeField] private PlatformAlbumGrid m_CollectionGrid;
        private int m_Filter;
        private bool m_CardsChanged;

        private void OnEnable()
        {
            m_CollectionGrid.CardsChanged += OnCardsChangedInternal;
            m_Deck = Player.Instance.PlayerCardDeck;
            m_Heroes = Player.Instance.HeroesCollection;
            m_Heroes.OnCharacterUsed.AddListener(RefreshHeroInternal);
            m_Deck.OnUsedCardsChanged.AddListener(RefreshSlotsInternal);
            RefreshHeroInternal(m_Heroes.UsedHero);
            RefreshSlotsInternal(m_Deck.GetUsedCards());
            SetPageInternal(false);
            FilterInternal(0);
        }

        private void OnDisable()
        {
            m_CollectionGrid.CardsChanged -= OnCardsChangedInternal;
            if (m_Heroes != null) m_Heroes.OnCharacterUsed.RemoveListener(RefreshHeroInternal);
            if (m_Deck != null) m_Deck.OnUsedCardsChanged.RemoveListener(RefreshSlotsInternal);
        }

        private void OnCardsChangedInternal() => m_CardsChanged = true;

        private void LateUpdate()
        {
            if (!m_CardsChanged) return;
            m_CardsChanged = false;
            if (m_Filter > 0) m_Tab.ShowRarity(m_Rarities[m_Filter - 1]);
        }

        public void ToggleDeck() => SetPageInternal(!m_HeroDeck.activeSelf);
        public void ShowCollection() => SetPageInternal(false);
        public void SetFilter(int index) => FilterInternal(index);

        private void SetPageInternal(bool deck)
        {
            m_Collection.SetActive(!deck);
            m_HeroDeck.SetActive(deck);
            m_Title.text = deck ? "Hero deck" : "Card collection";
            m_Chevron.localRotation = Quaternion.Euler(0f, 0f, deck ? 180f : 0f);
        }

        private void FilterInternal(int index)
        {
            if (index < 0 || index > m_Rarities.Length) return;
            m_Filter = index;
            if (index == 0) m_Tab.ShowAllCard();
            else m_Tab.ShowRarity(m_Rarities[index - 1]);
            for (int i = 0; i < m_FilterButtons.Length; i++)
                m_FilterButtons[i].interactable = i != m_Filter;
            m_CollectionScroll.verticalNormalizedPosition = 1f;
        }

        private void RefreshHeroInternal(HeroUnitConfig hero)
        {
            if (hero == null) return;
            m_HeroName.text = hero.BaseInfo.Name;
            m_Portrait.sprite = hero.CollectibleField.Icon;
            int count = hero.HeroDeckConfig != null ? hero.HeroDeckConfig.CardConfigs.Length : 0;
            m_DeckCount.text = $"{count} cards";
        }

        private void RefreshSlotsInternal(List<CardUnit> cards)
        {
            m_SlotCount.text = $"{cards.Count} / {m_Deck.GetMaxUsedCardCount()}";
        }
    }
}
