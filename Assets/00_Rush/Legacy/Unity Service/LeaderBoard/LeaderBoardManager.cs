using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Services.Leaderboards.Models;
using UnityEngine;

namespace LegionKnight
{
    public partial class LeaderBoardManager : LeaderBoard { }

    public partial class UnityService
    {
        [SerializeField] private LeaderBoardManager m_LeaderBoardManager;
        public int MaxRankToDisplay => m_LeaderBoardManager.MaxRankToDisplay;
        public bool IsLeaderboardReady => m_LeaderBoardManager != null && m_LeaderBoardManager.IsReady;
        public string LeaderboardPlayerId => m_LeaderBoardManager?.CurrentPlayerId;

        public Task SubmitScore(int score) { return m_LeaderBoardManager.SubmitScore(score); }
        public Task<List<LeaderboardEntry>> GetTopRanks() { return m_LeaderBoardManager.GetTopRanks(); }
        public Task<LeaderboardEntry> LoadPlayerLeaderboardScoreAsync() { return m_LeaderBoardManager.LoadPlayerScoreAsync(); }

        public void SubmitScoreEntry(int score)
        {
            // Persistence is synchronous; callers may save their local best afterwards.
            _ = m_LeaderBoardManager.SubmitScore(score);
        }
    }
}
