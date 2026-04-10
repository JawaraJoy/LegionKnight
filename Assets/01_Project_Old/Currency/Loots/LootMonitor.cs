using MoreMountains.Tools;
using Rush;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Events;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;

namespace LegionKnight
{
    public class LootMonitor : UIView
    {
        [Header("Source")]
        [SerializeField, MMReadOnly]
        private LootStorageManager m_LootStorageManager;

        [Header("View")]
        [SerializeField]
        private AssetReferenceGameObject m_LootViewAsset;
        [SerializeField]
        private Transform m_LootViewSpawn;
        [SerializeField]
        private Button m_DoubleLootButton;

        [Header("Runtime")]
        [SerializeField]
        private List<LootItemView> m_SpawnedLoots = new();

        [Header("Events")]
        [SerializeField]
        private UnityEvent<LootField> m_OnLootUdate;
        [SerializeField]
        private UnityEvent m_OnDoubledStart;
        [SerializeField]
        private UnityEvent<CollectibleConfig> m_OnLootDefiUpdate;

        public List<LootItemView> SpawnedLoots => m_SpawnedLoots;

        protected virtual void Awake()
        {
            SetupStorageReferenceInternal();

            m_DoubleLootButton.onClick.RemoveListener(ShowAdToDoubleLoot);
            m_DoubleLootButton.onClick.AddListener(ShowAdToDoubleLoot);
        }
        protected override void OnShowInvoke()
        {
            base.OnShowInvoke();
            m_DoubleLootButton.gameObject.SetActive(true);
        }
        protected override void OnHideInvoke()
        {
            base.OnHideInvoke();
            TakeLoots();
        }
        protected virtual void OnEnable()
        {
            BindStorageEventsInternal();
            RefreshFromStorageInternal();
        }

        protected virtual void OnDisable()
        {
            UnbindStorageEventsInternal();
        }

        public void SetLootStorageManager(LootStorageManager storage)
        {
            SetLootStorageManagerInternal(storage);
        }
        private void ShowAdToDoubleLoot()
        {
            UnityService.Instance.ShowRewardedAd(DoubleLoot);
        }
        protected virtual void DoubleLoot()
        {
            m_LootStorageManager.StartDoubleStoredLoots();
            m_DoubleLootButton.gameObject.SetActive(false);
        }
        protected virtual void TakeLoots()
        {
            m_LootStorageManager.TakeLooteds();
        }
        protected virtual void SetLootStorageManagerInternal(LootStorageManager storage)
        {
            if (m_LootStorageManager == storage)
            {
                return;
            }

            UnbindStorageEventsInternal();
            m_LootStorageManager = storage;
            BindStorageEventsInternal();
            RefreshFromStorageInternal();
        }

        public LootItemView GetLootItemView(LootField loot)
        {
            return GetLootItemViewInternal(loot);
        }

        protected virtual LootItemView GetLootItemViewInternal(LootField loot)
        {
            if (loot == null || loot.ItemLoot == null)
            {
                return null;
            }

            return GetLootViewInternal(loot.ItemLoot);
        }

        public LootItemView GetLootItemView(CollectibleConfig config)
        {
            return GetLootItemViewInternal(config);
        }

        protected virtual LootItemView GetLootItemViewInternal(CollectibleConfig config)
        {
            return GetLootViewInternal(config);
        }

        private LootItemView GetLootViewInternal(CollectibleConfig config)
        {
            if (config == null)
            {
                return null;
            }

            for (int i = 0; i < m_SpawnedLoots.Count; i++)
            {
                LootItemView lootView = m_SpawnedLoots[i];

                if (lootView == null || lootView.LootField == null || lootView.LootField.ItemLoot == null)
                {
                    continue;
                }

                if (lootView.LootField.ItemLoot.BaseInfo.Id == config.BaseInfo.Id)
                {
                    return lootView;
                }
            }

            return null;
        }

        public void RefreshFromStorage()
        {
            RefreshFromStorageInternal();
        }

        protected virtual void RefreshFromStorageInternal()
        {
            if (m_LootStorageManager == null)
            {
                return;
            }

            RushGameManager.Instance.StartCoroutine(RefreshingFromStorage());
        }

