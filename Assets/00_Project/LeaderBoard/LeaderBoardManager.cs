using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.CloudSave;
using Unity.Services.Leaderboards;
using Unity.Services.Leaderboards.Models;
using UnityEngine;

namespace LegionKnight
{
    public partial class LeaderBoardManager : LeaderBoard
    {
    }

    public partial class UnityService
    {
        [SerializeField]
        private LeaderBoardManager m_LeaderBoardManager;

        public int MaxRankToDisplay => m_LeaderBoardManager.MaxRankToDisplay;
        public async Task SubmitScore(int score)
        {
            await m_LeaderBoardManager.SubmitScore(score);

        }

        public async Task<List<LeaderboardEntry>> GetTopRanks()
        {
            return await m_LeaderBoardManager.GetTopRanks();
        }

        public async void SubmiteScoreEntry(int score)
        {
            await m_LeaderBoardManager.SubmitScore(score);
        }
        public void Init()
        {
            m_LeaderBoardManager.Init();
        }
    }
}
