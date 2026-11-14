using LegionKnight;
using System.Collections.Generic;
using UnityEngine;

namespace Rush
{
    public class CardDeckSlotBarView : UIView
    {
        [SerializeField] private CardSlotView m_SlotPrefab;
        [SerializeField] private Transform m_SlotContainer;
        [SerializeField] private PlatformAlbum m_Album;
        private readonly List<CardSlotView> m_Slots = new();
        private CardDeck m_Deck;

        public void Init()
        {
            if (m_Deck != null) m_Deck.OnUsedCardsChanged.RemoveListener(RefreshInternal);
            m_Deck = Player.Instance.PlayerCardDeck;
            while (m_Slots.Count < m_Deck.GetMaxUsedCardCount())
            {
                var slot = Instantiate(m_SlotPrefab, m_SlotContainer);
                slot.BindAlbum(m_Album, m_Slots.Count);
                slot.Show();
                m_Slots.Add(slot);
            }
            m_Deck.OnCardAdded.RemoveListener(OnCardChanged);
            m_Deck.OnCardAdded.AddListener(OnCardChanged);
            m_Deck.OnUsedCardsChanged.AddListener(RefreshInternal);
            RefreshInternal(m_Deck.GetUsedCards());
        }

        private void OnCardChanged(CardUnit card) { if (card.IsAdded) RefreshInternal(m_Deck.GetUsedCards()); }
        public void Refresh() { if (m_Deck != null) RefreshInternal(m_Deck.GetUsedCards()); }

        private void RefreshInternal(List<CardUnit> equipped)
        {
            var slots = new CardUnit[m_Slots.Count];
            foreach (var unit in equipped)
            {
                int i = unit.DeckSlot;
                if (i >= 0 && i < slots.Length && slots[i] == null) slots[i] = unit;
            }
            foreach (var unit in equipped)
            {
                if (System.Array.IndexOf(slots, unit) >= 0) continue;
                int i = System.Array.IndexOf(slots, null);
                if (i < 0) break;
                unit.AssignSlot(i);
                slots[i] = unit;
            }
            for (int i = 0; i < slots.Length; i++)
            {
                if (slots[i] == null) m_Slots[i].SetEmpty();
                else m_Slots[i].SetFilled(slots[i]);
            }
        }

        private void OnDestroy()
        {
            if (m_Deck != null) m_Deck.OnCardAdded.RemoveListener(OnCardChanged);
            if (m_Deck != null) m_Deck.OnUsedCardsChanged.RemoveListener(RefreshInternal);
        }
    }
}

