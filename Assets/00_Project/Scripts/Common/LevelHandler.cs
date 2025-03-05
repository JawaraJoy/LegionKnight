using UnityEngine;
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
        private Currency m_CurrentCoinHighScore;
        public Currency CurrentCoinReward => m_CurrentCoinReward;
        public Currency CurrentCoinHighScore => m_CurrentCoinHighScore;

        public Transform PlayerStartPostion => m_LevelObject.PlayerStartPostion;
        public bool LevelOver => m_LevelOver;
        public LevelDefinition LevelDefinition => m_LevelDefinition;
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
        public void ApplyNormalReward()
        {
            AddAmountInternal(m_LevelDefinition.GetNormalTouchDown().Amount);
        }
        public void ApplyPerfectReward()
        {
            AddAmountInternal(m_LevelDefinition.GetPerfectTouchdown().Amount);
        }
        public void SetRewardAmount(int set)
        {
            SetRewardAmountInternal(set);
        }
        public void SetLevelOver(bool set)
        {
            SetLevelOverInternal(set);
        }
        private void SetLevelOverInternal(bool set)
        {
            m_LevelOver = set;
        }
        private void SetRewardAmountInternal(int set)
        {
            m_CurrentCoinReward.SetAmount(set);
            DetermineHighScore();
        }
        public void AddAmount(int add)
        {
            AddAmountInternal(add);
        }
        public void RemoveAmount(int remove)
        {
            RemoveAmountInternal(remove);
        }
        private void AddAmountInternal(int add)
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
            int currentScore = m_CurrentCoinReward.Amount;
            int currentHighScore = m_CurrentCoinHighScore.Amount;

            if (currentScore > currentHighScore)
            {
                m_CurrentCoinHighScore.SetAmount(currentScore);
            }
            
        }
        public void Play()
        {
            PlayInternal();
        }
        private void PlayInternal()
        {
            SetRewardAmountInternal(0);
            SetLevelOverInternal(false);

            m_LevelObject.Play();
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
