using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace LegionKnight
{
    
    public partial class LevelManager : Singleton<LevelManager>
    {
        [SerializeField]
        private bool m_LevelOver;
        [SerializeField]
        private LevelDefinition m_LevelDefinition;
        [SerializeField]
        private Transform m_PlayerStartPosition;
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

        [SerializeField]
        private Currency m_CurrentCoinReward;
        public Currency CurrentCoinReward => m_CurrentCoinReward;

        public Transform PlayerStartPostion => m_PlayerStartPosition;
        public void SetAmount(int set)
        {
            SetAmountInternal(set);
        }
        private void SetAmountInternal(int set)
        {
            m_CurrentCoinReward.SetAmount(set);
        }
        public void AddAmount(int add)
        {
            m_CurrentCoinReward.AddAmount(add);
        }
        public void RemoveAmount(int remove)
        {
            m_CurrentCoinReward.RemoveAmount(remove);
        }

        private void Start()
        {
            PlayInternal();
        }
        public void Play()
        {
            PlayInternal();
        }
        private void PlayInternal()
        {
            StartCoroutine(Playing());
        }

        private IEnumerator Playing()
        {
            ClearPlatform();
            DestinationReset();
            yield return new WaitForSeconds(0.5f);
            Player.Instance.Reborn();
            Player.Instance.SetPosition(m_PlayerStartPosition.position);
            yield return new WaitForSeconds(2f);
            SpawnPlatformInternal();
            SetAmountInternal(0);
        }

        private void ClearPlatform()
        {
            foreach(Platform platform in m_SpawnedPlatform)
            {
                Destroy(platform.gameObject);
            }
            m_SpawnedPlatform.Clear();
        }

        private AssetReferenceGameObject PlatformAssetInternal => m_LevelDefinition.PlatformAsset;
        public void SpawnPlatform()
        {
            SpawnPlatformInternal();
        }

        private void SpawnPlatformInternal()
        {
            if (m_LevelOver) return;
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
            spawn.SetSpeed(m_LevelDefinition.GetSpeed());
            spawn.SetDestination(m_PlatformDestination);
            spawn.transform.SetParent(m_PlatformStack);
            spawn.SetLevelDefnition(m_LevelDefinition);
            spawn.SetCanMove(true);
            AddSpawnedPlatform(spawn);
        }

        private void AddSpawnedPlatform(Platform add)
        {
            m_SpawnedPlatform.Add(add);
        }

        public void Up()
        {
            m_PlatformDestination.localPosition += Vector3.up + new Vector3(0f, 0.4f, 0f);
        }
        private void DestinationReset()
        {
            m_PlatformDestination.localPosition = Vector3.zero;
        }
    }
}
