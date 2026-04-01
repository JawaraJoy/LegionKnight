using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Rush;

namespace LegionKnight
{
    public class CardSelectionView : UIView
    {
        [SerializeField]
        private AssetReferenceGameObject m_CardSelectViewAsset;

        [SerializeField]
        private Transform m_SpawnSpot;

        [SerializeField]
        private List<CardSelectView> m_SpawnedCardSelectionViews = new();

        public void Init()
        {
            InitInternal();
        }

        private CardSelectView GetSelectView(CardConfig cardConfig)
        {
            CardSelectView view = m_SpawnedCardSelectionViews.Find(x => x.CardConfig == cardConfig);
            return view;
        }
        private void InitInternal()
        {
            CardUnit[] units = Player.Instance.PlayerCardDeck.GetCardUnits();
            foreach (CardUnit unit in units)
            {
                if (GetSelectView(unit.CardConfig) != null)
                {
                    GetSelectView(unit.CardConfig).Init(unit);
                }
                else
                {
                    SpawnCardSelectInternal(unit);
                }   
            }
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
                    if (m_SpawnedCardSelectionViews.Contains(view)) yield break;
                    m_SpawnedCardSelectionViews.Add(view);
                }

            }
        }

        private void SpawnCardSelectInternal(CardUnit unit)
        {
            AsyncOperationHandle<GameObject> handle = m_CardSelectViewAsset.InstantiateAsync(m_SpawnSpot, false);
            StartCoroutine(SpawningCardSelectView(handle, unit));
        }
        public void SpawnCardSelect(CardUnit unit)
        {
            SpawnCardSelectInternal(unit);
        }
        public void ShowRarity(RarityConfig rarityConfig)
        {
            ShowRarityInternal(rarityConfig);
        }

        public void ShowAllCards()
        {
            foreach (CardSelectView cardSelectView in m_SpawnedCardSelectionViews)
            {
                cardSelectView.Show();
            }
        }
        public void HideAllCards()
        {
            foreach (CardSelectView cardSelectView in m_SpawnedCardSelectionViews)
            {
                cardSelectView.Hide();
            }
        }

        public void RefreshEquiped()
        {
            foreach (CardSelectView cardSelectView in m_SpawnedCardSelectionViews)
            {
                cardSelectView.RefreshEquiped();
            }
        }

        private void ShowRarityInternal(RarityConfig rarityConfig)
        {
            foreach (CardSelectView cardSelectView in m_SpawnedCardSelectionViews)
            {
                cardSelectView.Hide();
            }
            CardSelectView[] views = GetCardSelectViews(rarityConfig);
            foreach (CardSelectView characterSelectView in views)
            {
                characterSelectView.Show();
            }
        }
        private CardSelectView[] GetCardSelectViews(RarityConfig rarityConfig)
        {
            return m_SpawnedCardSelectionViews.FindAll(x => x.CardConfig.CollectibleField.RarityConfig.BaseInfo.Id == rarityConfig.BaseInfo.Id).ToArray();
        }
    }
}
