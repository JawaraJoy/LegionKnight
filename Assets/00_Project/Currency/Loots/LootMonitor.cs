using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Events;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace LegionKnight
{
    public class LootMonitor : UIView
    {
        [SerializeField]
        private AssetReferenceGameObject m_LootViewAsset;
        [SerializeField]
        private Transform m_LootViewSpawn;

        private readonly List<LootItemView> m_SpawnedLoots = new();

        [SerializeField]
        private UnityEvent<LootField> m_OnLootUdate;

        private LootItemView GetLootView(ScriptableObject defi)
        {
            foreach (var loot in m_SpawnedLoots)
            {
                if (loot.Definition is ScriptableObject targetDefi)
                {
                    if (targetDefi == defi)
                    {
                        return loot;
                    }
                }
            }
            return null;
        }
        public void SpawnLootsView(List<LootField> loots)
        {
            SpawnLootsViewInternal(loots);
        }

        protected void SpawnLootsViewInternal(List<LootField> loots)
        {
            StartCoroutine(SpawningLootsView(loots));
        }
        private IEnumerator SpawningLootsView(List<LootField> loots)
        {
            for (int i = 0; i < loots.Count; i++)
            {
                yield return new WaitForSeconds(0.2f);
                yield return StartCoroutine(SpawningLootView(loots[i]));
            }
        }
        public virtual void SpawnLootView(LootField loot)
        {
            LootItemView view = GetLootView(loot.Item);
            if (view != null)
            {
                if(!loot.IsUnique)
                {
                    view.AddAmount(loot.Amount);
                }
                else
                {
                    StartCoroutine(SpawningLootView(loot));
                }
            }
            else
            {
                StartCoroutine(SpawningLootView(loot));
            }
            m_OnLootUdate?.Invoke(loot);
        }

        private IEnumerator SpawningLootView(LootField loot)
        {
            var handle = m_LootViewAsset.InstantiateAsync(m_LootViewSpawn);
            yield return handle;
            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                GameObject lootView = handle.Result;
                if (lootView.TryGetComponent(out LootItemView view))
                {
                    view.Init(loot);
                    m_SpawnedLoots.Add(view);
                }
            }
        }
        private void ClearAllLootViewsInternal()
        {
            foreach (var loot in m_SpawnedLoots)
            {
                if (loot != null)
                {
                    loot.Hide();
                    Addressables.ReleaseInstance(loot.gameObject);
                }
            }
            m_SpawnedLoots.Clear();
        }
        public void ClearAllLootViews()
        {
            
            ClearAllLootViewsInternal();
        }
    }
}
