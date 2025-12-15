using LegionKnight.Dialogue;
using MoreMountains.Tools;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Events;
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

        private Transform m_FinalPostSpawnPlatform;

        [SerializeField]
        private UnityEvent<BosDefinition> m_OnLevelStart = new();
        [SerializeField]
        private UnityEvent<LevelDefinition> m_OnLevelDefinitionSet = new();
        [SerializeField]
        private UnityEvent<BosDefinition> m_OnBosSpawned = new();

        [SerializeField, MMReadOnly]
        private List<Platform> m_SpawnedPlatform = new();

        [SerializeField, MMReadOnly]
        private List<StandbyPlatformDefinition> m_RealStanbyPlatformAssets = new();
        public Transform PlayerStartPostion => m_PlayerStartPosition;
        private AssetReferenceGameObject BosAssetInternal
        {
            get
            {
                bool isInfinite = GetLevelDefinition().IsInfiniteLevel;
                AssetReferenceGameObject bosAsset = GetLevelDefinition().BosAsset;
                if (isInfinite)
                {
                    bosAsset = GameManager.Instance.GetRandomDefeatedBoss().BosAsset;
                    return bosAsset;
                }
                return bosAsset;
            }
        }

        private const float m_OffsideDestination = -0.1f;
        public Transform PlatformDestination => m_PlatformDestination;

        private float m_SpeedPlatformRate = 1f;
        public float SpeedPlatformRate => m_SpeedPlatformRate;
        public void SetSpeedPlatformRate(float rate)
        {
            m_SpeedPlatformRate = rate;
        }
        public void AddSpeedPlatformRate(float rate)
        {
            m_SpeedPlatformRate += rate;
        }

        private float m_FinalOffsideDestination;
        private void Start()
        {
            
            GameManager.Instance.SetLevelObject(this);
            AddRealStanbyPlatformInternal(GetLevelDefinition().GetPlatformAssets());
            Player.Instance.AddPlayerStandbyPlatform();
            Player.Instance.AddUniqueHeroPlatform();
            GameManager.Instance.ResetBoss();
            
            m_OnLevelStart?.Invoke(GetLevelDefinition().BosDefinition);
            //OpenBos();
        }

        public void RemovePlatform(Platform platform)
        {
            RemovePlatformInternal(platform);
        }

        public void AddStandByPlatform(StandbyPlatformDefinition platform)
        {
            if (platform == null) return;
            if (m_RealStanbyPlatformAssets.Contains(platform)) return;
            m_RealStanbyPlatformAssets.Add(platform);
        }
        public void RemoveStandByPlatform(StandbyPlatformDefinition platform)
        {
            if (platform == null) return;
            if (m_RealStanbyPlatformAssets.Count <= 0) return;
            if (m_RealStanbyPlatformAssets.Contains(platform))
            {
                m_RealStanbyPlatformAssets.Remove(platform);
            }
        }

        private void RemovePlatformInternal(Platform platform)
        {
            if (m_SpawnedPlatform.Count <= 0) return;
            if (m_SpawnedPlatform.Contains(platform))
            {
                m_SpawnedPlatform.Remove(platform);
            }
        }
        public Platform GetLastSpawnedPlatform()
        {
            return GetLastSpawnedPlatformInternal();
        }

        private Platform GetLastSpawnedPlatformInternal()
        {
            if (m_SpawnedPlatform.Count <= 0) return null;
            int lastIndex = m_SpawnedPlatform.Count - 1;
            return m_SpawnedPlatform[lastIndex];
        }
        public void SetLastSpawnedPlatformActive(bool set)
        {
            GetLastSpawnedPlatformInternal().SetActiveBehaviourCollider(set);
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
                m_OnLevelDefinitionSet?.Invoke(m_LevelDefinition);
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

            // Skip nulls and platforms with null Platform property
            foreach (StandbyPlatformDefinition platform in m_RealStanbyPlatformAssets)
            {
                if (platform == null || platform.Platform == null)
                    continue;
                totalChance += platform.ChanceRateToSpawn;
            }

            if (totalChance == 0)
                return null; // No valid platforms

            int random = Random.Range(0, totalChance);

            float cumulativeChance = 0;
            foreach (StandbyPlatformDefinition platform in m_RealStanbyPlatformAssets)
            {
                if (platform == null || platform.Platform == null)
                    continue;
                cumulativeChance += platform.ChanceRateToSpawn;
                if (random < cumulativeChance)
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
            m_FinalPostSpawnPlatform = LeftOrRight();
            GetPlatformComingTrack().Show();
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

            //--Tenjin Record
            TenjinManager.Instance.UpdatePlatformProgression();
        }
        private void SpawnBosInternal()
        {
            if (GetLevelDefinition().HasBoss())
            {
                Addressables.InstantiateAsync(BosAssetInternal).Completed += OnSpawnBosInternal;
                //StartCoroutine(SpawningBosInternal(loading));
            }
        }

        private void OnSpawnBosInternal(AsyncOperationHandle<GameObject> handle)
        {
            if (handle.Status != AsyncOperationStatus.Succeeded) return;
            GameObject result = handle.Result;
            if (result.TryGetComponent(out BosEnemy bos))
            {
                GameManager.Instance.SetSpawnedBosEnemy(bos);
                float offset = Player.Instance.transform.position.y + 100f;
                bos.SetLocalPosition(new Vector2(0f, offset));
                m_BosSpawnPost.DetachChildren();
                m_OnBosSpawned?.Invoke(GetLevelDefinition().BosDefinition);
                BosDefinition bosDefinition = GetLevelDefinition().BosDefinition;

                bool isInfinite = GetLevelDefinition().IsInfiniteLevel;
                if (!isInfinite)
                {
                    ConversationDefinition conversation = bosDefinition.ConversationDefinition;
                    if (conversation == null)
                    {
                        
                    }
                    else
                    {
                        GameManager.Instance.StartConversation(conversation);
                    }
                }
                else
                {
                    GameplayPanel gameplayPanel = GameManager.Instance.GetPanel<GameplayPanel>();
                    BosBarGameplay bosBarGameplay = gameplayPanel.GetBinding<BosBarGameplay>();
                    bosBarGameplay.ShowHealthBar();
                }
            }
        }

        public void StartBos()
        {
            if (!GetLevelDefinition().HasBoss()) return;
            AddRealStanbyPlatformInternal(GetLevelDefinition().GetBosPlatformAssets());
            
            GameManager.Instance.SetBosTriggered(true);
            SpawnBosInternal();
        }

        private void OpenBos()
        {
            if (GetLevelDefinition().HasBoss())
            {
                SpawnBosInternal();
            }
            GameObject gameObject = GameManager.Instance.SpawnedBosenemy.gameObject;
            Destroy(gameObject, 5f);
        }


        public void RemoveBos()
        {
            if (!GetLevelDefinition().HasBoss()) return;
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

            //--Tenjin Record
            TenjinManager.Instance.StartRecordProgression();
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

        private GameplayPanel GetGameplayPanel()
        {
            return GameManager.Instance.GetPanel<GameplayPanel>();
        }

        private PlatformComingTrack GetPlatformComingTrack()
        {
            GameplayPanel gameplayPanel = GetGameplayPanel();
            return gameplayPanel.GetBinding<PlatformComingTrack>();
        }

        private bool m_IsLeftSide;
        private Transform LeftOrRight()
        {
            if (LeftOrRightBool())
            {
                return m_LeftPost;
            }
            else
            {
                return m_RightPost;
            }
        }
        private bool LeftOrRightBool()
        {
            int random = Random.Range(-100, 100);
            GetPlatformComingTrack().Hide();
            m_IsLeftSide = random <= 0;
            if (m_IsLeftSide)
            {
                m_FinalOffsideDestination = m_OffsideDestination * -1f;
                GetPlatformComingTrack().ShowLeftTrack();
            }
            else
            {
                m_FinalOffsideDestination = m_OffsideDestination * 1f;
                GetPlatformComingTrack().ShowRightTrack();
            }
            return m_IsLeftSide;
        }
        private Vector2 GetFinalDestination()
        {
            Vector2 target = new Vector2(m_PlatformDestination.position.x + m_FinalOffsideDestination, m_PlatformDestination.position.y);
            return target;
        }
        public Transform GetPlatformDestination()
        {
            return m_PlatformDestination;
        }
        public bool HasBoss()
        {
            return HasBossInternal();
        }
        private bool HasBossInternal()
        {
            return GetLevelDefinition().HasBoss();
        }

        private Transform GetFinalPostSpawnPlatform()
        {
            if (m_FinalPostSpawnPlatform == null)
            {
                m_FinalPostSpawnPlatform = LeftOrRight();
            }
            return m_FinalPostSpawnPlatform;
        }
        private void SetStartPosition(Platform spawn)
        {
            GetPlatformComingTrack().Show();
            bool isFatalLevel = GetLevelDefinition().IsFatalLevel;
            spawn.SetFatal(isFatalLevel);
            spawn.SetStartPosition(m_FinalPostSpawnPlatform);
            
            spawn.SetSpeed(GetLevelDefinition().GetSpeed());
            spawn.SetDestination(GetFinalDestination());
            spawn.transform.SetParent(m_PlatformStack);
            spawn.SetLevelDefnition(GetLevelDefinition());
            spawn.SetCanMove(true);
            AddSpawnedPlatform(spawn);
            StartCoroutine(HidePlatformTrack());
        }

        private IEnumerator HidePlatformTrack()
        {
            yield return new WaitForSeconds(1f);
            GetPlatformComingTrack().Hide();
        }

        private void AddSpawnedPlatform(Platform add)
        {
            m_SpawnedPlatform.Add(add);
            
            int count = m_SpawnedPlatform.Count;
            int maxCount = GameManager.Instance.MaxPlatformCount;
            if (count >= maxCount)
            {
                Platform firstPlatform = m_SpawnedPlatform[0];
                RemovePlatformInternal(firstPlatform);
                Addressables.ReleaseInstance(firstPlatform.gameObject);
                //Destroy(firstPlatform.gameObject);
            }
        }

        private void DestinationReset()
        {
            m_PlatformDestination.localPosition = Vector3.zero;
        }
        public float GetOffsideDestination()
        {
            return m_FinalOffsideDestination;
        }
    }
}
