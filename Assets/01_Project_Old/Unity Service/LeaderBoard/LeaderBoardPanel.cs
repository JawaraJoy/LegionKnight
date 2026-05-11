using Rush;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Services.Authentication;
using Unity.Services.Leaderboards;
using Unity.Services.Leaderboards.Models;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace LegionKnight
{
    public partial class LeaderBoardPanel : PanelView
    {
        [Header("Leaderboard Settings")]
        [SerializeField] private AssetReferenceGameObject m_RankScoreViewAsset; // Prefab for RankScoreView
        [SerializeField] private Transform m_Spawn; // Parent container for rank views
        [SerializeField] private List<RankScoreView> m_RankScoreViews = new(); // Cached RankScoreView list
        [SerializeField] private RankScoreView m_MyScoreView; // View for player's own score

        [SerializeField]
        private string m_LeaderboardId = "highestscorerank";

        // ---------------------------------------------------
        // PANEL LIFECYCLE
        // ---------------------------------------------------
        protected override void ShowInternal()
        {
            base.ShowInternal();
            RushGameManager.Instance.StartCoroutine(ShowLeaderboardCoroutine());
        }

        protected override void HideInternal()
        {
            base.HideInternal();
            StopAllCoroutines();
        }

        // ---------------------------------------------------
        // MAIN FLOW
        // ---------------------------------------------------
        private IEnumerator ShowLeaderboardCoroutine()
        {
            // ✅ Ensure UnityService + Authentication are ready
            yield return WaitForAuthenticationCoroutine();

            if (!AuthenticationService.Instance.IsSignedIn)
            {
                Debug.LogError("[LeaderBoardPanel] Player is not signed in. Cannot display leaderboard.");
                yield break;
            }

            // ✅ Fetch leaderboard entries
            var leaderboardTask = UnityService.Instance.GetTopRanks();
            while (!leaderboardTask.IsCompleted) yield return null;
            var leaderboard = leaderboardTask.Result;

            // 🔍 DEBUG TRACER
            Debug.Log("[LeaderBoard] ===== LEADERBOARD DEBUG START =====");
            Debug.Log($"[LeaderBoard] IsInitialized: {UnityService.Instance.IsInitialized}");
            Debug.Log($"[LeaderBoard] IsSignedIn: {AuthenticationService.Instance.IsSignedIn}");
            Debug.Log($"[LeaderBoard] Leaderboard ID: {m_LeaderboardId}");
            Debug.Log($"[LeaderBoard] Leaderboard Entries: {(leaderboard == null ? "NULL" : leaderboard.Count.ToString())}");
            Debug.Log($"[LeaderBoard] RankScoreViewAsset: {(m_RankScoreViewAsset == null ? "NULL" : m_RankScoreViewAsset.AssetGUID)}");
            Debug.Log($"[LeaderBoard] Spawn Parent: {(m_Spawn == null ? "NULL" : m_Spawn.name)}");
            Debug.Log("[LeaderBoard] ===== LEADERBOARD DEBUG END =====");

            if (leaderboard == null || leaderboard.Count == 0)
            {
                Debug.LogWarning("[LeaderBoardPanel] No leaderboard entries found.");
                yield break;
            }

            // ✅ Sort by score
            leaderboard.Sort((a, b) => b.Score.CompareTo(a.Score));

            int maxRankToDisplay = UnityService.Instance.MaxRankToDisplay;
            int displayCount = Mathf.Min(maxRankToDisplay, leaderboard.Count);

            Debug.Log($"[LeaderBoardPanel] Displaying top {displayCount} entries...");

            // ✅ Spawn and populate leaderboard entries
            for (int i = 0; i < displayCount; i++)
            {
                var entry = leaderboard[i];
                Debug.Log($"[LeaderBoardPanel] Rank #{i + 1} | PlayerId: {entry.PlayerId} | Score: {entry.Score}");

                if (i >= m_RankScoreViews.Count)
                    yield return SpawnRankScoreViewCoroutine(entry, i + 1);
                else
                {
                    m_RankScoreViews[i].SetRankScore(entry, i + 1);
                    m_RankScoreViews[i].gameObject.SetActive(true);
                }

                yield return null; // smooth spawn pacing
            }

            // ✅ Hide unused entries
            for (int i = displayCount; i < m_RankScoreViews.Count; i++)
            {
                m_RankScoreViews[i].Clear();
                m_RankScoreViews[i].Hide();
            }

            // ✅ Show player’s own score
            yield return DisplayMyScoreCoroutine(leaderboard);
        }

        // ---------------------------------------------------
        // SPAWN RANK VIEW
        // ---------------------------------------------------
        private IEnumerator SpawnRankScoreViewCoroutine(LeaderboardEntry entry, int rank)
        {
            if (m_RankScoreViewAsset == null)
            {
                Debug.LogError("[LeaderBoardPanel] RankScoreView asset is not assigned!");
                yield break;
            }

            var handle = Addressables.InstantiateAsync(m_RankScoreViewAsset, m_Spawn);
            yield return handle;

            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                var go = handle.Result;
                var rankScoreView = go.GetComponent<RankScoreView>() ?? go.GetComponentInChildren<RankScoreView>();

                if (rankScoreView == null)
                {
                    Debug.LogError("[LeaderBoardPanel] Prefab missing RankScoreView component!");
                    Addressables.Release(handle);
                    yield break;
                }

                rankScoreView.SetRankScore(entry, rank);
                go.SetActive(true);
                m_RankScoreViews.Add(rankScoreView);

                Debug.Log($"[LeaderBoardPanel] ✅ Spawned RankScoreView #{rank} for player {entry.PlayerId} (Score: {entry.Score})");
            }
            else
            {
                Debug.LogError("[LeaderBoardPanel] ❌ Failed to spawn RankScoreView prefab.");
                Addressables.Release(handle);
            }
        }

        // ---------------------------------------------------
        // DISPLAY PLAYER’S OWN SCORE
        // ---------------------------------------------------
        private IEnumerator DisplayMyScoreCoroutine(List<LeaderboardEntry> leaderboard)
        {
            string playerId = UnityService.Instance.PlayerId;
            var playerEntry = leaderboard.FirstOrDefault(e => e.PlayerId == playerId);

            // If not in top ranks, fetch separately
            if (playerEntry == null)
            {
                Debug.Log("[LeaderBoardPanel] Player not in top ranks, fetching individual score...");
                var scoreTask = LeaderboardsService.Instance.GetPlayerScoreAsync(m_LeaderboardId);
                while (!scoreTask.IsCompleted) yield return null;
                playerEntry = scoreTask.Result;
            }

            if (playerEntry != null)
            {
                int rankIndex = leaderboard.FindIndex(e => e.PlayerId == playerId);
                int rank = (rankIndex >= 0) ? rankIndex + 1 : playerEntry.Rank + 1;

                m_MyScoreView.SetRankScore(playerEntry, rank);
                m_MyScoreView.Show();
                Debug.Log($"[LeaderBoardPanel] 👤 Player Rank #{rank} | Score: {playerEntry.Score}");
            }
            else
            {
                Debug.LogWarning("[LeaderBoardPanel] Player score not found.");
                m_MyScoreView.Clear();
                m_MyScoreView.Hide();
            }
        }

        // ---------------------------------------------------
        // WAIT FOR UNITY SERVICE & AUTHENTICATION
        // ---------------------------------------------------
        private IEnumerator WaitForAuthenticationCoroutine()
        {
            // 🟢 Wait for UnityService initialization
            int retries = 0;
            while (!UnityService.Instance.IsInitialized && retries < 50)
            {
                retries++;
                yield return new WaitForSeconds(0.2f);
            }

            if (!UnityService.Instance.IsInitialized)
            {
                Debug.LogError("[LeaderBoardPanel] UnityService failed to initialize in time!");
                yield break;
            }

            Debug.Log("[LeaderBoardPanel] UnityService initialized, checking authentication...");

            // 🟢 Auto sign-in if needed
            if (!AuthenticationService.Instance.IsSignedIn)
            {
                Debug.Log("[LeaderBoardPanel] Player not signed in yet. Attempting anonymous sign-in...");
                var signInTask = AuthenticationService.Instance.SignInAnonymouslyAsync();
                while (!signInTask.IsCompleted) yield return null;
            }

            if (AuthenticationService.Instance.IsSignedIn)
                Debug.Log("[LeaderBoardPanel] Authentication complete.");
            else
                Debug.LogError("[LeaderBoardPanel] Sign-in failed.");
        }
    }
}