        private IEnumerator RefreshingFromStorage()
        {
            yield return new WaitForEndOfFrame();
            yield return new WaitUntil(() => IsShowInternal);

            IReadOnlyList<LootField> storageLoots = m_LootStorageManager.Looteds;
            HashSet<string> storageIds = new();

            for (int i = 0; i < storageLoots.Count; i++)
            {
                LootField loot = storageLoots[i];
                if (loot == null || loot.ItemLoot == null)
                {
                    continue;
                }

                string id = loot.ItemLoot.BaseInfo.Id;
                storageIds.Add(id);

                LootItemView existingView = GetLootViewInternal(loot.ItemLoot);
                if (existingView == null)
                {
                    yield return RushGameManager.Instance.StartCoroutine(SpawningLootView(loot));
                }
                else
                {
                    existingView.Bind(loot);
                    existingView.SetAmountAnimated(loot.Amount);
                    m_OnLootUdate?.Invoke(loot);
                    OnLootUpdateInternal(loot.ItemLoot);
                }
            }

            for (int i = m_SpawnedLoots.Count - 1; i >= 0; i--)
            {
                LootItemView spawnedView = m_SpawnedLoots[i];

                if (spawnedView == null)
                {
                    m_SpawnedLoots.RemoveAt(i);
                    continue;
                }

                if (spawnedView.LootField == null || spawnedView.LootField.ItemLoot == null)
                {
                    spawnedView.Hide();
                    Addressables.ReleaseInstance(spawnedView.gameObject);
                    m_SpawnedLoots.RemoveAt(i);
                    continue;
                }

                string spawnedId = spawnedView.LootField.ItemLoot.BaseInfo.Id;
                if (!storageIds.Contains(spawnedId))
                {
                    RemoveLootViewInternal(spawnedView.LootField);
                }
            }
        }

        public void AddLootsView(List<LootField> loots)
        {
            AddLootsViewInternal(loots);
        }

        protected virtual void AddLootsViewInternal(List<LootField> loots)
        {
            if (loots == null || loots.Count < 1)
            {
                return;
            }

            RushGameManager.Instance.StartCoroutine(AddingLootsView(loots));
        }

        private IEnumerator AddingLootsView(List<LootField> loots)
        {
            yield return new WaitForEndOfFrame();
            yield return new WaitUntil(() => IsShowInternal);

            for (int i = 0; i < loots.Count; i++)
            {
                if (loots[i] == null || loots[i].ItemLoot == null)
                {
                    continue;
                }

                yield return RushGameManager.Instance.StartCoroutine(AddingLootView(loots[i]));
                yield return new WaitUntil(() => GetLootViewInternal(loots[i].ItemLoot) != null);
            }
        }

        public virtual void AddLootView(LootField loot)
        {
            AddLootViewInternal(loot);
        }

        protected virtual void AddLootViewInternal(LootField loot)
        {
            if (loot == null || loot.ItemLoot == null)
            {
                return;
            }

            RushGameManager.Instance.StartCoroutine(AddingLootView(loot));
        }

        private IEnumerator AddingLootView(LootField loot)
        {
            yield return new WaitForEndOfFrame();
            yield return new WaitUntil(() => IsShowInternal);

            LootItemView existingView = GetLootViewInternal(loot.ItemLoot);
            if (existingView != null)
            {
                existingView.Bind(loot);
                existingView.SetAmountAnimated(loot.Amount);
                m_OnLootUdate?.Invoke(loot);
                OnLootUpdateInternal(loot.ItemLoot);
                yield break;
            }

            yield return RushGameManager.Instance.StartCoroutine(SpawningLootView(loot));

            LootItemView spawnedView = GetLootViewInternal(loot.ItemLoot);
            if (spawnedView != null)
            {
                spawnedView.Bind(loot);
                spawnedView.SetAmountImmediate(loot.Amount);
                m_OnLootUdate?.Invoke(loot);
                OnLootUpdateInternal(loot.ItemLoot);
            }
        }

        public virtual void RemoveLootView(LootField loot)
        {
            RemoveLootViewInternal(loot);
        }

        protected virtual void RemoveLootViewInternal(LootField loot)
        {
            LootItemView view = GetLootItemViewInternal(loot);
            if (view == null)
            {
                return;
            }

            view.Hide();
            m_SpawnedLoots.Remove(view);
            Addressables.ReleaseInstance(view.gameObject);
            m_OnLootUdate?.Invoke(loot);
        }

        public void UpdateLootAmountView(LootField loot)
        {
            UpdateLootAmountViewInternal(loot);
        }

