using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Rush;

namespace LegionKnight
{
    public class PlatformSelectionView : UIView
    {
        [SerializeField]
        private AssetReferenceGameObject m_PlatformSelectViewAsset;

        [SerializeField]
        private Transform m_SpawnSpot;

        [SerializeField]
        private List<PlatformSelectView> m_SpawnedPlatformSelectionViews = new();

        public void Init()
        {
            InitInternal();
        }

        private PlatformSelectView GetSelectView(PlatformConfig platformConfig)
        {
            PlatformSelectView view = m_SpawnedPlatformSelectionViews.Find(x => x.PlatformConfig == platformConfig);
            return view;
        }
        private void InitInternal()
        {
            PlatformUnit[] units = Player.Instance.PlatformDeck.GetPlatformUnits();
            foreach (PlatformUnit unit in units)
            {
                if (GetSelectView(unit.PlatformConfig) != null)
                {
                    GetSelectView(unit.PlatformConfig).Init(unit);
                }
                else
                {
                    SpawnPlatformSelectInternal(unit);
                }   
            }
        }
        private IEnumerator SpawningPlatformSelectView(AsyncOperationHandle<GameObject> handle, PlatformUnit unit)
        {
            yield return handle;
            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                GameObject spawned = handle.Result;
                if (spawned.TryGetComponent(out PlatformSelectView view))
                {
                    view.Init(unit);
                    if (m_SpawnedPlatformSelectionViews.Contains(view)) yield break;
                    m_SpawnedPlatformSelectionViews.Add(view);
                }

            }
        }

        private void SpawnPlatformSelectInternal(PlatformUnit unit)
        {
            AsyncOperationHandle<GameObject> handle = m_PlatformSelectViewAsset.InstantiateAsync(m_SpawnSpot, false);
            StartCoroutine(SpawningPlatformSelectView(handle, unit));
        }
        public void SpawnPlatformSelect(PlatformUnit unit)
        {
            SpawnPlatformSelectInternal(unit);
        }
        public void ShowRarity(RarityConfig rarityConfig)
        {
            ShowRarityInternal(rarityConfig);
        }

        public void ShowAllPlatforms()
        {
            foreach (PlatformSelectView platformSelectView in m_SpawnedPlatformSelectionViews)
            {
                platformSelectView.Show();
            }
        }
        public void HideAllPlatforms()
        {
            foreach (PlatformSelectView platformSelectView in m_SpawnedPlatformSelectionViews)
            {
                platformSelectView.Hide();
            }
        }

        public void RefreshEquiped()
        {
            foreach (PlatformSelectView platformSelectView in m_SpawnedPlatformSelectionViews)
            {
                platformSelectView.RefreshEquiped();
            }
        }

        private void ShowRarityInternal(RarityConfig rarityConfig)
        {
            foreach (PlatformSelectView platformSelectView in m_SpawnedPlatformSelectionViews)
            {
                platformSelectView.Hide();
            }
            PlatformSelectView[] views = GetPlatformSelectViews(rarityConfig);
            foreach (PlatformSelectView characterSelectView in views)
            {
                characterSelectView.Show();
            }
        }
        private PlatformSelectView[] GetPlatformSelectViews(RarityConfig rarityConfig)
        {
            return m_SpawnedPlatformSelectionViews.FindAll(x => x.PlatformConfig.CollectibleField.RarityConfig.BaseInfo.Id == rarityConfig.BaseInfo.Id).ToArray();
        }
    }
}
