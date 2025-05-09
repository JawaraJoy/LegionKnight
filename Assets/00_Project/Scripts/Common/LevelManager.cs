using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace LegionKnight
{
    public partial class LevelManager : LevelHandler
    {
        
    }
    public partial class GameManager
    {
        [SerializeField]
        private LevelManager m_LevelManager;

        public bool LevelOver => m_LevelManager.LevelOver;
        public LevelDefinition LevelDefinition => m_LevelManager.LevelDefinition;
        public Currency CurrentCoinReward => m_LevelManager.CurrentCoinReward;
        public Currency HighScore => m_LevelManager.CurrentHighScore;
        public bool IsInfiniteLevel => m_LevelManager.IsInfiniteLevel;

        public void InitLevelHandler()
        {
            m_LevelManager.Init();
        }
        public void SetLevelObject(LevelObject set)
        {
            m_LevelManager.SetLevelObject(set);
        }
        public void ResetPlayerPost()
        {
            m_LevelManager.ResetPlayerPost();
        }
        public bool IsLevelUnlocked(LevelDefinition set)
        {
            return m_LevelManager.IsLevelUnlocked(set);
        }
        public bool IsLevelCompleted(LevelDefinition set)
        {
            return m_LevelManager.IsLevelCompleted(set);
        }
        public void SetCurrentTouchDownPost(Vector2 playerTouchDown)
        {
            m_LevelManager.SetCurrentTouchDownPost(playerTouchDown);
        }
        public void SetRewardAmount(int set)
        {
            m_LevelManager.SetRewardAmount(set);
        }
        public void SetLevelOver(bool set)
        {
            m_LevelManager.SetLevelOver(set);
        }
        public void AddStandbyPlatform(List<StandbyPlatformDefinition> standby)
        {
            m_LevelManager.AddStandbyPlatform(standby);
        }
        public bool BosTriggered => m_LevelManager.BosTriggered;
        public void SetBosTriggered(bool set)
        {
            m_LevelManager.SetBosTriggered(set);
        }
        public BosEnemy SpawnedBosenemy => m_LevelManager.SpawnedBosEnemy;
        public void SetSpawnedBosEnemy(BosEnemy set)
        {
            m_LevelManager.SetSpawnedBosEnemy(set);
        }
        public void StartBos()
        {
            m_LevelManager.StartBos();
        }
        public void Play()
        {
            m_LevelManager.Play();
        }
        public void AddCurrencyRewardAmount(int add)
        {
            m_LevelManager.AddCurrencyRewardAmount(add);
        }
        public void RemoveAmount(int remove)
        {
            m_LevelManager.RemoveAmount(remove);
        }
        public void SpawnPlatform()
        {
            m_LevelManager.SpawnPlatform();
        }
        public int GetNormalTouchDownPoint()
        {
            return m_LevelManager.GetNormalTouchDownPoint();
        }
        public int GetPerfectTouchDownPoint()
        {
            return m_LevelManager.GetPerfectTouchDownPoint();
        }
        public void ResetBoss()
        {
            m_LevelManager.ResetBoss();
        }
        public void SetBosSpawnCount(int set)
        {
            m_LevelManager.SetBosSpawnCount(set);
            
        }
        public void OnPerectTouchDownInvoke()
        {
            m_LevelManager.OnPerectTouchDownInvoke();
        }
        public void OnNormalTouchDown()
        {
            m_LevelManager.OnNormalTouchDown();
        }
        public void SetScoreAmount(int set)
        {
            m_LevelManager.SetScoreAmount(set);
        }
        public void AddScoreAmount(int set)
        {
            m_LevelManager.AddScoreAmount(set);
        }
    }
    public partial class LevelManagerAgent
    {
        public void AddCurrencyRewardAmount(int add)
        {
            GameManager.Instance.AddCurrencyRewardAmount(add);
        }
        public void SetBosSpawnCount(int set)
        {
            GameManager.Instance.SetBosSpawnCount(set);
        }
        public void OnPerectTouchDownInvoke()
        {
            GameManager.Instance.OnPerectTouchDownInvoke();
        }
        public void OnNormalTouchDown()
        {
            GameManager.Instance.OnNormalTouchDown();
        }
        public void ResetPlayerPost()
        {
            GameManager.Instance.ResetPlayerPost();
        }
        public void SetScoreAmount(int set)
        {
            GameManager.Instance.SetScoreAmount(set);
        }
        public void AddScoreAmount(int set)
        {
            GameManager.Instance.AddScoreAmount(set);
        }
        public void InitLevelHandler()
        {
            GameManager.Instance.InitLevelHandler();
        }
    }
}
