using System.Collections;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace LegionKnight
{
    [System.Serializable]
    public class MinionUnit
    {
        [SerializeField]
        private MinionDefinition m_Definition;
        [SerializeField]
        private bool m_CanSpawn;
        [SerializeField]
        private LevelDefinition m_LevelAllowed;

        public MinionDefinition Definition => m_Definition;
        public bool CanSpawn => m_CanSpawn;
        public LevelDefinition LevelAllowed => m_LevelAllowed;

        public void SetCanSpawn(bool canSpawn)
        {
            m_CanSpawn = canSpawn;
        }
        public void SpawnMinion(Transform spot, Vector2 offsite)
        {
            bool levelSync = GameManager.Instance.LevelDefinition == m_LevelAllowed;
            if (!CanSpawn || !levelSync) return;
            AssetReferenceGameObject asset = m_Definition.ModelPrefab;
            AsyncOperationHandle<GameObject> handle = asset.InstantiateAsync(offsite, Quaternion.identity);
            GameManager.Instance.StartCoroutine(SpawningMinion(spot, handle));
        }
        public void CustomSpawnMinion(Transform spot, Vector2 offsite)
        {
            AssetReferenceGameObject asset = m_Definition.ModelPrefab;
            AsyncOperationHandle<GameObject> handle = asset.InstantiateAsync(offsite, Quaternion.identity);
            GameManager.Instance.StartCoroutine(SpawningMinion(spot, handle));
        }
        private IEnumerator SpawningMinion(Transform spot, AsyncOperationHandle<GameObject> handle)
        {
            yield return handle;
            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                GameObject spawned = handle.Result;
                if (spawned.TryGetComponent(out Minion minion))
                {
                    minion.Init(m_Definition);
                    spot.DetachChildren();

                   // SetPositionRandomRadius(spot, spawned, offsite);
                }
            }
        }
        private void SetPositionRandomRadius(Transform spot, GameObject minion, Vector2 offsite)
        {
            float startX = spot.position.x;
            float startY = spot.position.y;
            float AddX = offsite.x + startX;
            float AddY = offsite.y + startY;

            float randomX = Random.Range(startX, AddX);
            float randomY = Random.Range(startY, AddY);
            Vector2 randomRadiues = new(randomX, randomY);

            minion.transform.position = randomRadiues;
        }
    }
}
