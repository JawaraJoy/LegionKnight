using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Leaderboards;
using Unity.Services.Leaderboards.Exceptions;
using Unity.Services.Leaderboards.Models;

namespace Rush
{
    public interface ILeaderboardClient
    {
        bool IsInitialized { get; }
        bool IsSignedIn { get; }
        string PlayerId { get; }
        Task<LeaderboardEntry> SubmitScoreAsync(string leaderboardId, int score);
        Task<List<LeaderboardEntry>> LoadTopRanksAsync(string leaderboardId, int limit);
        Task<LeaderboardEntry> LoadPlayerScoreAsync(string leaderboardId);
    }

    internal sealed class UnityLeaderboardClient : ILeaderboardClient
    {
        private ILeaderboardsService Leaderboards => UnityServices.Instance.GetLeaderboardsService();

        public bool IsInitialized => UnityServices.State == ServicesInitializationState.Initialized;
        public bool IsSignedIn => IsInitialized && AuthenticationService.Instance.IsSignedIn;
        public string PlayerId => IsInitialized ? AuthenticationService.Instance.PlayerId : null;

        public Task<LeaderboardEntry> SubmitScoreAsync(string leaderboardId, int score)
        {
            return Leaderboards.AddPlayerScoreAsync(leaderboardId, score);
        }

        public async Task<List<LeaderboardEntry>> LoadTopRanksAsync(string leaderboardId, int limit)
        {
            var page = await Leaderboards.GetScoresAsync(leaderboardId,
                new GetScoresOptions { Limit = limit, Offset = 0 });
            return page.Results;
        }

        public async Task<LeaderboardEntry> LoadPlayerScoreAsync(string leaderboardId)
        {
            try { return await Leaderboards.GetPlayerScoreAsync(leaderboardId); }
            catch (LeaderboardsException exception) when (exception.Reason == LeaderboardsExceptionReason.EntryNotFound)
            {
                return null;
            }
        }
    }
}
