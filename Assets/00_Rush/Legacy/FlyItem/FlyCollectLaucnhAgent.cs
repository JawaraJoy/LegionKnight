using Rush;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace LegionKnight
{
    public class FlyCollectLaunchAgent : MonoBehaviour
    {
        [SerializeField] private AssetReferenceGameObject m_FlyItemPrefab;
        [SerializeField] private PadDefinition m_TargetPad;

        [Header("Pool Settings")]
        [SerializeField] private int m_PrewarmCount = 2;

        [Header("Spawn Settings")]
        [SerializeField] private int m_DefaultSpawnCount = 1;

        [Header("Spread Settings")]
        [SerializeField] private SpreadType m_SpreadType = SpreadType.Circle;
        [SerializeField] private float m_SpreadRadius = 1f;
        [SerializeField] private Vector2 m_SpreadBoxSize = new Vector2(2f, 2f);

        private readonly List<FlyItem> m_Pool = new List<FlyItem>();

        public enum SpreadType { None, Circle, Random, Grid, Arc }

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

        // Hitung offset posisi berdasarkan tipe spread
        private Vector3 GetSpreadOffset(int index, int total)
        {
            if (total <= 1 || m_SpreadType == SpreadType.None)
                return Vector3.zero;

            switch (m_SpreadType)
            {
                case SpreadType.Circle:
                    {
                        float angle = (360f / total) * index * Mathf.Deg2Rad;
                        return new Vector3(
                            Mathf.Cos(angle) * m_SpreadRadius,
                            0f,
                            Mathf.Sin(angle) * m_SpreadRadius
                        );
                    }

                case SpreadType.Random:
                    {
                        return new Vector3(
                            Random.Range(-m_SpreadBoxSize.x, m_SpreadBoxSize.x),
                            0f,
                            Random.Range(-m_SpreadBoxSize.y, m_SpreadBoxSize.y)
                        );
                    }

                case SpreadType.Grid:
                    {
                        int cols = Mathf.CeilToInt(Mathf.Sqrt(total));
                        int row = index / cols;
                        int col = index % cols;
                        float spacing = m_SpreadRadius;
                        float offsetX = col * spacing - (cols - 1) * spacing * 0.5f;
                        float offsetZ = row * spacing - (Mathf.CeilToInt((float)total / cols) - 1) * spacing * 0.5f;
                        return new Vector3(offsetX, 0f, offsetZ);
                    }

                case SpreadType.Arc:
                    {
                        float halfArc = 90f;
                        float angle = (-halfArc + (halfArc * 2f / Mathf.Max(1, total - 1)) * index) * Mathf.Deg2Rad;
                        return new Vector3(
                            Mathf.Cos(angle) * m_SpreadRadius,
                            0f,
                            Mathf.Sin(angle) * m_SpreadRadius
                        );
                    }

                default:
                    return Vector3.zero;
            }
        }

        private IEnumerator CreateNewItem(bool active, CollectibleConfig config = null, Vector3 spawnPos = default)
        {
            var handle = m_FlyItemPrefab.InstantiateAsync(spawnPos, Quaternion.identity);
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
                        item.transform.position = spawnPos;
                        item.Init(config, m_TargetPad);
                    }
                }
            }
        }

        private FlyItem GetAvailableItem()
        {
            foreach (var item in m_Pool)
            {
                if (!item.IsActive) return item;
            }
            return null;
        }

        // Overload tanpa parameter — pakai default count dari Inspector
        public void SpawnFlyItem(CollectibleConfig config)
        {
            SpawnFlyItem(config, m_DefaultSpawnCount);
        }

        // Overload dengan jumlah custom
        public void SpawnFlyItem(CollectibleConfig config, int count)
        {
            count = Mathf.Max(1, count);
            for (int i = 0; i < count; i++)
            {
                Vector3 offset = GetSpreadOffset(i, count);
                Vector3 spawnPos = transform.position + offset;
                RushGameManager.Instance.StartCoroutine(SpawnRoutine(config, spawnPos));
            }
        }

        private IEnumerator SpawnRoutine(CollectibleConfig config, Vector3 spawnPos)
        {
            FlyItem item = GetAvailableItem();

            if (item != null)
            {
                item.gameObject.SetActive(true);
                item.transform.position = spawnPos;
                item.Init(config, m_TargetPad);
            }
            else
            {
                yield return CreateNewItem(true, config, spawnPos);
            }
        }
    }
}