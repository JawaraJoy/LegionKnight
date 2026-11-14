using LegionKnight;
using System.Collections.Generic;
using UnityEngine;

namespace Rush
{
    /// <summary>
    /// Menampilkan semua card dari DeckConfig hero yang sedang aktif.
    /// Read-only — tidak ada Add/Remove.
    /// Pakai object pooling: item di-reuse saat hero berganti,
    /// tidak Destroy/Instantiate ulang setiap kali.
    /// </summary>
    public class DefaultCardDeckView : UIView
    {
        [SerializeField] private DefaultCardItemView m_ItemPrefab;
        [SerializeField] private Transform m_ItemContainer;

        // ── Pool ──────────────────────────────────────────────────────────────
        private readonly Queue<DefaultCardItemView> m_Pool = new();
        private readonly List<DefaultCardItemView> m_ActiveItems = new();

        // ── Init (dipanggil sekali, misal dari PreparationPanel.ShowInternal) ─
        public void Init()
        {
            // Listen saat hero berganti → refresh deck
            Player.Instance.HeroesCollection.OnCharacterUsed.RemoveListener(OnHeroChanged);
            Player.Instance.HeroesCollection.OnCharacterUsed.AddListener(OnHeroChanged);

            // Tampilkan deck hero yang sedang aktif
            var changed = Player.Instance.PlayerCardDeck.OnCardAdded;
            changed.RemoveListener(OnCardChanged);
            changed.AddListener(OnCardChanged);
            RefreshDeck();
        }

        private void OnCardChanged(CardUnit unit)
        {
            var cards = Player.Instance.PlayerCardDeck.UsedHeroDeck?.CardConfigs;
            if (cards != null && System.Array.IndexOf(cards, unit.CardConfig) >= 0) RefreshDeck();
        }

        // ── Refresh saat hero berganti ────────────────────────────────────────
        private void OnHeroChanged(HeroUnitConfig _)
        {
            RefreshDeck();
        }

        // ── Core refresh ──────────────────────────────────────────────────────
        private void RefreshDeck()
        {
            DeckConfig deck = Player.Instance.PlayerCardDeck.UsedHeroDeck;

            if (deck == null)
            {
                Debug.LogWarning("[DefaultCardDeckView] UsedHeroDeck is null.");
                ReturnAllToPool();
                return;
            }

            CardConfig[] cards = deck.CardConfigs;

            // Kembalikan semua active item ke pool dulu
            ReturnAllToPool();

            if (cards == null || cards.Length == 0) return;

            foreach (CardConfig config in cards)
            {
                if (config == null) continue;

                CardUnit unit = Player.Instance.PlayerCardDeck.GetCardOwned(config);
                if (unit == null) continue;

                DefaultCardItemView item = GetFromPool();
                item.Setup(unit, m_ActiveItems.Count);
                item.Show();
                m_ActiveItems.Add(item);
            }
        }

        // ── Pool helpers ──────────────────────────────────────────────────────
        private DefaultCardItemView GetFromPool()
        {
            if (m_Pool.Count > 0)
            {
                DefaultCardItemView pooled = m_Pool.Dequeue();
                pooled.transform.SetParent(m_ItemContainer, false);
                return pooled;
            }

            // Pool kosong → instantiate baru
            DefaultCardItemView newItem = Instantiate(m_ItemPrefab, m_ItemContainer);
            return newItem;
        }

        private void ReturnAllToPool()
        {
            foreach (DefaultCardItemView item in m_ActiveItems)
            {
                item.Hide();
                // Pindah ke luar container agar tidak terlihat di layout
                item.transform.SetParent(m_ItemContainer.parent, false);
                m_Pool.Enqueue(item);
            }
            m_ActiveItems.Clear();
        }

        private void OnDestroy()
        {
            if (Player.Instance != null) Player.Instance.PlayerCardDeck.OnCardAdded.RemoveListener(OnCardChanged);
            if (Player.Instance != null)
                Player.Instance.HeroesCollection.OnCharacterUsed
                    .RemoveListener(OnHeroChanged);
        }
    }
}

