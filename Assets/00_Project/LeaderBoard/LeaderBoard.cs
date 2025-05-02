using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using Unity.Services.Authentication;
using UnityEngine;
using Unity.Services.Leaderboards;
using Unity.Services.Leaderboards.Models;
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

        [SerializeField]
        private CurrencyDefinition m_CurrentScore;
        

        public async Task SubmitScore(int score)
        {
            try
            {
                // Authenticate the player if not already authenticated
                if (!AuthenticationService.Instance.IsSignedIn)
                {
                    return; // Handle authentication failure
                }

                // Submit the player's score to the leaderboard
                int currentScore = Player.Instance.GetCurrencyAmount(m_CurrentScore);
                if (currentScore > score)
                {
                    Debug.Log($"Current score {currentScore} is higher than submitted score {score}. Not submitting.");
                    return; // Do not submit if the current score is higher
                }
                var playerScoreEntry = await LeaderboardsService.Instance.GetPlayerScoreAsync(m_LeaderboardId);
                if (playerScoreEntry != null)
                {
                    Debug.Log($"Current player score: {playerScoreEntry.Score}");
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
                    return null; // Handle authentication failure
                }

                var playerScoreEntry = await LeaderboardsService.Instance.GetPlayerScoreAsync(m_LeaderboardId);
                if (playerScoreEntry == null)
                {
                    Debug.Log("Player has not submitted a score. Submitting a default score of 0.");
                    await SubmitScore(0); // Submit a default score of 0
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
                Debug.LogError($"Failed to retrieve leaderboard: {ex.Message}");
                return new List<LeaderboardEntry>();
            }
        }
    }
}