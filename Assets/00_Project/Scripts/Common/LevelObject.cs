using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace LegionKnight
{
    public partial class LevelObject : ModelView
    {
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
        private Transform m_BosSpawnPost;

        private List<Platform> m_SpawnedPlatform = new();

        private List<AssetReferenceGameObject> m_StandbyPlatformAssets = new();
        public Transform PlayerStartPostion => m_PlayerStartPosition;
        private AssetReferenceGameObject PlatformAssetInternal => GetLevelDefinition().PlatformAsset;
        private AssetReferenceGameObject BosAssetInternal => GetLevelDefinition().BosAsset;

        private void Start()
        {
            GameManager.Instance.SetLevelObject(this);
            AddStandbyPlatformInternal(GetLevelDefinition().GetPlatformAssets());
            //GameManager.Instance.ShowPanel(PanelId.StartGamePanel);
            //Player.Instance.SetCamera();

        }
        private AssetReferenceGameObject GetStandbyPlatformAssetsRandom()
        {
            int random = Random.Range(0, m_StandbyPlatformAssets.Count);
            return m_StandbyPlatformAssets[random];
        }
        private void AddStandbyPlatformInternal(List<AssetReferenceGameObject> standby)
        {
            foreach(AssetReferenceGameObject p in standby)
            {
                m_StandbyPlatformAssets.Add(p);
            }
        }
        private void RemoveStandbyPlatformInternal(List<AssetReferenceGameObject> standby)
        {
            foreach (AssetReferenceGameObject p in standby)
            {
                if (m_StandbyPlatformAssets.Contains(p))
                {
                    m_StandbyPlatformAssets.Remove(p);
                }
            }
        }
        private LevelDefinition GetLevelDefinition()
        {
            if (m_LevelDefinition == null)
            {
                m_LevelDefinition = GameManager.Instance.LevelDefinition;
            }
            return m_LevelDefinition;
        }
        public void SetCurrentTouchDownPost(Vector2 playerTouchDown)
        {
            m_PlatformDestination.position = playerTouchDown;
        }
        public void SpawnPlatform()
        {
            SpawnPlatformInternal();
        }
        private void SpawnPlatformInternal()
        {
            if (GameManager.Instance.LevelOver) return;
            Vector2 farAway = new Vector2(1000f, 0f);
            Addressables.InstantiateAsync(GetStandbyPlatformAssetsRandom(), farAway, Quaternion.identity).Completed += OnPlatformSpawned;
        }
        private void SpawnBosInternal()
        {
            Addressables.InstantiateAsync(BosAssetInternal, m_BosSpawnPost.position, Quaternion.identity).Completed += OnBosSpawned;
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
        private void OnBosSpawned(AsyncOperationHandle<GameObject> handle)
        {
            if (handle.Status != AsyncOperationStatus.Succeeded) return;
            GameObject result = handle.Result;
            if (result.TryGetComponent(out BosEnemy bos))
            {
                GameManager.Instance.SetSpawnedBosEnemy(bos);
            }
        }
        public void StartBos()
        {
            AddStandbyPlatformInternal(GetLevelDefinition().GetBosPlatformAssets());
            GameManager.Instance.SetBosTriggered(true);
            SpawnBosInternal();
        }

        public void RemoveBos()
        {
            RemoveStandbyPlatformInternal(GetLevelDefinition().GetBosPlatformAssets());
            GameManager.Instance.SetBosTriggered(false);
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
            ShowInternal();
            ClearPlatform();
            DestinationReset();
            yield return new WaitForSeconds(0.5f);
            Player.Instance.Reborn();
            Player.Instance.SetPosition(m_PlayerStartPosition.position);
            yield return new WaitForSeconds(2f);
            SpawnPlatformInternal();
            
        }
        private void ClearPlatform()
        {
            foreach (Platform platform in m_SpawnedPlatform)
            {
                Destroy(platform.gameObject);
            }
            m_SpawnedPlatform.Clear();
        }
        public void SetLevelOver(bool set)
        {
            GameManager.Instance.SetLevelOver(set);
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

        private void SetStartPosition(Platform spawn)
        {
            spawn.SetStartPosition(LeftOrRight());
            spawn.SetSpeed(GetLevelDefinition().GetSpeed());
            spawn.SetDestination(m_PlatformDestination);
            spawn.transform.SetParent(m_PlatformStack);
            spawn.SetLevelDefnition(GetLevelDefinition());
            spawn.SetCanMove(true);
            AddSpawnedPlatform(spawn);
        }

        private void AddSpawnedPlatform(Platform add)
        {
            m_SpawnedPlatform.Add(add);
        }

        private void DestinationReset()
        {
            m_PlatformDestination.localPosition = Vector3.zero;
        }
    }
}
