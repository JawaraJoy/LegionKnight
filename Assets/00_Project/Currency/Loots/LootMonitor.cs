using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Events;
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

        [SerializeField]
        private List<LootItemView> m_SpawnedLoots = new();

        [SerializeField]
        private UnityEvent<LootField> m_OnLootUdate;

        private LootItemView GetLootView(LootField definition)
        {
            foreach (LootItemView loot in m_SpawnedLoots)
            {
                if (loot.Definition is LootField lootField)
                {
                    if (lootField.Item == definition.Item)
                    {
                        return loot;
                    }
                }
            }
            return null;
        }
        public void AddLootsView(List<LootField> loots)
        {
            AddLootsViewInternal(loots);
        }

        protected void AddLootsViewInternal(List<LootField> loots)
        {
            StartCoroutine(AddingLootsView(loots));
        }
        private IEnumerator AddingLootsView(List<LootField> loots)
        {
            yield return new WaitForEndOfFrame();
            yield return new WaitUntil(() => m_IsShow);
            for (int i = 0; i < loots.Count; i++)
            {   
                Debug.Log($"Loot {i}: {loots[i].Item.name}, IsUnique: {loots[i].IsUnique}");
                yield return StartCoroutine(AddingLootView(loots[i]));
                bool alreadySpawned = GetLootView(loots[i]) != null;
                yield return new WaitUntil(() => alreadySpawned);
            }
        }
        private IEnumerator AddingLootView(LootField loot)
        {
            Debug.Log($"Adding loot view: {loot.Item.name} x{loot.Amount}");
            
            bool has = GetLootView(loot) != null;
            bool unique = loot.IsUnique;
            Debug.Log($"Has loot view: {has}");
            if (has)
            {
                if (!unique)
                {
                    UpdateLootAmountView(loot);
                }
                else
                {
                    yield return StartCoroutine(SpawningLootView(loot));
                }
            }
            else
            {
                // spawn new loot view
                yield return StartCoroutine(SpawningLootView(loot));
            }
            yield return new WaitForEndOfFrame();
        }
        public virtual void AddLootView(LootField loot)
        {
            StartCoroutine(AddingLootView(loot));
        }

        private void UpdateLootAmountView(LootField loot)
        {
            LootItemView view = GetLootView(loot);
            if (view != null)
            {
                view.SetAmount(loot.Amount);
                m_OnLootUdate?.Invoke(loot);
                Debug.Log($"Updated loot view: {loot.Item.name} x{loot.Amount}");
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
                    m_OnLootUdate?.Invoke(loot);
                }
            }
            yield return new WaitForEndOfFrame();
        }
        private void ClearAllLootViewsInternal()
        {
            if (m_SpawnedLoots.Count < 1)
            {
                return;
            }
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
