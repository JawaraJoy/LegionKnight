using MoreMountains.Tools;
using Rush;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace LegionKnight
{
    [System.Serializable]
    public partial class CardUnit
    {
        [SerializeField, MMReadOnly]
        private bool m_IsOwned;
        [SerializeField, MMReadOnly]
        private bool m_IsAdded;
        [SerializeField]
        private int m_Amount;
        [SerializeField]
        private CardConfig m_CardConfig;
        public bool IsOwned => m_IsOwned = m_Amount > 0;
        public bool IsAdded => m_IsAdded;
        public int Amount => m_Amount;
        public CardConfig CardConfig => m_CardConfig;

        private string AmountKey => m_CardConfig.BaseInfo.Id + "amount";
        private string UsedKey => m_CardConfig.BaseInfo.Id + "used";
        public CardUnit(CardConfig cardConfig)
        {
            m_CardConfig = cardConfig;
            m_IsOwned = false;
            m_IsAdded = false;
        }
        public void AddAmount(int add)
        {
            m_Amount += add;
            UnityService.Instance.SaveData(AmountKey, m_Amount);
            m_IsOwned = m_Amount > 0;
        }
        public void SetIsAdded(bool set)
        {
            m_IsAdded = set;
            UnityService.Instance.SaveData(UsedKey, m_IsAdded);
        }
        public void Init()
        {
            if (UnityService.Instance.HasData(AmountKey))
            {
                m_Amount = UnityService.Instance.GetData<int>(AmountKey);
            }
            m_IsOwned = m_Amount > 0;
        }
    }

    public partial class CardDeck : MonoBehaviour
    {
        [SerializeField, MMReadOnly]
        private List<CardUnit> m_UsedCards = new();
        [SerializeField]
        private int m_MaxUsedCardCount = 5; // Clamp max used card
        [SerializeField]
        private CardUnit m_SelectedCard;
        [SerializeField]
        private List<CardUnit> m_CardCollections = new();

        [SerializeField]
        private UnityEvent m_OnInitialized = new();
        [SerializeField]
        private UnityEvent<CardUnit> m_OnInitializedUnit = new();
        [SerializeField]
        private UnityEvent<CardUnit> m_OnCardAdded = new();
        [SerializeField]
        private UnityEvent<List<CardUnit>> m_OnCardUnitUsed = new();
        [SerializeField]
        private UnityEvent<CardUnit> m_OnSelectedCard = new();
        public List<CardUnit> GetUsedCards() => m_UsedCards;
        public int GetMaxUsedCardCount() => m_MaxUsedCardCount;
        public UnityEvent<CardUnit> OnInitializedUnit => m_OnInitializedUnit;
        public UnityEvent<CardUnit> OnSelectedCard => m_OnSelectedCard;
        public UnityEvent<CardUnit> OnCardAdded => m_OnCardAdded;
        private CardUnit GetCardOwnedInternal(CardConfig cardConfig)
        {
            foreach (var cardOwned in m_CardCollections)
            {
                if (cardOwned.CardConfig.BaseInfo.Id == cardConfig.BaseInfo.Id)
                    return cardOwned;
            }
            return null;
        }

        public CardUnit[] GetCardUnits() => m_CardCollections.ToArray();

        public CardUnit GetCardOwned(CardConfig cardConfig) => GetCardOwnedInternal(cardConfig);

        private PreparationPanel m_PreparationPanel;
        private PreparationPanel PreparationPanel
        {
            get
            {
                if (m_PreparationPanel == null)
                {
                    m_PreparationPanel = CanvasManager.Instance.GetPanel<PreparationPanel>();
                }
                return m_PreparationPanel;
            }
        }
        public bool IsCardOwned(CardConfig cardConfig)
        {
            var cardOwned = GetCardOwnedInternal(cardConfig);
            return cardOwned != null && cardOwned.IsOwned;
        }

        public void SetIsEquiped(CardConfig config, bool isEquiped)
        {
            foreach (var platformOwned in m_CardCollections)
                platformOwned.SetIsAdded(false);
            GetCardOwnedInternal(config)?.SetIsAdded(isEquiped);
        }

        public void AddCardAmount(CardConfig config, int add) => AddCardAmountInternal(config, add);

        private void AddCardAmountInternal(CardConfig config, int add)
        {
            var cardOwned = GetCardOwnedInternal(config);
            cardOwned?.AddAmount(add);
            m_OnCardAdded?.Invoke(GetCardOwnedInternal(config));
        }

        // Add selected card to used list (with clamp & duplicate check)
        public void SetUsedCardConfig()
        {
            if (m_SelectedCard == null) return;

            if (m_UsedCards.Contains(m_SelectedCard))
            {
                Debug.Log("Card already in used list.");
                return;
            }

            if (m_UsedCards.Count >= m_MaxUsedCardCount)
            {
                Debug.Log($"Max used card limit ({m_MaxUsedCardCount}) reached.");
                return;
            }

            m_UsedCards.Add(m_SelectedCard);
            AddCardAmountInternal(m_SelectedCard.CardConfig, -1);
            OnCardConfigUsedInvoke();
        }

        // Remove card from used list
        public void RemoveUsedCardConfig(CardConfig cardConfig)
        {
            if (GetCardOwnedInternal(cardConfig) != null)
            {
                m_UsedCards.Remove(GetCardOwnedInternal(cardConfig));
                AddCardAmountInternal(m_SelectedCard.CardConfig, 1);
                OnCardConfigUsedInvoke();
            }
        }

        // Only update UI preview, does NOT modify used list
        public void SelectStandbyCardConfig(CardConfig cardConfig)
        {
            m_SelectedCard = GetCardOwnedInternal(cardConfig);
            OnSelectedCardConfigInvoke();
        }

        // Use ALL cards in used list, add each to roguelike deck
        public void UseCardConfig()
        {
            foreach (var cardunit in m_UsedCards)
            {
                var cardUnit = GetCardOwnedInternal(cardunit.CardConfig);
                if (cardUnit != null && cardUnit.IsOwned)
                {
                    RushGameManager.Instance.RogueLikeManager.AddCard(cardunit.CardConfig);
                    AddCardAmountInternal(cardunit.CardConfig, -1);
                }
            }
        }

        public void Init() => OnInitializedInvoke();

        private void OnInitializedInvoke()
        {
            m_OnInitialized?.Invoke();
            foreach (CardUnit unit in m_CardCollections)
            {
                unit.Init();
                //PreparationPanel.CardTabView.SpawnCardSelect(unit);
                m_OnInitializedUnit?.Invoke(unit);
            }
        }

        private void OnSelectedCardConfigInvoke()
        {
            m_OnSelectedCard?.Invoke(m_SelectedCard);
        }

        private void OnCardConfigUsedInvoke()
        {
            m_OnCardUnitUsed?.Invoke(m_UsedCards);
        }
    }
}