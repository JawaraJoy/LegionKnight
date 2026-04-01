using Rush;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace LegionKnight
{
    [System.Serializable]
    public partial class CardUnit
    {
        [SerializeField]
        private bool m_IsOwned;
        [SerializeField]
        private bool m_IsEquiped;
        [SerializeField]
        private int m_Amount;
        [SerializeField]
        private CardConfig m_CardConfig;
        public bool IsOwned => m_IsOwned = m_Amount > 0;
        public bool IsEquiped => m_IsEquiped;
        public int Amount => m_Amount;
        public CardConfig CardConfig => m_CardConfig;
        public CardUnit(CardConfig cardConfig)
        {
            m_CardConfig = cardConfig;
            m_IsOwned = false;
            m_IsEquiped = false;
        }
        public void AddAmount(int add)
        {
            m_Amount += add;
            UnityService.Instance.SaveData(m_CardConfig.BaseInfo.Id + "amount", m_Amount);
            m_IsOwned = m_Amount > 0;
        }
        public void SetIsEquiped(bool set)
        {
            m_IsEquiped = set;
        }
        public void Init()
        {
            if (UnityService.Instance.HasData(m_CardConfig.BaseInfo.Id + "amount"))
            {
                m_Amount = UnityService.Instance.GetData<int>(m_CardConfig.BaseInfo.Id + "amount");
            }
            else
            {
                m_Amount = 0;
                UnityService.Instance.SaveData(m_CardConfig.BaseInfo.Id + "amount", m_Amount);
            }
            m_IsOwned = m_Amount > 0;
        }
    }

    public partial class CardDeck : MonoBehaviour
    {
        [SerializeField]
        private List<CardConfig> m_UsedCardConfig = new();
        [SerializeField]
        private int m_MaxUsedCardCount = 5; // Clamp max used card
        [SerializeField]
        private CardConfig m_SelectedCard;
        [SerializeField]
        private List<CardUnit> m_Deck = new();

        [SerializeField]
        private UnityEvent m_OnInitialized = new();
        [SerializeField]
        private UnityEvent<CardUnit> m_OnInitializedUnit = new();
        [SerializeField]
        private UnityEvent<List<CardConfig>> m_OnCardConfigUsed = new();
        [SerializeField]
        private UnityEvent<CardConfig> m_OnSelectedPlatform = new();

        private CardUnit GetCardOwnedInternal(CardConfig cardConfig)
        {
            foreach (var platformOwned in m_Deck)
            {
                if (platformOwned.CardConfig.BaseInfo.Id == cardConfig.BaseInfo.Id)
                    return platformOwned;
            }
            return null;
        }

        public CardUnit[] GetCardUnits() => m_Deck.ToArray();

        public CardUnit GetCardOwned(CardConfig cardConfig) => GetCardOwnedInternal(cardConfig);

        public bool IsCardOwned(CardConfig cardConfig)
        {
            var cardOwned = GetCardOwnedInternal(cardConfig);
            return cardOwned != null && cardOwned.IsOwned;
        }

        public List<CardConfig> GetUsedCardConfig() => m_UsedCardConfig;

        public int GetMaxUsedCardCount() => m_MaxUsedCardCount;

        public void SetIsEquiped(CardConfig config, bool isEquiped)
        {
            foreach (var platformOwned in m_Deck)
                platformOwned.SetIsEquiped(false);
            GetCardOwnedInternal(config)?.SetIsEquiped(isEquiped);
        }

        public void AddCardAmount(CardConfig config, int add) => AddCardAmountInternal(config, add);

        public void AddCardAmountInternal(CardConfig config, int add)
        {
            var cardOwned = GetCardOwnedInternal(config);
            cardOwned?.AddAmount(add);
        }

        // Add selected card to used list (with clamp & duplicate check)
        public void SetUsedCardConfig()
        {
            if (m_SelectedCard == null) return;

            if (m_UsedCardConfig.Contains(m_SelectedCard))
            {
                Debug.Log("Card already in used list.");
                return;
            }

            if (m_UsedCardConfig.Count >= m_MaxUsedCardCount)
            {
                Debug.Log($"Max used card limit ({m_MaxUsedCardCount}) reached.");
                return;
            }

            m_UsedCardConfig.Add(m_SelectedCard);
            OnCardConfigUsedInvoke();
        }

        // Remove card from used list
        public void RemoveUsedCardConfig(CardConfig cardConfig)
        {
            if (m_UsedCardConfig.Contains(cardConfig))
            {
                m_UsedCardConfig.Remove(cardConfig);
                OnCardConfigUsedInvoke();
            }
        }

        // Only update UI preview, does NOT modify used list
        public void SelectStandbyCardConfig(CardConfig cardConfig)
        {
            m_SelectedCard = cardConfig;
            OnSelectedCardConfigInvoke();
        }

        // Use ALL cards in used list, add each to roguelike deck
        public void UseCardConfig()
        {
            foreach (var cardConfig in m_UsedCardConfig)
            {
                var cardUnit = GetCardOwnedInternal(cardConfig);
                if (cardUnit != null && cardUnit.IsOwned)
                {
                    RushGameManager.Instance.RogueLikeManager.AddCard(cardConfig);
                    AddCardAmountInternal(cardConfig, -1);
                }
            }
        }

        public void Init() => OnInitializedInvoke();

        private void OnInitializedInvoke()
        {
            m_OnInitialized?.Invoke();
            foreach (CardUnit unit in m_Deck)
            {
                unit.Init();
                m_OnInitializedUnit?.Invoke(unit);
            }
        }

        private void OnSelectedCardConfigInvoke()
        {
            m_OnSelectedPlatform?.Invoke(m_SelectedCard);
            CanvasManager.Instance.SetCardConfigSelected(m_SelectedCard);
        }

        private void OnCardConfigUsedInvoke()
        {
            m_OnCardConfigUsed?.Invoke(m_UsedCardConfig);
            CanvasManager.Instance.SetUsedCardConfigList(m_UsedCardConfig);
        }
    }
}