using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace LegionKnight
{
    
    public partial class StackingPlatforms : MonoBehaviour
    {
        [SerializeField]
        private LevelDefinition m_LevelDefinition;
        [SerializeField]
        private Transform m_LeftPost;
        [SerializeField]
        private Transform m_RightPost;
        [SerializeField]
        private Transform m_PlatformDestination;
        [SerializeField]
        private Transform m_PlatformStack;
        [SerializeField]
        private Transform m_PlatformMoving;
        private List<Platform> m_SpawnedPlatform = new();

        private IEnumerator Start()
        {
            yield return new WaitForSeconds(2f);
            SpawnPlatformInternal();
        }
        private AssetReferenceGameObject PlatformAssetInternal => m_LevelDefinition.PlatformAsset;
        public void SpawnPlatform()
        {
            SpawnPlatformInternal();
        }

        private void SpawnPlatformInternal()
        {
            Addressables.InstantiateAsync(PlatformAssetInternal, m_PlatformDestination, true).Completed += OnPlatformSpawned;
        }

        private Transform LeftOrRight()
        {
            int random = Random.Range(-100, 100);
            if (random <= 0)
            {
                return m_LeftPost;
            }
            return m_RightPost;
        }
        private void OnPlatformSpawned(AsyncOperationHandle<GameObject> handle)
        {
            if (handle.Status != AsyncOperationStatus.Succeeded) return;
            GameObject result = handle.Result;
            if (result.TryGetComponent(out Platform platform))
            {
                SetStartPosition(platform);
            }
        }

        private void SetStartPosition(Platform spawn)
        {
            spawn.SetStartPosition(LeftOrRight());
            spawn.AddOnPlatformStop(Up);
            spawn.AddOnPlatformStop(SpawnPlatformInternal);
            spawn.SetSpeed(m_LevelDefinition.GetSpeed());
            spawn.SetDestination(m_PlatformDestination);
            spawn.transform.SetParent(m_PlatformStack);
            spawn.SetCanMove(true);
            AddSpawnedPlatform(spawn);
        }

        private void AddSpawnedPlatform(Platform add)
        {
            m_SpawnedPlatform.Add(add);
        }

        private void Up()
        {
            m_PlatformDestination.localPosition += Vector3.up;
        }
    }
}
