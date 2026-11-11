using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Rush;
using Unity.Services.Leaderboards.Models;
using UnityEngine;

namespace LegionKnight
{
    public partial class LeaderBoard : MonoBehaviour
    {
        [SerializeField] private int m_MaxRankToDisplay = 20;
        [SerializeField] private string m_LeaderboardId = "highestscorerank";

        private const float c_SubmissionIntervalSeconds = 5f;
        private const float c_IdlePollIntervalSeconds = 0.5f;
        private const float c_MaxRetrySeconds = 60f;
        private const string c_UnassignedOwner = "unassigned";
        private ILeaderboardClient m_Client;
        private LeaderboardScoreOutbox m_Outbox;
        private string m_OutboxKey;
        private bool m_IsSubmitting;
        private float m_NextAttemptAt;
        private float m_RetrySeconds = c_SubmissionIntervalSeconds;

        public int MaxRankToDisplay => Mathf.Clamp(m_MaxRankToDisplay, 1, 100);
        public string LeaderboardId => m_LeaderboardId;
        public bool IsReady => m_Client != null && m_Client.IsSignedIn;
        public string CurrentPlayerId => m_Client?.PlayerId;
        private string StoragePrefix => "Rush.Leaderboard.Outbox.v1/" + Application.cloudProjectId + "/" + m_LeaderboardId + "/";

        private void Awake() { InitializeClientInternal(); }

        private void Update()
        {
            if (m_IsSubmitting || Time.realtimeSinceStartup < m_NextAttemptAt) return;
            m_NextAttemptAt = Time.realtimeSinceStartup + c_IdlePollIntervalSeconds;
            InitializeClientInternal();
            if (!m_Client.IsSignedIn) return;
            try
            {
                PrepareOutboxInternal();
                PromoteUnassignedScoreInternal();
                if (m_Outbox.HasPendingScore) _ = FlushPendingScoreInternal();
            }
            catch (Exception exception) { ScheduleRetryInternal(exception); }
        }

        /// <summary>Durably queues a score. Completion means queued, not server acknowledgement.</summary>
        public Task SubmitScore(int score)
        {
            QueueScoreInternal(score);
            return Task.CompletedTask;
        }

        private void QueueScoreInternal(int score)
        {
            InitializeClientInternal();
            PrepareOutboxInternal();
            m_Outbox.AddScore(score);
        }

        private void InitializeClientInternal()
        {
            if (m_Client == null) m_Client = new UnityLeaderboardClient();
        }

        private void PrepareOutboxInternal()
        {
            string owner = string.IsNullOrEmpty(m_Client.PlayerId) ? c_UnassignedOwner : m_Client.PlayerId;
            string key = StoragePrefix + owner;
            if (m_Outbox != null && m_OutboxKey == key) return;
            m_Outbox = new LeaderboardScoreOutbox(key);
            m_OutboxKey = key;
        }

        private void PromoteUnassignedScoreInternal()
        {
            // Persist under the authenticated account before clearing pre-login work.
            var unassigned = new LeaderboardScoreOutbox(StoragePrefix + c_UnassignedOwner);
            if (!unassigned.HasPendingScore) return;
            m_Outbox.AddScore(unassigned.PendingScore);
            unassigned.ClearUnassignedScore();
        }

        private async Task FlushPendingScoreInternal()
        {
            if (m_IsSubmitting || !IsReady || !m_Outbox.HasPendingScore) return;
            m_IsSubmitting = true;
            var outbox = m_Outbox;
            string playerId = m_Client.PlayerId;
            int score = outbox.PendingScore;
            m_NextAttemptAt = Time.realtimeSinceStartup + c_SubmissionIntervalSeconds;
            try
            {
                // Keep one request in flight, including during account changes.
                var entry = await m_Client.SubmitScoreAsync(m_LeaderboardId, score);
                if (this == null) return;
                if (entry == null || entry.PlayerId != playerId || entry.Score < score)
                    throw new InvalidOperationException("The server did not confirm the queued high score.");
                outbox.AcknowledgeScore(score);
                m_RetrySeconds = c_SubmissionIntervalSeconds;
            }
            catch (Exception exception)
            {
                if (this != null) ScheduleRetryInternal(exception);
            }
            finally { m_IsSubmitting = false; }
        }

        private void ScheduleRetryInternal(Exception exception)
        {
            m_NextAttemptAt = Time.realtimeSinceStartup + m_RetrySeconds;
            m_RetrySeconds = Mathf.Min(m_RetrySeconds * 2f, c_MaxRetrySeconds);
            Debug.LogWarning("[LeaderBoard] Score remains queued; retry scheduled. " + exception.Message, this);
        }

        public Task<List<LeaderboardEntry>> GetTopRanks()
        {
            InitializeClientInternal();
            if (!IsReady) throw new InvalidOperationException("Sign in to view the leaderboard.");
            return m_Client.LoadTopRanksAsync(m_LeaderboardId, MaxRankToDisplay);
        }

        public Task<LeaderboardEntry> LoadPlayerScoreAsync()
        {
            InitializeClientInternal();
            if (!IsReady) throw new InvalidOperationException("Sign in to view your rank.");
            return m_Client.LoadPlayerScoreAsync(m_LeaderboardId);
        }

        private void OnApplicationPause(bool paused)
        {
            if (!paused) m_NextAttemptAt = 0f;
        }
    }
}
