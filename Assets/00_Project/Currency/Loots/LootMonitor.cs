using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using static Spine.Unity.Examples.SpineboyFootplanter;

namespace LegionKnight
{
    public class LootMonitor : UIView
    {
        [SerializeField]
        private AssetReferenceGameObject m_LootViewAsset;
        [SerializeField]
        private Transform m_LootViewSpawn;

        private readonly List<LootItemView> m_SpawnedLoots = new();

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
        public void SpawnLootView(LootField loot)
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
        public void ClearAllLoots()
        {
            foreach (var loot in m_SpawnedLoots)
            {
                if (loot != null)
                {
                    Addressables.Release(loot.gameObject);
                }
            }
            m_SpawnedLoots.Clear();
        }
    }
}
