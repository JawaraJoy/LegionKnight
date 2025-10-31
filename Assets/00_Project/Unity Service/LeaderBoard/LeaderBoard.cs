using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Services.Authentication;
using UnityEngine;
using Unity.Services.Leaderboards;
using Unity.Services.Leaderboards.Models;
using System;
using Unity.Services.Core;

namespace LegionKnight
{
    public partial class LeaderBoard : MonoBehaviour
    {
        [SerializeField]
        private int m_MaxRankToDisplay = 20; // Limit to top 20 ranks
        private readonly string m_LeaderboardId = "Legion_Knight_Top_Player"; // Replace with your actual leaderboard ID

        public int MaxRankToDisplay => m_MaxRankToDisplay;

        public async void Init()
        {
            try
            {
                if (!UnityServices.State.Equals(ServicesInitializationState.Initialized))
                {
                    await UnityServices.InitializeAsync();
                    Debug.Log("Unity Services initialized successfully.");
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"Failed to initialize Unity Services: {ex.Message}");
            }
        }
        
        public async Task SubmitScore(int score)
        {
            await SubmitScoreInternal(score);
        }
        private async Task SubmitScoreInternal(int score)
        {
            try
            {
                if (UnityServices.State != ServicesInitializationState.Initialized)
                    await UnityServices.InitializeAsync();

                // Wait until signed in
                int retries = 0;
                while (!AuthenticationService.Instance.IsSignedIn && retries < 30)
                {
                    await Task.Delay(200);
                    retries++;
                }

                if (!AuthenticationService.Instance.IsSignedIn)
                {
                    Debug.LogError("Player still not signed in after waiting. Cannot submit score.");
                    return;
                }

                // Continue to submit the score normally...
                var playerScoreEntry = await LeaderboardsService.Instance.GetPlayerScoreAsync(m_LeaderboardId);
                if (playerScoreEntry != null && playerScoreEntry.Score >= score)
                {
                    Debug.Log($"Current score ({playerScoreEntry.Score}) is higher or equal to submitted score ({score}). Not submitting.");
                    return;
                }

                await LeaderboardsService.Instance.AddPlayerScoreAsync(m_LeaderboardId, score);
                Debug.Log($"Score {score} submitted successfully!");
            }
            catch (Exception ex)
            {
                Debug.LogError($"Failed to submit score: {ex.Message}");
            }
        }

        public async Task<List<LeaderboardEntry>> GetTopRanks()
        {
            try
            {
                if (!AuthenticationService.Instance.IsSignedIn)
                {
                    await AuthenticationService.Instance.SignInAnonymouslyAsync();
                }

                var scores = await LeaderboardsService.Instance.GetScoresAsync(m_LeaderboardId, new GetScoresOptions
                {
                    Limit = m_MaxRankToDisplay
                });

                return scores.Results;
            }
            catch (Exception ex)
            {
                Debug.LogError($"Failed to retrieve leaderboard: {ex.Message}");
                return new List<LeaderboardEntry>();
            }
        }
    }
}