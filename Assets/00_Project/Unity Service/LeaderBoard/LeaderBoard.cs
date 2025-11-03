using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Leaderboards;
using Unity.Services.Leaderboards.Models;
using UnityEngine;

namespace LegionKnight
{
    public partial class LeaderBoard : MonoBehaviour
    {
        [Header("Leaderboard Settings")]
        [SerializeField]
        private int m_MaxRankToDisplay = 20;

        [SerializeField]
        private string m_LeaderboardId = "highestscorerank"; // ⚠️ Ensure this matches Unity Dashboard

        public int MaxRankToDisplay => m_MaxRankToDisplay;

        // ---------------------------------------------------
        // SUBMIT SCORE
        // ---------------------------------------------------
        public async Task SubmitScore(int score)
        {
            await SubmitScoreInternal(score);
        }

        private async Task SubmitScoreInternal(int score)
        {
            if (!AuthenticationService.Instance.IsSignedIn)
            {
                Debug.LogError("[LeaderBoard] Player is not signed in. Cannot submit score.");
                return;
            }

            Debug.Log($"[LeaderBoard] Submitting score {score} for player {AuthenticationService.Instance.PlayerId}...");

            // Submit score directly to Unity Leaderboards (no try/catch)
            var result = await LeaderboardsService.Instance.AddPlayerScoreAsync(m_LeaderboardId, score);

            if (result != null)
            {
                Debug.Log($"[LeaderBoard] ✅ Score submitted successfully. Player: {result.PlayerId}, Score: {result.Score}");
            }
            else
            {
                Debug.LogError("[LeaderBoard] ❌ Score submission returned null result.");
            }

            // Immediately fetch back player score to confirm
            Debug.Log("[LeaderBoard] Fetching back player score...");
            var playerScore = await LeaderboardsService.Instance.GetPlayerScoreAsync(m_LeaderboardId);
            Debug.Log($"[LeaderBoard] Player current score: {playerScore.Score}");
        }

        // ---------------------------------------------------
        // FETCH LEADERBOARD
        // ---------------------------------------------------
        public async Task<List<LeaderboardEntry>> GetTopRanks()
        {
            Debug.Log($"[LeaderBoard] Requesting top ranks for leaderboard: {m_LeaderboardId}");

            // Directly fetch leaderboard entries (no try/catch)
            var scoresPage = await LeaderboardsService.Instance.GetScoresAsync(m_LeaderboardId);

            Debug.Log($"[LeaderBoard] ✅ Retrieved {scoresPage.Results.Count} entries from leaderboard.");
            return scoresPage.Results;
        }

        // ---------------------------------------------------
        // HELPER — WAIT FOR AUTHENTICATION
        // ---------------------------------------------------

        private TaskCompletionSource<bool> _signInTcs;

        private async Task WaitForAuthenticationAsync()
        {
            // Already signed in? Just return.
            if (AuthenticationService.Instance.IsSignedIn)
            {
                Debug.Log("[LeaderBoard] Player already signed in.");
                return;
            }

            Debug.Log($"[LeaderBoard] Player not signed in. Waiting for authentication event...");

            _signInTcs = new TaskCompletionSource<bool>();

            void OnSignedIn()
            {
                Debug.Log($"[LeaderBoard] ✅ Authentication event received! Player ID: {AuthenticationService.Instance.PlayerId}");
                _signInTcs.TrySetResult(true);
            }

            // Subscribe to the event
            AuthenticationService.Instance.SignedIn += OnSignedIn;

            // Wait until either:
            // 1. Authentication event fires, or
            // 2. 10 seconds pass (timeout)
            await Task.WhenAny(_signInTcs.Task, Task.Delay(10000));

            // Unsubscribe after waiting
            AuthenticationService.Instance.SignedIn -= OnSignedIn;

            if (AuthenticationService.Instance.IsSignedIn)
            {
                Debug.Log($"[LeaderBoard] Authentication confirmed. Player ID: {AuthenticationService.Instance.PlayerId}");
            }
            else
            {
                Debug.LogError("[LeaderBoard] ❌ Player not signed in after waiting period.");
            }
        }

    }
}
