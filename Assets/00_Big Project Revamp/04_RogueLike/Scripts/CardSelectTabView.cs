using LegionKnight;
using Rush;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace LegionKnight
{
    public class CardSelectTabView : UIView
    {
        [SerializeField] private CardDeckSlotBarView m_DeckSlotBarView;
        [SerializeField] private DefaultCardDeckView m_DefaultCardDeckView;

        [Header("Card List")]
        [SerializeField] private AssetReferenceGameObject m_CardSelectViewAsset;
        [SerializeField] private Transform m_SpawnSpot;
        [SerializeField] private List<CardSelectView> m_SpawnedCardSelectionViews = new();

        [Header("Detail")]
        [SerializeField] private CardDetailView m_CardDetailView;

        public CardDeckSlotBarView CardDeckSlotBarView => m_DeckSlotBarView;
        public CardDetailView CardDetailView => m_CardDetailView;
        public DefaultCardDeckView DefaultCardDeckView => m_DefaultCardDeckView;

        // Track berapa card yang belum selesai spawn (async)
        private int m_PendingSpawnCount = 0;

        // ── Init slot bar ─────────────────────────────────────────────────────
        public void InitSlotBar()
        {
            if (m_DeckSlotBarView != null)
                m_DeckSlotBarView.Init();
        }

        private void Awake()
        {
            // Re-sort saat amount card berubah (misal setelah add/remove dari deck)
            Player.Instance.PlayerCardDeck.OnCardAdded.AddListener(_ => SortCardViews());
        }

        // ── Spawn card select item ────────────────────────────────────────────
        public void SpawnCardSelect(CardUnit unit)
        {
            SpawnCardSelectInternal(unit);
        }

        private void SpawnCardSelectInternal(CardUnit unit)
        {
            CardSelectView existing = GetSelectView(unit.CardConfig);
            if (existing != null)
            {
                existing.Init(unit);
                SortCardViews();
                return;
            }

            m_PendingSpawnCount++;
            AsyncOperationHandle<GameObject> handle =
                m_CardSelectViewAsset.InstantiateAsync(m_SpawnSpot, false);

            RushGameManager.Instance.StartCoroutine(SpawningCardSelectView(handle, unit));
        }

        private IEnumerator SpawningCardSelectView(AsyncOperationHandle<GameObject> handle, CardUnit unit)
        {
            yield return handle;

            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                GameObject spawned = handle.Result;
                if (spawned.TryGetComponent(out CardSelectView view))
                {
                    view.Init(unit);
                    if (!m_SpawnedCardSelectionViews.Contains(view))
                        m_SpawnedCardSelectionViews.Add(view);
                }
            }

            m_PendingSpawnCount--;

            // Sort hanya setelah semua card selesai di-spawn
            if (m_PendingSpawnCount <= 0)
            {
                m_PendingSpawnCount = 0;
                SortCardViews();
            }
        }

        // ── Sort: amount > 0 di atas, amount = 0 di bawah ────────────────────
        /// <summary>
        /// Sort berdasarkan amount descending — card yang punya amount
        /// lebih banyak tampil lebih atas. Amount 0 selalu paling bawah.
        /// Pakai sibling index di Transform untuk mengatur urutan UI.
        /// </summary>
        private void SortCardViews()
        {
            if (m_SpawnedCardSelectionViews.Count == 0) return;

            // Sort list berdasarkan amount descending
            m_SpawnedCardSelectionViews.Sort((a, b) =>
            {
                int amountA = Player.Instance.PlayerCardDeck
                    .GetCardOwned(a.CardConfig)?.Amount ?? 0;
                int amountB = Player.Instance.PlayerCardDeck
                    .GetCardOwned(b.CardConfig)?.Amount ?? 0;

                return amountB.CompareTo(amountA); // descending
            });

            // Apply urutan ke sibling index di container
            for (int i = 0; i < m_SpawnedCardSelectionViews.Count; i++)
            {
                m_SpawnedCardSelectionViews[i].transform.SetSiblingIndex(i);
            }
        }

        // ── Show / Hide ───────────────────────────────────────────────────────
        protected override void ShowInternal()
        {
            base.ShowInternal();
            m_CardDetailView.Hide();
        }

        protected override void HideInternal()
        {
            HideAllCardInternal();
            base.HideInternal();
        }

        public void ShowAllCard() => ShowAllCardInternal();
        public void HideAllCard() => HideAllCardInternal();

        private void ShowAllCardInternal()
        {
            foreach (var view in m_SpawnedCardSelectionViews)
                view.Show();
        }

        private void HideAllCardInternal()
        {
            foreach (var view in m_SpawnedCardSelectionViews)
                view.Hide();
        }

        // ── Filter by rarity ──────────────────────────────────────────────────
        public void ShowRarity(RarityConfig rarityConfig)
        {
            foreach (var view in m_SpawnedCardSelectionViews)
                view.Hide();

            foreach (var view in GetCardSelectViews(rarityConfig))
                view.Show();
        }

        // ── Lookup ────────────────────────────────────────────────────────────
        private CardSelectView GetSelectView(CardConfig cardConfig)
        {
            return m_SpawnedCardSelectionViews.Find(x => x.CardConfig == cardConfig);
        }

        private CardSelectView[] GetCardSelectViews(RarityConfig rarityConfig)
        {
            return m_SpawnedCardSelectionViews
                .FindAll(x => x.CardConfig.CollectibleField.RarityConfig.BaseInfo.Id
                              == rarityConfig.BaseInfo.Id)
                .ToArray();
        }
    }
}