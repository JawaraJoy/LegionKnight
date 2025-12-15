using AppsFlyerSDK;
using System;
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
        private UnityEvent<BosDefinition> OnLevelStart = new();
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
            if (!m_Completed)
            {
                if (m_LevelDefinition.FirstReward == null) return;
                m_LevelDefinition.FirstReward.ClaimReward();
            }
            else
            {
                if (m_LevelDefinition.RepeatReward == null) return;
                m_LevelDefinition.RepeatReward.ClaimReward();
            }
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
                OnLevelStart?.Invoke(m_LevelDefinition.BosDefinition);
            }
        }
    }
    
    public partial class LevelHandler : MonoBehaviour
    {
        [SerializeField]
        private bool m_LevelOver;
        [SerializeField]
        private int m_MaxPlatformCount = 10;
        [SerializeField]
        private LevelDefinition m_SelectedLevelDefinition;
        [SerializeField]
        private LevelSelect[] m_LevelSelects;
        private LevelObject m_LevelObject;
        [SerializeField]
        [Obsolete]
        private Currency m_CurrentCoinReward;
        [SerializeField]
        [Obsolete]
        private Currency m_CurrentScore;
        [SerializeField, Range(0f, 2f)]
        private float m_ExpReceiverRate = 1f;
        [SerializeField]
        private UnityEvent m_OnPlay = new();
        public int MaxPlatformCount => m_MaxPlatformCount;
        [Obsolete("Soon gonna be replaced by Loot System")]
        public Currency CurrentCoinReward => m_CurrentCoinReward;
        [Obsolete("Soon gonna be replaced by Loot System")]
        public Currency CurrentScore => m_CurrentScore;
        public Transform PlayerStartPostion => m_LevelObject.PlayerStartPostion;
        public bool LevelOver => m_LevelOver;
        public LevelDefinition LevelDefinition => m_SelectedLevelDefinition;
        public bool IsInfiniteLevel => m_SelectedLevelDefinition.IsInfiniteLevel;
        public float SpeedPlatformRate => m_LevelObject.SpeedPlatformRate;
        public Transform PlatformDestination => m_LevelObject.GetPlatformDestination();

        private BosEnemy m_SpawnedBosEnemy;
        private int m_BosSpawnCount;
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
        public int BossSpawnCount => m_BosSpawnCount;
        [SerializeField]
        private CurrencyDefinition m_ExpDefinition;
        [SerializeField]
        private CurrencyDefinition m_PotOfLifeDefinition;
        public void Init()
        {
            foreach (LevelSelect levelSelect in m_LevelSelects)
            {
                levelSelect.Init();
            }
            
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
            bool hasBos = m_SelectedLevelDefinition.HasBoss();
            GameManager.Instance.SetActiveBosIndicatorView(hasBos);
            Debug.Log("Has Boss: " + hasBos);
            SetBossSpawnCountInternal(0);
            Player.Instance.SetCurrencyAmount(m_PotOfLifeDefinition, 0);
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

                string eventName = AFEventName.OnLevelCompleted;
                CharacterDefinition usedChar = Player.Instance.UsedCharacter;
                CharacterUnit usedCharUnit = Player.Instance.GetCharacterUnit(usedChar);
                //AppsflyerManager.Instance.SendEvent(eventName, usedChar.Label, usedCharUnit.Level.ToString());
                //AppsflyerManager.Instance.SendEvent(eventName, LevelDefinition.LevelName, set.BosDefinition.Id);
                Dictionary<string, string> eventValues = new Dictionary<string, string>
                {
                    {"characterused", usedChar.Label},
                    {"characterlevel", usedCharUnit.Level.ToString()},
                    {"characterbt", usedCharUnit.Star.ToString()},
                    {"levelname",  set.LevelName}
                };
                AppsFlyer.sendEvent(eventName, eventValues);

                //--Tenjin Record
                TenjinManager.Instance.StopRecordProgression();
            }
        }

        private void SetLastPlayerPositionInternal(Vector2 set)
        {
            m_LastPlayerPost = set;
        }

        public void RessurectionPlayer()
        {
            RessurectionPlayerInternal();
        }
        private void RessurectionPlayerInternal()
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
                //SetLastPlayerPositionInternal(m_LevelObject.PlayerStartPostion.position);
            }
            DelayActionInternal(1f, action);
        }
        public void ApplyPotOfLife()
        {
            
            bool has = Player.Instance.HasCurrency(m_PotOfLifeDefinition, out Currency currency);
            if (has && currency.Amount > 0)
            {
                int maxHealth = Player.Instance.MaxHealth;
                float rebornRate = 1f;
                CustomVariable<float> potOfLifeVariable = currency.CurrencyDefinition.GetCustomVariable("rebornRate");
                if (potOfLifeVariable != null)
                {
                    rebornRate = potOfLifeVariable.Value;

                    int rebornHealth = Mathf.RoundToInt(maxHealth * rebornRate);
                    Player.Instance.SetPause(true);
                    DelayActionInternal(2, () =>
                    {
                        PotOfLifeEff(rebornHealth, currency);
                    });
                }
                else
                {
                    ShowInternitialAdsAndGameOver();
                }
            }
            else
            {
                ShowInternitialAdsAndGameOver();
            }
        }

        private void ShowInternitialAdsAndGameOver()
        {
            bool canRevive = Player.Instance.CanUseResurrectionAds;
            if (!canRevive)
            {
                UnityService.Instance.UnityAdsManager.LevelPlayService.ShowInternitialAds(() =>
                {
                    
                    Debug.Log("Show Internitial Ads - Game Over");
                });
            }
            GameManager.Instance.ShowPanel(PanelId.GameOverPanelId);
        }
        private void PotOfLifeEff(int rebornHealth, Currency currency)
        {
            RessurectionPlayerInternal();
            Player.Instance.SetCurrentHealth(rebornHealth);
            Player.Instance.RemoveCurrencyAmount(currency.CurrencyDefinition, 1);
            GameManager.Instance.GetLootStorageManager().RemoveLoot(new LootField(currency.CurrencyDefinition, false, 0, 0));
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
        public void RemoveStandByPlatform(StandbyPlatformDefinition platform)
        {
            m_LevelObject.RemoveStandByPlatform(platform);
        }
        public void AddStandByPlatform(StandbyPlatformDefinition platform)
        {
            m_LevelObject.AddStandByPlatform(platform);
        }

        public void OnPerectTouchDownInvoke()
        {
            m_OnPerfectTouchDown?.Invoke();
            Vector2 playerPost = Player.Instance.transform.position;
            SetLastPlayerPositionInternal(playerPost);
            AddManaToAllBosSkill(1);
        }
        public void OnNormalTouchDown()
        {
            m_OnNormalTouchDown?.Invoke();
            Vector2 playerPost = Player.Instance.transform.position;
            AddManaToAllBosSkill(10);
            SetLastPlayerPositionInternal(playerPost);
        }
        public void SetBosSpawnCount(int set)
        {
            SetBossSpawnCountInternal(set);
        }
        private void SetBossSpawnCountInternal(int set)
        {
            //if (m_SelectedLevelDefinition ==)
            m_BosSpawnCount = set;
        }
        private void AddBosSpawnCountInternal(int add)
        {
            m_BosSpawnCount += add;
        }
        public void ResetBoss()
        {
            if (!m_SelectedLevelDefinition.HasBoss()) return;
            List<StandbyPlatformDefinition> bosStandbyPlatforms = m_SelectedLevelDefinition.GetBosPlatformAssets();
            bool isInfinite = m_SelectedLevelDefinition.IsInfiniteLevel;
            if (isInfinite)
            {
                bool hasBos = m_SpawnedBosEnemy != null;
                if (hasBos)
                {
                    bosStandbyPlatforms = m_SpawnedBosEnemy.BosDefinition.BosPlatformsAsset;
                }
            }
            RemoveStandbyPlatformInternal(bosStandbyPlatforms);
            m_OnResetBoss?.Invoke();

            List<BosDamageable> dmg = new(FindObjectsByType<BosDamageable>(FindObjectsInactive.Include, FindObjectsSortMode.None));
            foreach (BosDamageable d in dmg)
            {
               // Destroy(d.gameObject);
            }
        }
        public void StoreLevelScore()
        {
            /*LootField coinLoot = new(m_CurrentScore.CurrencyDefinition, false, m_CurrentScore.Amount, 1f);
            LootStorage lootStorage = GameManager.Instance.GetLootStorageManager();
            lootStorage.AddLoot(coinLoot);
            //Player.Instance.AddCurrencyAmount(m_CurrentCoinReward.CurrencyDefinition, m_CurrentCoinReward.Amount);
            int exp = Mathf.RoundToInt(m_CurrentScore.Amount * m_ExpReceiverRate);
            LootField expLoot = new(m_ExpDefinition, false, exp, 1f);
            lootStorage.AddLoot(expLoot);*/
            //Player.Instance.AddPlayerExperience(exp);
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
            bool isInfite = m_SelectedLevelDefinition.IsInfiniteLevel;
            BosDefinition bosDef = m_SelectedLevelDefinition.BosDefinition;
            if (isInfite)
            {
                bosDef = GameManager.Instance.GetRandomDefeatedBoss().BosDefinition;
            }
            m_SpawnedBosEnemy.SetBosDefinition(bosDef);
            //m_SpawnedBosEnemy.InitDamageable(m_BosHealthBonus * m_BosSpawnCount
            m_SpawnedBosEnemy.InitDamageable(m_BosSpawnCount);
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
        }
        public void SetCurrentTouchDownPost(Vector2 playerTouchDown)
        {
            m_LevelObject.SetCurrentTouchDownPost(playerTouchDown);
        }
        public void SetSpeedPlatformRate(float rate)
        {
            m_LevelObject.SetSpeedPlatformRate(rate);
        }
        public void AddSpeedPlatformRate(float add)
        {
            m_LevelObject.AddSpeedPlatformRate(add);
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
            LootStorage lootStorage = GameManager.Instance.GetLootStorageManager();

            //Player.Instance.AddCurrencyAmount(m_CurrentCoinReward.CurrencyDefinition, m_CurrentCoinReward.Amount);
            int exp = Mathf.RoundToInt(add * m_ExpReceiverRate);
            LootField expLoot = new(m_ExpDefinition, false, exp, 1f);
            lootStorage.AddLoot(expLoot);
            Player.Instance.AddPlayerExperience(exp);

            Player.Instance.SetCurrencyAmount(m_ExpDefinition, 0);
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
            LootStorage lootStorage = GameManager.Instance.GetLootStorageManager();
            LootField coinLoot = new(m_CurrentCoinReward.CurrencyDefinition, false, add, 1f);

            lootStorage.AddLoot(coinLoot);
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
            SetLastPlayerPositionInternal(Vector2.zero);
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
