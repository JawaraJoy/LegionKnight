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
        [Header("Deck Slot Bar")]
        [SerializeField] private CardDeckSlotBarView m_DeckSlotBarView;

        [Header("Card List")]
        [SerializeField] private AssetReferenceGameObject m_CardSelectViewAsset;
        [SerializeField] private Transform m_SpawnSpot;
        [SerializeField] private List<CardSelectView> m_SpawnedCardSelectionViews = new();
        [SerializeField] private CardDetailView m_CardDetailView;

        public CardDeckSlotBarView CardDeckSlotBarView => m_DeckSlotBarView;

        // ── Init ──────────────────────────────────────────────────────────────
        /// <summary>
        /// Dipanggil sekali saat CardDeck.OnInitializedInvoke().
        /// Init slot bar dulu, baru spawn card list.
        /// </summary>
        public void InitSlotBar()
        {
            if (m_DeckSlotBarView != null)
                m_DeckSlotBarView.Init();
        }

        // ── Spawn card select item ─────────────────────────────────────────────
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
                return;
            }

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
        public void ShowAllCard()
        {
            ShowAllCardInternal();
        }

        public void HideAllCard()
        {
            HideAllCardInternal();
        }
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
            ShowRarityInternal(rarityConfig);
        }

        private void ShowRarityInternal(RarityConfig rarityConfig)
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