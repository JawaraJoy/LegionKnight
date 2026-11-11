using LegionKnight;
using UnityEngine;

namespace Rush
{
    public class PlayerScoreViewAdapter : MonoBehaviour
    {
        [SerializeField]
        private CurrentScoreView[] m_ScoreViews;

        private PlayerScore m_PlayerScore;

        private void Start()
        {
            RegisterInternal();
        }

        private void OnDestroy()
        {
            UnregisterInternal();
        }

        private void RegisterInternal()
        {
            m_PlayerScore = RushPlayer.Instance.PlayerScore;
            if (m_PlayerScore == null)
            {
                Debug.LogWarning("PlayerScore is unavailable for the score view adapter.", this);
                return;
            }

            m_PlayerScore.OnScoreCurrencyChanged.RemoveListener(SetScoreViews);
            m_PlayerScore.OnScoreCurrencyChanged.AddListener(SetScoreViews);
        }

        private void UnregisterInternal()
        {
            if (m_PlayerScore == null)
            {
                return;
            }

            m_PlayerScore.OnScoreCurrencyChanged.RemoveListener(SetScoreViews);
            m_PlayerScore = null;
        }

        private void SetScoreViews(Currency currency)
        {
            foreach (CurrentScoreView scoreView in m_ScoreViews)
            {
                if (scoreView != null)
                {
                    scoreView.SetView(currency);
                }
            }
        }
    }
}
