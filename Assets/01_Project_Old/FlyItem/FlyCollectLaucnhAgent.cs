using Rush;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace LegionKnight
{
    public class FlyCollectLaucnhAgent : MonoBehaviour
    {
        [SerializeField]
        private AssetReferenceGameObject m_FlyItemPrefab;
        [SerializeField]
        private PadDefinition m_TargetPad;

        private readonly List<FlyItem> m_ActiveFlyItems = new List<FlyItem>();

        private void Register(FlyItem item)
        {
            if (!m_ActiveFlyItems.Contains(item))
            {
                m_ActiveFlyItems.Add(item);
            }
        }
        private void Unregister(FlyItem item)
        {
            if (m_ActiveFlyItems.Contains(item))
            {
                m_ActiveFlyItems.Remove(item);
            }
        }

        private bool AnyCountFlyItemExists()
        {
            bool anyCount = m_ActiveFlyItems.Count > 0;
            return anyCount;
        }
        private bool AnyActiveFlyItemExists()
        {
            // Check if any FlyItem in the list is active in hierarchy
            return m_ActiveFlyItems.Exists(item => item.gameObject.activeInHierarchy);
        }

        private FlyItem GetInactiveFlyItem()
        {
            foreach (var item in m_ActiveFlyItems)
            {
                if (!item.gameObject.activeInHierarchy)
                {
                    return item;
                }
            }
            return null;
        }
        public void SpawnFlyItem(CollectibleConfig objek)
        {
            RushGameManager.Instance.StartCoroutine(SpawningFlyItem(objek, m_TargetPad));
        }
        private IEnumerator SpawningFlyItem(CollectibleConfig objek, PadDefinition targetPad)
        {
            // Reuse inactive FlyItem if available
            if (AnyCountFlyItemExists() && !AnyActiveFlyItemExists())
            {
                FlyItem item = GetInactiveFlyItem();
                item.gameObject.SetActive(true);
                item.transform.position = transform.position;
                item.Init(objek, targetPad);
            }
            else
            {
                var handle = m_FlyItemPrefab.InstantiateAsync(transform.position, Quaternion.identity);
                yield return handle;
                if (handle.Status == AsyncOperationStatus.Succeeded)
                {
                    GameObject flyItemObj = handle.Result;
                    if (flyItemObj.TryGetComponent<FlyItem>(out var existingFlyItem))
                    {
                        Register(existingFlyItem);
                        existingFlyItem.Init(objek, targetPad);
                    }
                }
            }
                
        }
    }
}
