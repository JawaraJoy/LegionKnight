using Newtonsoft.Json.Bson;
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

        private List<StandbyPlatformDefinition> m_RealStanbyPlatformAssets = new();
        public Transform PlayerStartPostion => m_PlayerStartPosition;
        private AssetReferenceGameObject BosAssetInternal => GetLevelDefinition().BosAsset;

        private const float m_OffsideDestination = -0.2f;
        private BosEnemy m_BossPrefab;

        private float m_SpeedPlatformRate = 1f;
        public float SpeedPlatformRate => m_SpeedPlatformRate;
        public void SetSpeedPlatformRate(float rate)
        {
            m_SpeedPlatformRate = rate;
        }

        private float m_FinalOffsideDestination;
        private void Start()
        {
            
            GameManager.Instance.SetLevelObject(this);
            AddRealStanbyPlatformInternal(GetLevelDefinition().GetPlatformAssets());
            Player.Instance.AddPlayerStandbyPlatform();
            Player.Instance.AddUniqueHeroPlatform();
            LoadBosInternal();
            GameManager.Instance.ResetBoss();
        }

        public void RemovePlatform(Platform platform)
        {
            if (m_SpawnedPlatform.Count <= 0) return;
            if (m_SpawnedPlatform.Contains(platform))
            {
                m_SpawnedPlatform.Remove(platform);
            }
        }

        private Platform GetLastSpawnedPlatform()
        {
            if (m_SpawnedPlatform.Count <= 0) return null;
            return m_SpawnedPlatform[m_SpawnedPlatform.Count - 1];
        }
        public void SetLastSpawnedPlatformActive(bool set)
        {
            GetLastSpawnedPlatform().SetActiveBehaviourCollider(set);
        }
        public void AddRealStanbyPlatform(List<StandbyPlatformDefinition> standby)
        {
            AddRealStanbyPlatformInternal(standby);
        }
        public void RemoveRealStanbyPlatform(List<StandbyPlatformDefinition> standby)
        {
            RemoveRealStanbyPlatformInternal(standby);
        }
        private void AddRealStanbyPlatformInternal(List<StandbyPlatformDefinition> standby)
        {
            foreach (StandbyPlatformDefinition p in standby)
            {
                m_RealStanbyPlatformAssets.Add(p);
            }
        }
        private void RemoveRealStanbyPlatformInternal(List<StandbyPlatformDefinition> standby)
        {
            if (m_RealStanbyPlatformAssets.Count <= 0) return;
            foreach (StandbyPlatformDefinition p in standby)
            {
                if (m_RealStanbyPlatformAssets.Contains(p))
                {
                    m_RealStanbyPlatformAssets.Remove(p);
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
        private AssetReferenceGameObject GetRandomPlatformByChance()
        {
            int totalChance = 0;
            AssetReferenceGameObject selected = null;
            foreach (StandbyPlatformDefinition platform in m_RealStanbyPlatformAssets)
            {
                totalChance += platform.ChanceRateToSpawn;
            }
            int random = Random.Range(0, totalChance);

            float cumulativeChance = 0;
            foreach (StandbyPlatformDefinition platform in m_RealStanbyPlatformAssets)
            {
                cumulativeChance += platform.ChanceRateToSpawn;
                if (random <= cumulativeChance)
                {
                    selected = platform.Platform;
                    break;
                }
            }
            return selected;
        }
        private void SpawnPlatformInternal()
        {
            if (GameManager.Instance.LevelOver) return;
            Vector2 farAway = new Vector2(1000f, 0f);
            Addressables.InstantiateAsync(GetRandomPlatformByChance(), farAway, Quaternion.identity).Completed += OnPlatformSpawned;
        }
        private void OnPlatformSpawned(AsyncOperationHandle<GameObject> handle)
        {
            if (handle.Status != AsyncOperationStatus.Succeeded) return;
            GameObject result = handle.Result;
            if (result.TryGetComponent(out Platform platform))
            {
                //m_PlatformDestination.position = platform.GetContactPosition();
                SetStartPosition(platform);
            }
        }
        private void SpawnBosInternal()
        {
            if (m_LevelDefinition.HasBoss())
            {
                Addressables.InstantiateAsync(BosAssetInternal, m_BosSpawnPost).Completed += OnBosSpawned;
                BosEnemy bos = Instantiate(m_BossPrefab);
                GameManager.Instance.SetSpawnedBosEnemy(bos);
                bos.SetLocalPosition(new Vector2(0f, 100f));
                m_BosSpawnPost.DetachChildren();
            }
            
        }
        private void LoadBosInternal()
        {
            if (m_LevelDefinition.HasBoss())
            {
                AsyncOperationHandle<GameObject> handle = BosAssetInternal.LoadAssetAsync<GameObject>();
                StartCoroutine(LoadingBosInternal(handle));
            }
        }
        private IEnumerator LoadingBosInternal(AsyncOperationHandle<GameObject> handle)
        {
            if (handle.Status != AsyncOperationStatus.Succeeded) yield break;
            GameObject result = handle.Result;
            if (result.TryGetComponent(out BosEnemy bos))
            {
                m_BossPrefab = bos;
            }
        }

        private void OnBosSpawned(AsyncOperationHandle<GameObject> handle)
        {
            if (handle.Status != AsyncOperationStatus.Succeeded) return;
            GameObject result = handle.Result;
            if (result.TryGetComponent(out BosEnemy bos))
            {
                GameManager.Instance.SetSpawnedBosEnemy(bos);
                bos.SetLocalPosition(new Vector2(0f, 100f));
                m_BosSpawnPost.DetachChildren();
            }
        }
        public void StartBos()
        {
            if (!m_LevelDefinition.HasBoss()) return;
            AddRealStanbyPlatformInternal(GetLevelDefinition().GetBosPlatformAssets());
            
            GameManager.Instance.SetBosTriggered(true);
            SpawnBosInternal();

        }

        public void RemoveBos()
        {
            if (!m_LevelDefinition.HasBoss()) return;
            RemoveRealStanbyPlatformInternal(GetLevelDefinition().GetBosPlatformAssets());
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
            //Player.Instance.SetPosition(m_PlayerStartPosition.position);
            yield return new WaitForSeconds(2f);
            SpawnPlatformInternal();
            Player.Instance.SetCanUseResurrectionAds(true);
        }
        public void ResetPlayerPost()
        {
            Player.Instance.SetPosition(m_PlayerStartPosition.position);
        }

        public void PauseLevel()
        {
            GameManager.Instance.SetLevelOver(true);
        }

        private void ResumeLevel()
        {
            GameManager.Instance.SetLevelOver(false);
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
                m_FinalOffsideDestination = m_OffsideDestination * -1f;
                return m_LeftPost;
            }
            m_FinalOffsideDestination = m_OffsideDestination * 1f;
            return m_RightPost;
        }
        private Vector2 GetFinalDestination()
        {
            Vector2 target = new Vector2(m_PlatformDestination.position.x + m_FinalOffsideDestination, m_PlatformDestination.position.y);
            return target;
        }
        public bool HasBoss()
        {
            return HasBossInternal();
        }
        private bool HasBossInternal()
        {
            return GetLevelDefinition().HasBoss();
        }
        private void SetStartPosition(Platform spawn)
        {
            spawn.SetStartPosition(LeftOrRight());
            spawn.SetSpeed(GetLevelDefinition().GetSpeed());
            spawn.SetDestination(GetFinalDestination());
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
