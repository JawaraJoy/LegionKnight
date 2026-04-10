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
        [SerializeField] private AssetReferenceGameObject m_FlyItemPrefab;
        [SerializeField] private PadDefinition m_TargetPad;
        private const int m_PrewarmCount = 2;

        private readonly List<FlyItem> m_Pool = new List<FlyItem>();

        private void Start()
        {
            RushGameManager.Instance.StartCoroutine(Prewarm());
        }

        private IEnumerator Prewarm()
        {
            for (int i = 0; i < m_PrewarmCount; i++)
            {
                yield return CreateNewItem(false);
            }
        }

        private IEnumerator CreateNewItem(bool active, CollectibleConfig config = null)
        {
            var handle = m_FlyItemPrefab.InstantiateAsync(transform.position, Quaternion.identity);
            yield return handle;

            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                GameObject obj = handle.Result;

                if (obj.TryGetComponent(out FlyItem item))
                {
                    item.gameObject.SetActive(active);
                    m_Pool.Add(item);

                    if (active && config != null)
                    {
                        item.transform.position = transform.position;
                        item.Init(config, m_TargetPad);
                    }
                }
            }
        }

        private FlyItem GetAvailableItem()
        {
            foreach (var item in m_Pool)
            {
                if (!item.IsActive)
                {
                    return item;
                }
            }
            return null;
        }

        public void SpawnFlyItem(CollectibleConfig config)
        {
            RushGameManager.Instance.StartCoroutine(SpawnRoutine(config));
        }

        private IEnumerator SpawnRoutine(CollectibleConfig config)
        {
            FlyItem item = GetAvailableItem();

            if (item != null)
            {
                item.gameObject.SetActive(true);
                item.transform.position = transform.position;
                item.Init(config, m_TargetPad);
            }
            else
            {
                yield return CreateNewItem(true, config);
            }
        }
    }
}