using UnityEngine;

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
        public void SetLevelObject(LevelObject set)
        {
            m_LevelManager.SetLevelObject(set);
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
        public bool BosTriggered => m_LevelManager.BosTriggered;
        public void SetBosTriggered(bool set)
        {
            m_LevelManager.SetBosTriggered(set);
        }
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
    }
}
