using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Unity.Services.Leaderboards.Models;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace LegionKnight
{
    public partial class LeaderBoardPanel : PanelView
    {
        [SerializeField]
        private AssetReferenceGameObject m_RankScoreViewAsset; // Prefab for RankScoreView
        [SerializeField]
        private Transform m_Spawn;
        [SerializeField]
        private List<RankScoreView> m_RankScoreViews = new(); // Array of RankScoreView to display leaderboard entries

        [SerializeField]
        private RankScoreView m_MyScoreView; // View for the player's own score
        protected override void ShowInternal()
        {
            base.ShowInternal();
            ShowLeaderboard();
        }

        private async void ShowLeaderboard()
        {
            // Load and display the leaderboard when the panel is shown
            await DisplayLeaderboard();
        }

        public async Task DisplayLeaderboard()
        {
            try
            {
                // Get the leaderboard data and max rank to display
                var leaderboard = await UnityService.Instance.GetTopRanks();
                int maxRankToDisplay = UnityService.Instance.MaxRankToDisplay;

                // Sort leaderboard by score in descending order
                leaderboard.Sort((a, b) => b.Score.CompareTo(a.Score));

                // Populate the leaderboard, limiting to the max rank to display
                int count = Mathf.Min(maxRankToDisplay, leaderboard.Count);
                for (int i = 0; i < count; i++)
                {
                    var entry = leaderboard[i];

                    // Ensure enough RankScoreViews are instantiated
                    if (i >= m_RankScoreViews.Count)
                    {
                        var rankScoreView = await SpawnRankScoreView(entry, i + 1);
                        if (rankScoreView != null)
                        {
                            m_RankScoreViews.Add(rankScoreView);
                        }
                    }
                    else
                    {
                        // Set the rank, player name, and score in the existing RankScoreView
                        m_RankScoreViews[i].SetRankScore(entry, i + 1);
                        m_RankScoreViews[i].gameObject.SetActive(true);
                    }
                }

                // Clear any unused RankScoreViews
                for (int i = count; i < m_RankScoreViews.Count; i++)
                {
                    m_RankScoreViews[i].Clear();
                    m_RankScoreViews[i].Hide();
                }

                // Find the player's own score
                var playerId = UnityService.Instance.PlayerId; // Replace with the actual method to get the player's ID
                var playerEntry = leaderboard.FirstOrDefault(entry => entry.PlayerId == playerId);

                // Display the player's own score in m_MyScoreView
                if (playerEntry != null)
                {
                    // Find the player's rank
                    int playerRank = leaderboard.IndexOf(playerEntry) + 1;

                    // Set the player's rank, name, and score
                    m_MyScoreView.SetRankScore(playerEntry, playerRank);
                    m_MyScoreView.gameObject.SetActive(true);
                }
                else
                {
                    Debug.LogWarning("Player's score not found in the leaderboard.");
                    m_MyScoreView.Clear();
                    m_MyScoreView.gameObject.SetActive(false);
                }
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"Failed to display leaderboard: {ex.Message}");
            }
        }

        private async Task<RankScoreView> SpawnRankScoreView(LeaderboardEntry entry, int rank)
        {
            // Instantiate a new RankScoreView prefab
            var handle = Addressables.InstantiateAsync(m_RankScoreViewAsset, m_Spawn);
            await handle.Task;

            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                var rankScoreView = handle.Result.GetComponent<RankScoreView>();
                rankScoreView.SetRankScore(entry, rank);
                return rankScoreView;
            }
            else
            {
                Debug.LogError("Failed to spawn RankScoreView prefab.");
                return null;
            }
        }
    }
}
