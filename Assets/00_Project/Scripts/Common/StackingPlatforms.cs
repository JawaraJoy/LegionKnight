using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace LegionKnight
{
    [System.Serializable]
    public class StartingPlatformMove
    {
        [SerializeField]
        private Transform m_StartPost;
        [SerializeField]
        private float m_SpeedDirection;
        public Transform StartPost => m_StartPost;
        public float SpeedDirection => m_SpeedDirection;
    }
    public partial class StackingPlatforms : MonoBehaviour
    {
        [SerializeField]
        private AssetReferenceGameObject m_PlatformAsset;
        [SerializeField]
        private StartingPlatformMove m_LeftPost;
        [SerializeField]
        private StartingPlatformMove m_RightPost;
        [SerializeField]
        private Transform m_PlatformStack;
        private List<Platform> m_SpawnedPlatform = new();

        public void SpawnPlatform()
        {
            Addressables.InstantiateAsync(m_PlatformAsset, m_PlatformStack, false).Completed += OnPlatformSpawned;
        }

        private StartingPlatformMove LeftOrRight()
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
            StartingPlatformMove post = LeftOrRight();
            spawn.transform.position = post.StartPost.position;
            spawn.StartMove(post.SpeedDirection);

            AddSpawnedPlatform(spawn);
        }

        private void AddSpawnedPlatform(Platform add)
        {
            m_SpawnedPlatform.Add(add);
        }
    }
}
