using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Events;

namespace LegionKnight
{
    
    public partial class LevelHandler : MonoBehaviour
    {
        
        [SerializeField]
        private bool m_LevelOver;
        [SerializeField]
        private LevelDefinition m_LevelDefinition;

        private LevelObject m_LevelObject;
        [SerializeField]
        private Currency m_CurrentCoinReward;
        [SerializeField]
        private Currency m_CurrentScore;
        [SerializeField]
        private Currency m_CurrentHighScore;
        [SerializeField]
        private UnityEvent m_OnPlay = new();
        public Currency CurrentCoinReward => m_CurrentCoinReward;
        public Currency CurrentScore => m_CurrentScore;
        public Currency CurrentHighScore => m_CurrentHighScore;

        public Transform PlayerStartPostion => m_LevelObject.PlayerStartPostion;
        public bool LevelOver => m_LevelOver;
        public LevelDefinition LevelDefinition => m_LevelDefinition;

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

        public void OnPerectTouchDownInvoke()
        {
            m_OnPerfectTouchDown?.Invoke();
        }
        public void OnNormalTouchDown()
        {
            m_OnNormalTouchDown?.Invoke();
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
            RemoveStandbyPlatformInternal(m_LevelDefinition.GetBosPlatformAssets());
            m_OnResetBoss?.Invoke();

            List<BosDamageable> dmg = new(FindObjectsByType<BosDamageable>(FindObjectsInactive.Include, FindObjectsSortMode.None));
            foreach (BosDamageable d in dmg)
            {
               // Destroy(d.gameObject);
            }
        }
        public void ResetPlayerPost()
        {
            m_LevelObject.ResetPlayerPost();
        }
        public void SetSpawnedBosEnemy(BosEnemy set)
        {
            m_SpawnedBosEnemy = set;
            m_SpawnedBosEnemy.SetBosDefinition(m_LevelDefinition.BosDefinition);
            m_SpawnedBosEnemy.InitDamageable(m_BosHealthBonus * m_BosSpawnCount);
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
        public int GetNormalTouchDownPoint()
        {
            return m_LevelDefinition.GetNormalTouchDownPoint();
        }
        public int GetPerfectTouchDownPoint()
        {
            return m_LevelDefinition.GetPerfectTouchDownPoint();
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
            int currentHighScore = m_CurrentHighScore.Amount;

            if (currentScore > currentHighScore)
            {
                m_CurrentHighScore.SetAmount(currentScore);
            }
        }

        private void OnPlayInvoke()
        {
            m_OnPlay?.Invoke();
            m_LevelObject.RemoveBos();
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
    }
}
