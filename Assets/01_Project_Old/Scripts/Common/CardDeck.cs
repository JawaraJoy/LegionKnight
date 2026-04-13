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
                m_Amount = UnityService.Instance.GetData<int>(AmountKey);

            if (UnityService.Instance.HasData(UsedKey))
                m_IsAdded = UnityService.Instance.GetData<bool>(UsedKey);

            m_IsOwned = m_Amount > 0;
        }
    }

    public partial class CardDeck : MonoBehaviour
    {
        [SerializeField, MMReadOnly]
        private List<CardUnit> m_UsedCards = new();
        [SerializeField]
        private int m_MaxUsedCardCount = 5;
        [SerializeField]
        private CardUnit m_SelectedCard;
        [SerializeField]
        private List<CardUnit> m_CardCollections = new();

        [SerializeField] private UnityEvent m_OnInitialized = new();
        [SerializeField] private UnityEvent<CardUnit> m_OnInitializedUnit = new();
        [SerializeField] private UnityEvent<CardUnit> m_OnCardAdded = new();
        [SerializeField] private UnityEvent<List<CardUnit>> m_OnCardUnitUsed = new();
        [SerializeField] private UnityEvent<CardUnit> m_OnSelectedCard = new();
        // ✅ Event baru — dipanggil tiap kali used list berubah (add/remove)
        [SerializeField] private UnityEvent<List<CardUnit>> m_OnUsedCardsChanged = new();

        public List<CardUnit> GetUsedCards() => m_UsedCards;
        public int GetMaxUsedCardCount() => m_MaxUsedCardCount;
        public UnityEvent<CardUnit> OnInitializedUnit => m_OnInitializedUnit;
        public UnityEvent<CardUnit> OnSelectedCard => m_OnSelectedCard;
        public UnityEvent<CardUnit> OnCardAdded => m_OnCardAdded;
        public UnityEvent<List<CardUnit>> OnUsedCardsChanged => m_OnUsedCardsChanged;

        // ── Lookup ────────────────────────────────────────────────────────────
        private CardUnit GetCardOwnedInternal(CardConfig cardConfig)
        {
            foreach (var card in m_CardCollections)
            {
                if (card.CardConfig.BaseInfo.Id == cardConfig.BaseInfo.Id)
                    return card;
            }
            return null;
        }

        public CardUnit[] GetCardUnits() => m_CardCollections.ToArray();
        public CardUnit GetCardOwned(CardConfig cardConfig) => GetCardOwnedInternal(cardConfig);
        public bool IsCardOwned(CardConfig cardConfig)
        {
            var card = GetCardOwnedInternal(cardConfig);
            return card != null && card.IsOwned;
        }

        // ── PreparationPanel lazy ref ─────────────────────────────────────────
        private PreparationPanel m_PreparationPanel;
        private PreparationPanel PreparationPanel
        {
            get
            {
                if (m_PreparationPanel == null)
                    m_PreparationPanel = CanvasManager.Instance.GetPanel<PreparationPanel>();
                return m_PreparationPanel;
            }
        }

        // ── Equip state ───────────────────────────────────────────────────────
        /// <summary>
        /// Toggle IsAdded pada satu card tanpa menyentuh card lain.
        /// Berbeda dari versi lama yang reset semua ke false dulu.
        /// </summary>
        public void SetIsEquipped(CardConfig config, bool isEquipped)
        {
            GetCardOwnedInternal(config)?.SetIsAdded(isEquipped);
        }

        // ── Amount ────────────────────────────────────────────────────────────
        public void AddCardAmount(CardConfig config, int add) => AddCardAmountInternal(config, add);

        private void AddCardAmountInternal(CardConfig config, int add)
        {
            var card = GetCardOwnedInternal(config);
            card?.AddAmount(add);
            m_OnCardAdded?.Invoke(GetCardOwnedInternal(config));
        }

        // ── Select (preview only, tidak ubah used list) ───────────────────────
        public void SelectStandbyCardConfig(CardConfig cardConfig)
        {
            m_SelectedCard = GetCardOwnedInternal(cardConfig);
            m_OnSelectedCard?.Invoke(m_SelectedCard);
        }

        // ── Add card ke used list ─────────────────────────────────────────────
        public void SetUsedCardConfig()
        {
            if (m_SelectedCard == null) return;

            if (m_UsedCards.Contains(m_SelectedCard))
            {
                Debug.Log("[CardDeck] Card already in used list.");
                return;
            }

            if (m_UsedCards.Count >= m_MaxUsedCardCount)
            {
                Debug.Log($"[CardDeck] Max used card limit ({m_MaxUsedCardCount}) reached.");
                return;
            }

            m_UsedCards.Add(m_SelectedCard);

            // ✅ Mark sebagai equipped dan kurangi amount
            SetIsEquipped(m_SelectedCard.CardConfig, true);
            AddCardAmountInternal(m_SelectedCard.CardConfig, -1);

            OnCardConfigUsedInvoke();
        }

        // ── Remove card dari used list ────────────────────────────────────────
        /// <summary>
        /// Fix dari versi lama: pakai parameter cardConfig bukan m_SelectedCard
        /// sehingga remove card yang benar meski player sedang preview card lain.
        /// </summary>
        public void RemoveUsedCardConfig(CardConfig cardConfig)
        {
            CardUnit card = GetCardOwnedInternal(cardConfig);
            if (card == null) return;

            if (!m_UsedCards.Contains(card))
            {
                Debug.Log("[CardDeck] Card not in used list.");
                return;
            }

            m_UsedCards.Remove(card);

            // ✅ Unmark equipped dan kembalikan amount
            SetIsEquipped(cardConfig, false);
            AddCardAmountInternal(cardConfig, 1); // ✅ fix: pakai cardConfig bukan m_SelectedCard

            OnCardConfigUsedInvoke();
        }

        // ── Use all cards (roguelike run) ─────────────────────────────────────
        public void UseCardConfig()
        {
            foreach (var cardUnit in m_UsedCards)
            {
                var card = GetCardOwnedInternal(cardUnit.CardConfig);
                if (card != null && card.IsOwned)
                {
                    RushGameManager.Instance.RogueLikeManager.AddCard(cardUnit.CardConfig);
                    AddCardAmountInternal(cardUnit.CardConfig, -1);
                }
            }
        }

        // ── Init ──────────────────────────────────────────────────────────────
        public void Init() => OnInitializedInvoke();

        private void OnInitializedInvoke()
        {
            m_OnInitialized?.Invoke();
            foreach (CardUnit unit in m_CardCollections)
            {
                unit.Init();

                // Restore used list dari save data
                if (unit.IsAdded && !m_UsedCards.Contains(unit))
                    m_UsedCards.Add(unit);

                PreparationPanel.CardTabView.SpawnCardSelect(unit);
                m_OnInitializedUnit?.Invoke(unit);
            }
        }

        // ── Events ────────────────────────────────────────────────────────────
        private void OnCardConfigUsedInvoke()
        {
            m_OnCardUnitUsed?.Invoke(m_UsedCards);
            m_OnUsedCardsChanged?.Invoke(m_UsedCards); // ✅ notify semua listener
        }
    }
}