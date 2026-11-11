using System;
using System.Collections;
using System.Collections.Generic;
using LegionKnight;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Rush
{
    internal sealed class InboxRewardItems
    {
        private readonly List<LootItemView> m_Cells = new();
        private LootField[] m_Rewards = Array.Empty<LootField>();
        private bool m_IsLoading;

        internal void SetItems(LootField[] rewards, Transform container, LootItemView template, AssetReferenceGameObject asset)
        {
            m_Rewards = rewards;
            if (template != null)
            {
                while (m_Cells.Count < rewards.Length)
                {
                    m_Cells.Add(UnityEngine.Object.Instantiate(template, container, false));
                }
            }
            for (int i = 0; i < m_Cells.Count; i++) BindCellInternal(i);

            if (m_Cells.Count < rewards.Length && !m_IsLoading)
            {
                RushGameManager.Instance.StartCoroutine(LoadCellsInternal(container, asset));
            }
        }

        private void BindCellInternal(int index)
        {
            var cell = m_Cells[index];
            bool visible = index < m_Rewards.Length;
            cell.gameObject.SetActive(visible);
            if (visible) cell.Init(m_Rewards[index]);
        }

        private IEnumerator LoadCellsInternal(Transform container, AssetReferenceGameObject asset)
        {
            m_IsLoading = true;
            while (container != null && m_Cells.Count < m_Rewards.Length)
            {
                var handle = asset.InstantiateAsync(container);
                yield return handle;
                if (handle.Status != AsyncOperationStatus.Succeeded)
                {
                    Addressables.Release(handle);
                    break;
                }
                if (container == null || !handle.Result.TryGetComponent(out LootItemView cell))
                {
                    Addressables.ReleaseInstance(handle);
                    break;
                }
                m_Cells.Add(cell);
                BindCellInternal(m_Cells.Count - 1);
            }
            m_IsLoading = false;
        }
    }
}
