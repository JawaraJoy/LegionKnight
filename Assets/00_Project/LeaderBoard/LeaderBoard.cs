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
        [SerializeField]
        private string m_LeaderboardId = "top_player"; // Replace with your actual leaderboard ID

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
                // Authenticate the player if not already authenticated
                if (!AuthenticationService.Instance.IsSignedIn)
                {
                    try
                    {
                        await AuthenticationService.Instance.SignInAnonymouslyAsync();
                        Debug.Log("Player signed in anonymously.");
                    }
                    catch (Exception authEx)
                    {
                        Debug.LogError($"Failed to authenticate player: {authEx.Message}");
                        return; // Exit if authentication fails
                    }
                }

                var playerScoreEntry = await LeaderboardsService.Instance.GetPlayerScoreAsync(m_LeaderboardId);
                if (playerScoreEntry != null && playerScoreEntry.Score >= score)
                {
                    Debug.Log($"Current score {playerScoreEntry.Score} is higher or equal to submitted score {score}. Not submitting.");
                    return; // Do not submit if the current score is higher or equal
                }
                await LeaderboardsService.Instance.AddPlayerScoreAsync(m_LeaderboardId, score);

                Debug.Log($"Score {score} submitted successfully!");
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"Failed to submit score: {ex.Message}");
            }
        }

        public async Task<List<LeaderboardEntry>> GetTopRanks()
        {
            try
            {
                // Authenticate the player if not already authenticated
                if (!AuthenticationService.Instance.IsSignedIn)
                {
                    Debug.LogError("Player is not authenticated.");
                    // Attempt to sign in anonymously
                    return null; // Handle authentication failure
                }

                var playerScoreEntry = await LeaderboardsService.Instance.GetPlayerScoreAsync(m_LeaderboardId);
                if (playerScoreEntry == null)
                {
                    Debug.Log("Player has not submitted a score. Submitting a default score of 0.");
                    await SubmitScoreInternal(0); // Submit a default score of 0
                }

                // Retrieve the top scores from the leaderboard
                var scores = await LeaderboardsService.Instance.GetScoresAsync(m_LeaderboardId, new GetScoresOptions
                {
                    Limit = m_MaxRankToDisplay
                });

                Debug.Log($"Retrieved {scores.Results.Count} leaderboard entries.");
                return scores.Results;
            }
            catch (System.Exception ex)
            {
                if (ex.Message.Contains("Leaderboard entry could not be found"))
                {
                    Debug.LogWarning("Player has not submitted a score. Submitting a default score of 0.");
                    await SubmitScoreInternal(0); // Submit a default score of 0
                }
                else
                {
                    Debug.LogError($"Failed to retrieve leaderboard: {ex.Message}");
                }
                return new List<LeaderboardEntry>();
            }
        }
    }
}