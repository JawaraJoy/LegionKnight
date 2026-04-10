using LegionKnight;
using MoreMountains.Tools;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace Rush
{
    public class PlayerScore : MonoBehaviour, IReseter
    {
        [SerializeField]
        private ItemConfig m_ItemConsiderAsScoreConfig;
        [SerializeField, MMReadOnly]
        private int m_Score = 0;
        [SerializeField]
        private UnityEvent<int> m_OnScoreChanged;
        [SerializeField]
        private UnityEvent<Currency> m_OnScoreCurrencyChanged;
        public UnityEvent<Currency> OnScoreCurrencyChanged => OnScoreCurrencyChanged;

        private Currency m_ScoreCurrency;

        private List<CurrentScoreView> m_CurrentScoreViews = new List<CurrentScoreView>();

        private void Start()
        {
            m_CurrentScoreViews.Clear();
            m_CurrentScoreViews = FindObjectsByType<CurrentScoreView>(FindObjectsInactive.Include, FindObjectsSortMode.None).ToList();
        }
        private void RegisterCurrentScoreView(CurrentScoreView currentScoreView)
        {
            if (m_CurrentScoreViews.Contains(currentScoreView)) return;
            m_CurrentScoreViews.Add(currentScoreView);
        }
        private void UnregisterCurrentScoreView(CurrentScoreView currentScoreView)
        {
            if (m_CurrentScoreViews.Contains(currentScoreView))
            {
                m_CurrentScoreViews.Remove(currentScoreView);
            }
        }

        private void AddScoreInternal(int  score)
        {
            m_Score += score;
            m_OnScoreChanged?.Invoke(m_Score);
            m_ScoreCurrency = new Currency(m_ItemConsiderAsScoreConfig, m_Score);
            m_OnScoreCurrencyChanged?.Invoke(m_ScoreCurrency);

            foreach(var item in m_CurrentScoreViews)
            {
                item.SetView(m_ScoreCurrency);
            }
            ApplyHighScoreToPlayerInternal();
        }
        public void AddScore(int score)
        {
            AddScoreInternal(score);
        }
        private void ApplyHighScoreToPlayerInternal()
        {
            int playerHighScore = Player.Instance.CurrencyControl.GetCurrencyAmount(m_ItemConsiderAsScoreConfig);
            if (m_Score > playerHighScore)
            {
                Player.Instance.CurrencyControl.SetCurrencyAmount(m_ItemConsiderAsScoreConfig, m_Score);
                UnityService.Instance.SubmitScoreEntry(m_Score);
            }
        }
        public void ApplyHighScoreToPlayer()
        {
            ApplyHighScoreToPlayerInternal();
        }

        private void SetHighScoreInternal(int score)
        {
            m_Score = score;
            m_OnScoreChanged?.Invoke(m_Score);
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
        private PlayerScore m_PlayerScore;
        public PlayerScore PlayerScore => m_PlayerScore;
    }
}
