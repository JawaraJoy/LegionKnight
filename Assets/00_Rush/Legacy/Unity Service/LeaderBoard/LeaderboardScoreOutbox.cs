using System;
using UnityEngine;

namespace Rush
{
    /// <summary>A durable, coalescing high-score queue for one leaderboard and player.</summary>
    public sealed class LeaderboardScoreOutbox
    {
        private const int c_NoScore = -1;
        private readonly string m_StorageKey;
        private ScoreState m_State;

        public bool HasPendingScore => m_State.PendingScore != c_NoScore;
        public int PendingScore => m_State.PendingScore;
        public int AcknowledgedScore => m_State.AcknowledgedScore;

        private ScoreState StoredState
        {
            get
            {
                var state = PlayerPrefs.HasKey(m_StorageKey)
                    ? JsonUtility.FromJson<ScoreState>(PlayerPrefs.GetString(m_StorageKey)) : new ScoreState();
                if (state == null || !state.IsValid)
                    throw new InvalidOperationException("Invalid leaderboard outbox; stored data was preserved.");
                return state;
            }
        }

        public LeaderboardScoreOutbox(string storageKey)
        {
            m_StorageKey = storageKey ?? throw new ArgumentNullException(nameof(storageKey));
            m_State = StoredState;
        }

        public void AddScore(int score) { AddScoreInternal(score); }

        private void AddScoreInternal(int score)
        {
            if (score < 0) throw new ArgumentOutOfRangeException(nameof(score));
            m_State = StoredState;
            if (score <= m_State.AcknowledgedScore || score <= m_State.PendingScore) return;
            PersistInternal(new ScoreState(score, m_State.AcknowledgedScore));
        }

        public void AcknowledgeScore(int submittedScore) { AcknowledgeScoreInternal(submittedScore); }

        private void AcknowledgeScoreInternal(int submittedScore)
        {
            // Another account switch or component may have queued a newer value while the request was in flight.
            m_State = StoredState;
            int acknowledged = Math.Max(m_State.AcknowledgedScore, submittedScore);
            int pending = m_State.PendingScore > acknowledged ? m_State.PendingScore : c_NoScore;
            PersistInternal(new ScoreState(pending, acknowledged));
        }

        public void ClearUnassignedScore() { PersistInternal(new ScoreState()); }

        private void PersistInternal(ScoreState state)
        {
            PlayerPrefs.SetString(m_StorageKey, JsonUtility.ToJson(state));
            PlayerPrefs.Save();
            m_State = state;
        }

        [Serializable]
        private sealed class ScoreState
        {
            [SerializeField] private int m_Version = 1;
            [SerializeField] private int m_PendingScore = c_NoScore;
            [SerializeField] private int m_AcknowledgedScore = c_NoScore;

            public int PendingScore => m_PendingScore;
            public int AcknowledgedScore => m_AcknowledgedScore;
            public bool IsValid => m_Version == 1 && m_PendingScore >= c_NoScore && m_AcknowledgedScore >= c_NoScore;

            public ScoreState() { }
            public ScoreState(int pending, int acknowledged)
            {
                m_PendingScore = pending;
                m_AcknowledgedScore = acknowledged;
            }
        }
    }
}
