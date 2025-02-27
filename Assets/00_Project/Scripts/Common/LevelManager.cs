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
        public void SetLevelOver(bool set)
        {
            m_LevelManager.SetLevelOver(set);
        }
        public void Play()
        {
            m_LevelManager.Play();
        }
        public void AddAmount(int add)
        {
            m_LevelManager.AddAmount(add);
        }
        public void RemoveAmount(int remove)
        {
            m_LevelManager.RemoveAmount(remove);
        }
        public void SpawnPlatform()
        {
            m_LevelManager.SpawnPlatform();
        }
        public void Up()
        {
            m_LevelManager.Up();
        }
        public void ApplyNormalReward()
        {
            m_LevelManager.ApplyNormalReward();
        }
        public void ApplyPerfectReward()
        {
            m_LevelManager.ApplyPerfectReward();
        }
        public void HideLevel()
        {
            m_LevelManager.Hide();
        }
    }
}
