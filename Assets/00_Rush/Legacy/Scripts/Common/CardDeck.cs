using MoreMountains.Tools;
using Rush;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace LegionKnight
{
    public partial class CardDeck : MonoBehaviour
    {
        [SerializeField, MMReadOnly] private List<CardUnit> m_UsedCards = new();
        [SerializeField] private int m_MaxUsedCardCount = 4;
        [SerializeField] private CardUnit m_SelectedCard;
        [SerializeField] private List<CardUnit> m_CardCollections = new();
        [SerializeField] private UnityEvent m_OnInitialized = new();
        [SerializeField] private UnityEvent<CardUnit> m_OnInitializedUnit = new();
        [SerializeField] private UnityEvent<CardUnit> m_OnCardAdded = new();
        [SerializeField] private UnityEvent<List<CardUnit>> m_OnCardUnitUsed = new();
        [SerializeField] private UnityEvent<CardUnit> m_OnSelectedCard = new();
        [SerializeField] private UnityEvent<List<CardUnit>> m_OnUsedCardsChanged = new();
        [SerializeField] private CardConfig[] m_AdditionalCards;
        private PreparationPanel m_PreparationPanel;

        public DeckConfig UsedHeroDeck => Player.Instance.HeroesCollection.UsedHero.HeroDeckConfig;
        public UnityEvent<CardUnit> OnInitializedUnit => m_OnInitializedUnit;
        public UnityEvent<CardUnit> OnSelectedCard => m_OnSelectedCard;
        public UnityEvent<CardUnit> OnCardAdded => m_OnCardAdded;
        public UnityEvent<List<CardUnit>> OnUsedCardsChanged => m_OnUsedCardsChanged;
        private PreparationPanel PreparationPanel
        {
            get
            {
                if (m_PreparationPanel == null)
                    m_PreparationPanel = CanvasManager.Instance.GetPanel<PreparationPanel>();
                return m_PreparationPanel;
            }
        }

        public List<CardUnit> GetUsedCards() => m_UsedCards;
        public int GetMaxUsedCardCount() => m_MaxUsedCardCount;
        public CardUnit[] GetCardUnits() => m_CardCollections.ToArray();
        public CardUnit GetCardOwned(CardConfig config) => GetCardOwnedInternal(config);
        public bool IsCardOwned(CardConfig config) => GetCardOwnedInternal(config)?.IsOwned == true;

        private CardUnit GetCardOwnedInternal(CardConfig config)
        {
            if (config == null) return null;
            return m_CardCollections.Find(card => card != null && card.CardConfig != null &&
                card.CardConfig.BaseInfo.Id == config.BaseInfo.Id);
        }

        public void SetIsEquipped(CardConfig config, bool isEquipped)
        {
            if (isEquipped)
                AddUsedCardInternal(GetCardOwnedInternal(config));
            else
                RemoveUsedCardConfigInternal(config);
        }

        public void AddCardAmount(CardConfig config, int add) => AddCardAmountInternal(config, add);
        private void AddCardAmountInternal(CardConfig config, int add)
        {
            CardUnit card = GetCardOwnedInternal(config);
            if (card == null) return;
            card.ChangeCopies(add, () => OnCardAddedInvoke(card));
        }

        public void UpgradeCard(CardUnit card, int expected)
        {
            if (card != null && GetCardOwnedInternal(card.CardConfig) == card)
                card.UpgradeWithCopies(expected, () => OnCardAddedInvoke(card));
        }

        public void SelectStandbyCardConfig(CardConfig config)
        {
            m_SelectedCard = GetCardOwnedInternal(config);
            OnSelectedCardInvoke(m_SelectedCard);
        }

        public void SetUsedCardConfig() => AddUsedCardInternal(m_SelectedCard);
        private void AddUsedCardInternal(CardUnit card)
        {
            if (card == null || card.CardConfig == null || !card.IsOwned ||
                !card.CardConfig.IsAvailableForHero(Player.Instance.HeroesCollection.UsedHero) ||
                m_UsedCards.Contains(card) ||
                m_UsedCards.Count >= m_MaxUsedCardCount)
                return;
            m_UsedCards.Add(card);
            card.SetIsAdded(true);
            OnCardConfigUsedInvoke();
        }

        public void RemoveUsedCardConfig(CardConfig config) => RemoveUsedCardConfigInternal(config);
        private void RemoveUsedCardConfigInternal(CardConfig config)
        {
            CardUnit card = GetCardOwnedInternal(config);
            if (card == null) return;
            bool removed = m_UsedCards.Remove(card);
            if (card.IsAdded) card.SetIsAdded(false);
            if (removed) OnCardConfigUsedInvoke();
        }

        public void PrepareUsedCards() => PrepareUsedCardsInternal();
        private void PrepareUsedCardsInternal()
        {
            for (int i = m_UsedCards.Count - 1; i >= 0; i--)
            {
                CardUnit card = m_UsedCards[i];
                if (card == null || card.CardConfig == null)
                {
                    m_UsedCards.RemoveAt(i);
                    continue;
                }
                if (!card.IsOwned || !card.CardConfig.IsAvailableForHero(Player.Instance.HeroesCollection.UsedHero))
                    RemoveUsedCardConfigInternal(card.CardConfig);
            }
        }

        public void UseCardConfig() => UseCardConfigInternal();
        private void UseCardConfigInternal()
        {
            PrepareUsedCardsInternal();
            var manager = RushGameManager.Instance.RogueLikeManager;
            manager.ClearCustomCards();
            foreach (CardUnit card in m_UsedCards)
                manager.AddCustomCard(card.CardConfig);
        }

        public void Init() => InitializeInternal();
        private void InitializeInternal()
        {
            m_UsedCards.Clear();
            RushGameManager.Instance.RogueLikeManager.ClearCustomCards();
            var heroes = Player.Instance.HeroesCollection;
            var entitled = new HashSet<CardConfig>();
            foreach (var config in m_AdditionalCards ?? System.Array.Empty<CardConfig>())
                if (config != null && GetCardOwnedInternal(config) == null) m_CardCollections.Add(new CardUnit(config));
            foreach (var unit in heroes.HeroUnits)
            {
                if (unit?.HeroConfig?.HeroDeckConfig?.CardConfigs == null) continue;
                bool owned = unit.Owned || unit.HeroConfig == heroes.DefaultHero ||
                    UnityService.Instance.GetData<bool>(unit.HeroConfig.BaseInfo.Id + "Owned");
                foreach (var config in unit.HeroConfig.HeroDeckConfig.CardConfigs)
                {
                    if (config == null) continue;
                    if (GetCardOwnedInternal(config) == null) m_CardCollections.Add(new CardUnit(config));
                    if (owned) entitled.Add(config);
                }
            }
            heroes.OnCharacterOwned.RemoveListener(GrantHeroCardsInternal);
            heroes.OnCharacterOwned.AddListener(GrantHeroCardsInternal);
            OnInitializedInvoke();
            foreach (CardUnit card in m_CardCollections)
            {
                if (card == null || card.CardConfig == null) continue;
                card.Init(entitled.Contains(card.CardConfig));
                if (card.IsAdded)
                {
                    if (card.IsOwned && card.CardConfig.IsAvailableForHero(Player.Instance.HeroesCollection.UsedHero) &&
                        m_UsedCards.Count < m_MaxUsedCardCount)
                        m_UsedCards.Add(card);
                    else
                        card.SetIsAdded(false);
                }
                PreparationPanel.CardTabView.SpawnCardSelect(card);
                OnInitializedUnitInvoke(card);
            }
            OnCardConfigUsedInvoke();
            PreparationPanel.CardTabView.CardDeckSlotBarView.Init();
            PreparationPanel.CardTabView.DefaultCardDeckView.Init();
        }

        private void GrantHeroCardsInternal(HeroUnitConfig hero)
        {
            if (hero?.HeroDeckConfig?.CardConfigs == null) return;
            foreach (var config in hero.HeroDeckConfig.CardConfigs)
            {
                var card = GetCardOwnedInternal(config);
                card?.GrantHeroCard(() => OnCardAddedInvoke(card));
            }
        }

        private void OnInitializedInvoke() => m_OnInitialized?.Invoke();
        private void OnInitializedUnitInvoke(CardUnit card) => m_OnInitializedUnit?.Invoke(card);
        private void OnCardAddedInvoke(CardUnit card) => m_OnCardAdded?.Invoke(card);
        private void OnSelectedCardInvoke(CardUnit card) => m_OnSelectedCard?.Invoke(card);
        private void OnCardConfigUsedInvoke()
        {
            m_OnCardUnitUsed?.Invoke(m_UsedCards);
            m_OnUsedCardsChanged?.Invoke(m_UsedCards);
        }
    }
}

