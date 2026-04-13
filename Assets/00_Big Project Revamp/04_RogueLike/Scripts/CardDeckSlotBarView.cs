using LegionKnight;
using System.Collections.Generic;
using UnityEngine;

namespace Rush
{
    /// <summary>
    /// Bar slot di atas scroll list card.
    /// - Spawn CardSlotView sejumlah MaxUsedCardCount saat Init
    /// - Refresh tiap kali OnUsedCardsChanged fired
    /// </summary>
    public class CardDeckSlotBarView : UIView
    {
        [SerializeField] private CardSlotView m_SlotPrefab;
        [SerializeField] private Transform m_SlotContainer;

        private readonly List<CardSlotView> m_Slots = new();

        // ── Init (dipanggil oleh CardSelectTabView) ───────────────────────────
        public void Init()
        {
            SpawnSlots();

            // Listen perubahan used list
            Player.Instance.PlayerCardDeck.OnUsedCardsChanged.AddListener(OnUsedCardsChanged);

            // Refresh awal sesuai save data yang sudah di-load
            Refresh(Player.Instance.PlayerCardDeck.GetUsedCards());
        }

        // ── Spawn slot sejumlah MaxUsedCardCount ──────────────────────────────
        private void SpawnSlots()
        {
            // Clear slot lama jika ada
            foreach (var slot in m_Slots)
            {
                if (slot != null) Destroy(slot.gameObject);
            }
            m_Slots.Clear();

            int max = Player.Instance.PlayerCardDeck.GetMaxUsedCardCount();
            for (int i = 0; i < max; i++)
            {
                CardSlotView slot = Instantiate(m_SlotPrefab, m_SlotContainer);
                slot.SetEmpty();
                slot.Show();
                m_Slots.Add(slot);
            }
        }

        // ── Refresh semua slot sesuai used list terkini ───────────────────────
        private void Refresh(List<CardUnit> usedCards)
        {
            for (int i = 0; i < m_Slots.Count; i++)
            {
                if (i < usedCards.Count)
                    m_Slots[i].SetFilled(usedCards[i]);
                else
                    m_Slots[i].SetEmpty();
            }
        }

        private void OnUsedCardsChanged(List<CardUnit> usedCards)
        {
            Refresh(usedCards);
        }

        private void OnDestroy()
        {
            if (Player.Instance != null)
                Player.Instance.PlayerCardDeck.OnUsedCardsChanged.RemoveListener(OnUsedCardsChanged);
        }
    }
}