        protected virtual void UpdateLootAmountViewInternal(LootField loot)
        {
            if (loot == null || loot.ItemLoot == null)
            {
                return;
            }

            LootItemView view = GetLootItemViewInternal(loot);
            if (view == null)
            {
                AddLootViewInternal(loot);
                return;
            }

            view.Bind(loot);
            view.SetAmountAnimated(loot.Amount);
            m_OnLootUdate?.Invoke(loot);
            OnLootUpdateInternal(loot.ItemLoot);
        }

        public void ClearAllLootViews()
        {
            ClearAllLootViewsInternal();
        }

        private void ClearAllLootViewsInternal()
        {
            if (m_SpawnedLoots.Count < 1)
            {
                return;
            }

            for (int i = 0; i < m_SpawnedLoots.Count; i++)
            {
                LootItemView loot = m_SpawnedLoots[i];
                if (loot == null)
                {
                    continue;
                }

                loot.Hide();
                Addressables.ReleaseInstance(loot.gameObject);
            }

            m_SpawnedLoots.Clear();
        }

        private IEnumerator SpawningLootView(LootField loot)
        {
            var handle = m_LootViewAsset.InstantiateAsync(m_LootViewSpawn);
            yield return handle;

            if (handle.Status != AsyncOperationStatus.Succeeded)
            {
                yield break;
            }

            GameObject lootViewObj = handle.Result;
            if (!lootViewObj.TryGetComponent(out LootItemView view))
            {
                Addressables.ReleaseInstance(lootViewObj);
                yield break;
            }

            view.Init(loot);
            m_SpawnedLoots.Add(view);
            m_OnLootUdate?.Invoke(loot);
        }

        private void SetupStorageReferenceInternal()
        {
            if (m_LootStorageManager != null)
            {
                return;
            }

            if (GameManager.Instance == null)
            {
                return;
            }

            m_LootStorageManager = GameManager.Instance.LootStorageManager;
        }

        private void BindStorageEventsInternal()
        {
            if (m_LootStorageManager == null)
            {
                return;
            }

            m_LootStorageManager.OnAddNewLootEvent.RemoveListener(OnStorageAddLootInternal);
            m_LootStorageManager.OnLootAmountUpdateEvent.RemoveListener(OnStorageLootAmountUpdatedInternal);
            m_LootStorageManager.OnRemoveLootEvent.RemoveListener(OnStorageRemoveLootInternal);
            m_LootStorageManager.OnLootedsChangedEvent.RemoveListener(OnStorageLootsChangedInternal);
            m_LootStorageManager.OnDoubleLootStartedEvent.RemoveListener(OnStorageDoubleStartedInternal);

            m_LootStorageManager.OnAddNewLootEvent.AddListener(OnStorageAddLootInternal);
            m_LootStorageManager.OnLootAmountUpdateEvent.AddListener(OnStorageLootAmountUpdatedInternal);
            m_LootStorageManager.OnRemoveLootEvent.AddListener(OnStorageRemoveLootInternal);
            m_LootStorageManager.OnLootedsChangedEvent.AddListener(OnStorageLootsChangedInternal);
            m_LootStorageManager.OnDoubleLootStartedEvent.AddListener(OnStorageDoubleStartedInternal);
        }

        private void UnbindStorageEventsInternal()
        {
            if (m_LootStorageManager == null)
            {
                return;
            }

            m_LootStorageManager.OnAddNewLootEvent.RemoveListener(OnStorageAddLootInternal);
            m_LootStorageManager.OnLootAmountUpdateEvent.RemoveListener(OnStorageLootAmountUpdatedInternal);
            m_LootStorageManager.OnRemoveLootEvent.RemoveListener(OnStorageRemoveLootInternal);
            m_LootStorageManager.OnLootedsChangedEvent.RemoveListener(OnStorageLootsChangedInternal);
            m_LootStorageManager.OnDoubleLootStartedEvent.RemoveListener(OnStorageDoubleStartedInternal);
        }

        private void OnStorageAddLootInternal(LootField loot)
        {
            AddLootViewInternal(loot);
        }

        private void OnStorageLootAmountUpdatedInternal(LootField loot)
        {
            UpdateLootAmountViewInternal(loot);
        }

        private void OnStorageRemoveLootInternal(LootField loot)
        {
            RemoveLootViewInternal(loot);
        }

        private void OnStorageLootsChangedInternal(List<LootField> loots)
        {
            RefreshFromStorageInternal();
        }

        private void OnStorageDoubleStartedInternal()
        {
            m_OnDoubledStart?.Invoke();
        }

        private void OnLootUpdateInternal(CollectibleConfig config)
        {
            m_OnLootDefiUpdate?.Invoke(config);
        }
    }
}