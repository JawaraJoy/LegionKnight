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
        [SerializeField]
        private UnityEvent m_OnDoubledStart;
        [SerializeField]
        private UnityEvent<ScriptableObject> m_OnLootDefiUpdate;

        public List<LootItemView> SpawnedLoots => m_SpawnedLoots;

        public LootItemView GetLootItemView(LootField definition)
        {
            return GetLootViewInternal(definition);
        }
        private LootItemView GetLootViewInternal(LootField definition)
        {
            foreach (LootItemView loot in m_SpawnedLoots)
            {
                if (loot.Definition is LootField lootField)
                {
                    if (lootField.Item is IDescriptable descriptable && definition.Item is IDescriptable descriptable2)
                    {
                        if (descriptable.Id == descriptable2.Id)
                        {
                            return loot;
                        }
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
                bool alreadySpawned = GetLootViewInternal(loots[i]) != null;
                yield return new WaitUntil(() => alreadySpawned);
            }
        }
        private IEnumerator AddingLootView(LootField loot)
        {
            Debug.Log($"Adding loot view: {loot.Item.name} x{loot.Amount}");
            
            bool has = GetLootViewInternal(loot) != null;
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
        public void DoubledCountDownLootAmount()
        {
            m_OnDoubledStart?.Invoke();
            Debug.Log($"Doubling loot amounts for all spawned loots");
            foreach (var lootView in m_SpawnedLoots)
            {
                if (lootView.Definition is LootField lootField)
                {
                    int add = lootField.Amount;
                    lootView.AddAmountWithCountDown(add);
                    Debug.Log($"Counting up loot view: {lootField.Item.name} by {add}");
                }
                else
                {
                    Debug.LogWarning($"Loot view definition is not LootField");
                }
            }
        }
        public virtual void AddLootView(LootField loot)
        {
            StartCoroutine(AddingLootView(loot));
        }
        public virtual void RemoveLootView(LootField loot)
        {
            LootItemView view = GetLootViewInternal(loot);
            if (view != null)
            {
                view.Hide();
                m_SpawnedLoots.Remove(view);
                Addressables.ReleaseInstance(view.gameObject);
                m_OnLootUdate?.Invoke(loot);
                Debug.Log($"Removed loot view: {loot.Item.name}");
            }
        }

        private void UpdateLootAmountView(LootField loot)
        {
            LootItemView view = GetLootViewInternal(loot);
            if (view != null)
            {
                view.SetAmount(loot.Amount);
                m_OnLootUdate?.Invoke(loot);
                
                Debug.Log($"Updated loot view: {loot.Item.name} x{loot.Amount}");
            }
        }

        private void OnLootUpdate(ScriptableObject defi)
        {
            m_OnLootDefiUpdate?.Invoke(defi);
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
