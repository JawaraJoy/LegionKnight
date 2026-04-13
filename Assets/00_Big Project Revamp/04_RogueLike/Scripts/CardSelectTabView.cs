using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Rush;

namespace LegionKnight
{
    public class CardSelectTabView : UIView
    {
        [SerializeField]
        private AssetReferenceGameObject m_CardSelectViewAsset;

        [SerializeField]
        private Transform m_SpawnSpot;

        [SerializeField]
        private List<CardSelectView> m_SpawnedCardSelectionViews = new();

        [SerializeField]
        private CardDetailView m_CardDetailView;

        private void Awake()
        {
            Player.Instance.PlayerCardDeck.OnInitializedUnit.AddListener(SpawnCardSelectInternal);
        }

        private CardSelectView GetSelectView(CardConfig cardConfig)
        {
            CardSelectView view = m_SpawnedCardSelectionViews.Find(x => x.CardConfig == cardConfig);
            return view;
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
        public void SpawnCardSelect(CardUnit unit)
        {
            SpawnCardSelectInternal(unit);
        }
        private void SpawnCardSelectInternal(CardUnit unit)
        {
            if (GetSelectView(unit.CardConfig) != null)
            {
                GetSelectView(unit.CardConfig).Init(unit);
            }
            else
            {
                AsyncOperationHandle<GameObject> handle = m_CardSelectViewAsset.InstantiateAsync(m_SpawnSpot, false);
                RushGameManager.Instance.StartCoroutine(SpawningCardSelectView(handle, unit));
            }
            
        }
        public void ShowRarity(RarityConfig rarityConfig)
        {
            ShowRarityInternal(rarityConfig);
        }
        protected override void ShowInternal()
        {
            base.ShowInternal();
            m_CardDetailView.Hide();
        }

        public void ShowAll()
        {
            foreach (CardSelectView cardSelectView in m_SpawnedCardSelectionViews)
            {
                cardSelectView.Show();
            }
        }
        public void HideAll()
        {
            foreach (CardSelectView cardSelectView in m_SpawnedCardSelectionViews)
            {
                cardSelectView.Hide();
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
