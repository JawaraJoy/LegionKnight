using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Events;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace LegionKnight
{
    [System.Serializable]
    public partial class LevelSelect
    {
        [SerializeField]
        private bool m_Unlocked;
        [SerializeField]
        private bool m_Completed;
        [SerializeField]
        private LevelDefinition m_LevelDefinition;
        public bool Unlocked => m_Unlocked;
        public bool Completed => m_Completed;
        public LevelDefinition LevelDefinition => m_LevelDefinition;

        [SerializeField]
        private UnityEvent OnLevelDone;

        private string UnlockedKey => m_LevelDefinition.Id + "unl";
        private string CompletedKey => m_LevelDefinition.Id + "com";
        public void Init()
        {
            if (UnityService.Instance.HasData(UnlockedKey))
            {
                m_Unlocked = UnityService.Instance.GetData<bool>(UnlockedKey);
                Debug.Log("Unlocked: " + m_Unlocked);
            }
            else
            {
                //UnityService.Instance.SaveData(UnlockedKey, m_Unlocked);
            }
            if (UnityService.Instance.HasData(CompletedKey))
            {
                m_Completed = UnityService.Instance.GetData<bool>(CompletedKey);
                Debug.Log("Completed: " + m_Completed);
                
            }
            else
            {
                //UnityService.Instance.SaveData(CompletedKey, m_Completed);
                m_Completed = false;
            }
        }
        public void SetUnlocked(bool set)
        {
            m_Unlocked = set;
            UnityService.Instance.SaveData(UnlockedKey, set);
        }
        public void SetCompleted(bool set)
        {
            m_Completed = set;
            UnityService.Instance.SaveData(CompletedKey, set);
        }

        public void OnLevelDoneInvoke()
        {
            OnLevelDone?.Invoke();
        }

        public void StartLevel()
        {
            if (m_Unlocked)
            {
                m_LevelDefinition.StartLevel();
            }
        }
    }
    
    public partial class LevelHandler : MonoBehaviour
    {
        [SerializeField]
        private bool m_LevelOver;
        [SerializeField]
        private LevelDefinition m_SelectedLevelDefinition;
        [SerializeField]
        private LevelSelect[] m_LevelSelects;
        private LevelObject m_LevelObject;
        [SerializeField]
        private Currency m_CurrentCoinReward;
        [SerializeField]
        private Currency m_CurrentScore;
        [SerializeField]
        private UnityEvent m_OnPlay = new();
        public Currency CurrentCoinReward => m_CurrentCoinReward;
        public Currency CurrentScore => m_CurrentScore;
        public Transform PlayerStartPostion => m_LevelObject.PlayerStartPostion;
        public bool LevelOver => m_LevelOver;
        public LevelDefinition LevelDefinition => m_SelectedLevelDefinition;
        public bool IsInfiniteLevel => m_SelectedLevelDefinition.IsInfiniteLevel;
        public float SpeedPlatformRate => m_LevelObject.SpeedPlatformRate;

        private BosEnemy m_SpawnedBosEnemy;
        private int m_BosSpawnCount;
        [SerializeField]
        private int m_BosHealthBonus;
        public BosEnemy SpawnedBosEnemy => m_SpawnedBosEnemy;
        [SerializeField]
        private UnityEvent m_OnResetBoss = new();

        [SerializeField]
        private UnityEvent m_OnPerfectTouchDown = new();
        [SerializeField]
        private UnityEvent m_OnNormalTouchDown = new();

        [SerializeField]
        private UnityEvent<LevelSelect> m_OnLevelSelected = new();
        [SerializeField]
        private UnityEvent<LevelSelect> m_OnLevelUnlocked = new();
        [SerializeField]
        private UnityEvent<LevelSelect> m_OnLevelCompleted = new();

        private Vector2 m_LastPlayerPost;

        public void Init()
        {
            foreach (LevelSelect levelSelect in m_LevelSelects)
            {
                levelSelect.Init();
            }
            //SpawnBosInternal();
        }
        private void SpawnBosInternal()
        {
            if (m_SelectedLevelDefinition.HasBoss())
            {
                Addressables.InstantiateAsync(m_SelectedLevelDefinition.BosAsset).Completed += OnSpawnBosInternal;
                //StartCoroutine(SpawningBosInternal(loading));
            }
        }

        private void OnSpawnBosInternal(AsyncOperationHandle<GameObject> handle)
        {
            if (handle.Status != AsyncOperationStatus.Succeeded) return;
            GameObject result = handle.Result;
            Destroy(result, 5f); // Destroy the GameObject after a short delay to allow initialization
        }


        public Vector2 GetLastPlayerPosition()
        {
            return m_LastPlayerPost;
        }


        private LevelSelect GetLevelSelect(LevelDefinition level)
        {
            foreach (LevelSelect levelSelect in m_LevelSelects)
            {
                if (levelSelect.LevelDefinition == level)
                {
                    return levelSelect;
                }
            }
            return null;
        }

        public bool IsLevelUnlocked(LevelDefinition set)
        {
            LevelSelect levelSelect = GetLevelSelect(set);
            if (levelSelect != null)
            {
                return levelSelect.Unlocked;
            }
            return false;
        }
        public bool IsLevelCompleted(LevelDefinition set)
        {
            LevelSelect levelSelect = GetLevelSelect(set);
            if (levelSelect != null)
            {
                return levelSelect.Completed;
            }
            return false;
        }
        public void StartLevel(LevelDefinition defi)
        {
            GetLevelSelect(defi)?.StartLevel();
        }
        public bool HasBoss()
        {
            return m_SelectedLevelDefinition.HasBoss();
        }
        public void SetLevelDefinition(LevelDefinition set)
        {
            m_SelectedLevelDefinition = set;
            m_OnLevelSelected?.Invoke(GetLevelSelect(set));
        }
        public void SetLevelUnlocked(LevelDefinition set, bool unlocked)
        {
            LevelSelect levelSelect = GetLevelSelect(set);
            levelSelect?.SetUnlocked(unlocked);
            if (unlocked)
            {
                m_OnLevelUnlocked?.Invoke(levelSelect);
            }
        }
        public void SetLevelCompleted(LevelDefinition set, bool completed)
        {
            LevelSelect levelSelect = GetLevelSelect(set);
            levelSelect?.SetCompleted(completed);
            if (completed)
            {
                m_OnLevelCompleted?.Invoke(levelSelect);
            }
        }

        private void SetLastPlayerPositionInternal(Vector2 set)
        {
            m_LastPlayerPost = set;
        }

        public void RessurectionPlayer()
        {
            Player.Instance.SetPause(true);
            Player.Instance.Reborn();
            Vector2 ressoffsite = new Vector2(m_LastPlayerPost.x, m_LastPlayerPost.y + 5);
            Player.Instance.SetPosition(ressoffsite);
            
            void action()
            {
                //m_LevelObject.SetLastSpawnedPlatformActive(true);
                SetLevelOverInternal(false);
                SpawnPlatformInternal();
                Player.Instance.SetPause(false);
            }
            DelayActionInternal(1f, action);
        }
        private void DelayActionInternal(float delay, UnityAction action)
        {
            StartCoroutine(DelayingAction(delay, action));
        }
        private IEnumerator DelayingAction(float delay, UnityAction action)
        {
            yield return new WaitForSeconds(delay);
            action?.Invoke();
        }
        public void RemovePlatform(Platform platform)
        {
            m_LevelObject.RemovePlatform(platform);
        }

        public void OnPerectTouchDownInvoke()
        {
            m_OnPerfectTouchDown?.Invoke();
            Vector2 playerPost = Player.Instance.transform.position;
            SetLastPlayerPositionInternal(playerPost);
        }
        public void OnNormalTouchDown()
        {
            m_OnNormalTouchDown?.Invoke();
            Vector2 playerPost = Player.Instance.transform.position;
            SetLastPlayerPositionInternal(playerPost);
        }
        public void SetBosSpawnCount(int set)
        {
            m_BosSpawnCount = set;
        }
        private void AddBosSpawnCountInternal(int add)
        {
            m_BosSpawnCount += add;
        }
        public void ResetBoss()
        {
            if (!m_SelectedLevelDefinition.HasBoss()) return;
            RemoveStandbyPlatformInternal(m_SelectedLevelDefinition.GetBosPlatformAssets());
            m_OnResetBoss?.Invoke();

            List<BosDamageable> dmg = new(FindObjectsByType<BosDamageable>(FindObjectsInactive.Include, FindObjectsSortMode.None));
            foreach (BosDamageable d in dmg)
            {
               // Destroy(d.gameObject);
            }
        }
        public void StoreLevelScore()
        {
            Player.Instance.AddCurrencyAmount(m_CurrentCoinReward.CurrencyDefinition, m_CurrentCoinReward.Amount);
            Player.Instance.AddPlayerExperience(m_CurrentScore.Amount);
            GetLevelSelect(m_SelectedLevelDefinition)?.OnLevelDoneInvoke();
            ResetScore();
        }
        private void ResetScore()
        {
            m_CurrentScore.SetAmount(0);
            m_CurrentCoinReward.SetAmount(0);
        }
        public void ResetPlayerPost()
        {
            m_LevelObject.ResetPlayerPost();
        }
        public void SetSpawnedBosEnemy(BosEnemy set)
        {
            m_SpawnedBosEnemy = set;
            m_SpawnedBosEnemy.SetBosDefinition(m_SelectedLevelDefinition.BosDefinition);
            m_SpawnedBosEnemy.InitDamageable(m_BosHealthBonus * m_BosSpawnCount);
        }
        public BosEnemy GetSpawnedBosEnemy()
        {
            return m_SpawnedBosEnemy;
        }
        public void AddStandbyPlatform(List<StandbyPlatformDefinition> standby)
        {
            m_LevelObject.AddRealStanbyPlatform(standby);
        }
        public void RemoveStandbyPlatform(List<StandbyPlatformDefinition> standby)
        {
            RemoveStandbyPlatformInternal(standby);
        }
        private void RemoveStandbyPlatformInternal(List<StandbyPlatformDefinition> standby)
        {
            m_LevelObject.RemoveRealStanbyPlatform(standby);
        }

        private bool m_BosTriggered = false;
        public bool BosTriggered => m_BosTriggered;
        public void SetBosTriggered(bool set)
        {
            m_BosTriggered = set;
        }
        public void SetLevelObject(LevelObject set)
        {
            m_LevelObject = set;
            m_LevelObject.SetGroundLevelView(m_SelectedLevelDefinition.LevelOrnament);
        }
        public void SetCurrentTouchDownPost(Vector2 playerTouchDown)
        {
            m_LevelObject.SetCurrentTouchDownPost(playerTouchDown);
        }
        public void SetSpeedPlatformRate(float rate)
        {
            m_LevelObject.SetSpeedPlatformRate(rate);
        }
        public int GetNormalTouchDownPoint()
        {
            return m_SelectedLevelDefinition.GetNormalTouchDownPoint();
        }
        public int GetPerfectTouchDownPoint()
        {
            return m_SelectedLevelDefinition.GetPerfectTouchDownPoint();
        }
        public void SetRewardAmount(int set)
        {
            SetRewardAmountInternal(set);
        }
        public void SetLevelOver(bool set)
        {
            SetLevelOverInternal(set);
        }
        public void SetScoreAmount(int set)
        {
            SetScoreAmountInternal(set);
        }
        public void AddScoreAmount(int set)
        {
            AddScoreAmountInternal(set);
        }
        private void SetLevelOverInternal(bool set)
        {
            m_LevelOver = set;
        }
        private void SetRewardAmountInternal(int set)
        {
            m_CurrentCoinReward.SetAmount(set);
            //DetermineHighScore();
        }
        private void SetScoreAmountInternal(int set)
        {
            m_CurrentScore.SetAmount(set);
            DetermineHighScore();
        }
        private void AddScoreAmountInternal(int add)
        {
            m_CurrentScore.AddAmount(add);
            DetermineHighScore();
        }
        public void AddCurrencyRewardAmount(int add)
        {
            AddCurrencyRewardAmountInternal(add);
        }
        public void RemoveAmount(int remove)
        {
            RemoveAmountInternal(remove);
        }
        private void AddCurrencyRewardAmountInternal(int add)
        {
            m_CurrentCoinReward.AddAmount(add);
            DetermineHighScore();
        }
        
        private void RemoveAmountInternal(int remove)
        {
            m_CurrentCoinReward.RemoveAmount(remove);
        }

        private void DetermineHighScore()
        {
            int currentScore = m_CurrentScore.Amount;
            int currentHighScore = Player.Instance.GetCurrencyAmount(m_CurrentScore.CurrencyDefinition);

            if (currentScore > currentHighScore)
            {
                Player.Instance.SetCurrencyAmount(m_CurrentScore.CurrencyDefinition, currentScore);
            }
        }

        private void OnPlayInvoke()
        {
            m_OnPlay?.Invoke();
            m_LevelObject.RemoveBos();
            bool hasBos = m_SelectedLevelDefinition.HasBoss();
            GameManager.Instance.SetActiveBosIndicatorView(hasBos);
        }
        public void StartBos()
        {
            m_LevelObject.StartBos();
            AddBosSpawnCountInternal(1);
        }
        public void Play()
        {
            PlayInternal();
        }
        private void PlayInternal()
        {
            SetRewardAmountInternal(0);
            SetScoreAmountInternal(0);
            SetLevelOverInternal(false);

            m_LevelObject.Play();
            OnPlayInvoke();
        }
        public void SpawnPlatform()
        {
            SpawnPlatformInternal();
        }

        private void SpawnPlatformInternal()
        {
            m_LevelObject.SpawnPlatform();
        }
        public float GetOffsideDestination()
        {
            return m_LevelObject.GetOffsideDestination();
        }
        public Transform GetPlatformDestination()
        {
            return m_LevelObject.GetPlatformDestination();
        }
    }
}
