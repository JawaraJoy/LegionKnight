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
        // INITIALIZATION
        // ---------------------------------------------------
        public async void Init()
        {
            try
            {
                if (UnityServices.State != ServicesInitializationState.Initialized)
                {
                    await UnityServices.InitializeAsync();
                    Debug.Log("[LeaderBoard] Unity Services initialized.");
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"[LeaderBoard] Failed to initialize Unity Services: {ex.Message}");
            }
        }

        // ---------------------------------------------------
        // SUBMIT SCORE
        // ---------------------------------------------------
        public async Task SubmitScore(int score)
        {
            await SubmitScoreInternal(score);
        }

        private async Task SubmitScoreInternal(int score)
        {
            try
            {
                // Ensure services are initialized
                if (UnityServices.State != ServicesInitializationState.Initialized)
                {
                    Debug.LogWarning("[LeaderBoard] Unity Services not initialized. Attempting initialization...");
                    await UnityServices.InitializeAsync();
                }

                // Wait for authentication (since user handles login externally)
                await WaitForAuthenticationAsync();

                if (!AuthenticationService.Instance.IsSignedIn)
                {
                    Debug.LogError("[LeaderBoard] Player is not signed in. Cannot submit score.");
                    return;
                }

                // Try to get player's current leaderboard entry
                LeaderboardEntry playerScoreEntry = null;
                try
                {
                    playerScoreEntry = await LeaderboardsService.Instance.GetPlayerScoreAsync(m_LeaderboardId);
                }
                catch (Exception ex)
                {
                    if (!ex.Message.Contains("Leaderboard entry could not be found"))
                    {
                        Debug.LogError($"[LeaderBoard] Failed to fetch player score: {ex.Message}");
                        return;
                    }
                }

                // Prevent downgrading the score
                if (playerScoreEntry != null && playerScoreEntry.Score >= score)
                {
                    Debug.Log($"[LeaderBoard] Current score ({playerScoreEntry.Score}) is higher or equal to submitted score ({score}). No update.");
                    return;
                }

                // Submit score
                await LeaderboardsService.Instance.AddPlayerScoreAsync(m_LeaderboardId, score);
                Debug.Log($"[LeaderBoard] Successfully submitted score: {score}");
            }
            catch (Exception ex)
            {
                Debug.LogError($"[LeaderBoard] Failed to submit score: {ex.Message}");
            }
        }

        // ---------------------------------------------------
        // FETCH LEADERBOARD
        // ---------------------------------------------------
        public async Task<List<LeaderboardEntry>> GetTopRanks()
        {
            try
            {
                await WaitForAuthenticationAsync();

                if (!AuthenticationService.Instance.IsSignedIn)
                {
                    Debug.LogError("[LeaderBoard] Player is not signed in. Cannot retrieve leaderboard.");
                    return new List<LeaderboardEntry>();
                }

                var scores = await LeaderboardsService.Instance.GetScoresAsync(m_LeaderboardId, new GetScoresOptions
                {
                    Limit = m_MaxRankToDisplay
                });

                Debug.Log($"[LeaderBoard] Retrieved {scores.Results.Count} leaderboard entries.");
                return scores.Results;
            }
            catch (Exception ex)
            {
                Debug.LogError($"[LeaderBoard] Failed to retrieve leaderboard: {ex.Message}");
                return new List<LeaderboardEntry>();
            }
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
