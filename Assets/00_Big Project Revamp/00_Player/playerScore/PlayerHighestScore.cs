using LegionKnight;
using MoreMountains.Tools;
using UnityEngine;
using UnityEngine.Events;

namespace Rush
{
    public class PlayerHighestScore : MonoBehaviour, IReseter
    {
        [SerializeField]
        private ItemConfig m_ItemConsiderAsScoreConfig;
        [SerializeField, MMReadOnly]
        private int m_HighestScore = 0;
        [SerializeField]
        private UnityEvent<int> m_OnHighScoreChanged;

        private void AddScoreInternal(int  score)
        {
            m_HighestScore += score;
            m_OnHighScoreChanged?.Invoke(m_HighestScore);
        }
        public void AddScore(int score)
        {
            AddScoreInternal(score);
        }
        private void ApplyHighScoreToPlayerInternal()
        {
            int currentPlayerHighScore = Player.Instance.CurrencyControl.GetCurrencyAmount(m_ItemConsiderAsScoreConfig);
            if (m_HighestScore < currentPlayerHighScore)
            {
                m_HighestScore = currentPlayerHighScore;
                
                Player.Instance.CurrencyControl.AddCurrencyAmount(m_ItemConsiderAsScoreConfig, m_HighestScore);
            }
        }
        public void ApplyHighScoreToPlayer()
        {
            ApplyHighScoreToPlayerInternal();
        }

        private void SetHighScoreInternal(int score)
        {
            m_HighestScore = score;
            m_OnHighScoreChanged?.Invoke(m_HighestScore);
        }
        public void SetHighScore(int score)
        {
            SetHighScoreInternal(score);
        }

        public void ResetProgression()
        {
            SetHighScoreInternal(0);
        }
    }
    public partial class RushPlayer
    {
        [SerializeField]
        private PlayerHighestScore m_PlayerScore;
        public PlayerHighestScore PlayerScore => m_PlayerScore;
    }
}
