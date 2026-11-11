using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TMPro;
using Unity.Services.Leaderboards.Models;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;

namespace LegionKnight
{
    public partial class LeaderBoardPanel : PanelView
    {
        // Preserve existing serialized paths and prefab references.
        [SerializeField] private AssetReferenceGameObject m_RankScoreViewAsset;
        [SerializeField] private Transform m_Spawn;
        [SerializeField] private List<RankScoreView> m_RankScoreViews = new();
        [SerializeField] private RankScoreView m_MyScoreView;
        [SerializeField, HideInInspector] private string m_LeaderboardId = "highestscorerank";

        private const int c_RequestTimeoutMilliseconds = 15000;
        private const int c_AuthenticationTimeoutMilliseconds = 10000;
        private const int c_AuthenticationPollMilliseconds = 100;
        private const float c_StatusHeight = 155f;
        private const float c_StatusFontSize = 36f;
        private readonly List<AsyncOperationHandle<GameObject>> m_RowHandles = new();
        private CancellationTokenSource m_LoadCancellation;
        private Task m_LoadTask;
        private TextMeshProUGUI m_StatusText;
        public string StatusMessage => m_StatusText != null ? m_StatusText.text : string.Empty;

        protected override void ShowInternal()
        {
            CancelLoadingInternal();
            ClearRowsInternal();
            ReserveOwnRankSpaceInternal();
            base.ShowInternal();
            if (!IsShown || !isActiveAndEnabled) return;
            m_LoadCancellation = new CancellationTokenSource();
            m_LoadTask = RefreshLeaderboardInternal(m_LoadCancellation.Token);
        }

        protected override void HideInternal()
        {
            CancelLoadingInternal();
            ClearRowsInternal();
            SetStatusInternal(string.Empty);
            base.HideInternal();
        }

        private void OnDisable() { CancelLoadingInternal(); }

        private void OnDestroy()
        {
            CancelLoadingInternal();
            ReleaseRowsInternal();
        }

        private void CancelLoadingInternal()
        {
            if (m_LoadCancellation == null) return;
            m_LoadCancellation.Cancel();
            m_LoadCancellation.Dispose();
            m_LoadCancellation = null;
        }

        private void ReserveOwnRankSpaceInternal()
        {
            if (m_Spawn == null || m_MyScoreView == null) return;
            var scroll = m_Spawn.GetComponentInParent<ScrollRect>(true);
            var own = m_MyScoreView.transform as RectTransform;
            if (scroll == null || own == null || !scroll.transform.IsChildOf(transform) || scroll.transform.parent != own.parent) return;
            var rect = scroll.transform as RectTransform;
            if (rect == null || own.anchorMin.y != 0f || own.anchorMax.y != 0f) return;
            // The fixed own-rank footer must sit outside the scrolling leaderboard viewport.
            var bottom = rect.offsetMin;
            bottom.y = Mathf.Max(bottom.y, own.anchoredPosition.y + (1f - own.pivot.y) * own.rect.height);
            rect.offsetMin = bottom;
        }

        private async Task RefreshLeaderboardInternal(CancellationToken token)
        {
            try
            {
                SetStatusInternal("Loading leaderboard...");
                var service = UnityService.Instance;
                int elapsed = 0;
                // Authentication remains owned by the application's existing login flow.
                while (!service.IsLeaderboardReady && elapsed < c_AuthenticationTimeoutMilliseconds)
                {
                    await Task.Delay(c_AuthenticationPollMilliseconds, token);
                    elapsed += c_AuthenticationPollMilliseconds;
                }
                token.ThrowIfCancellationRequested();
                if (!service.IsLeaderboardReady)
                    throw new InvalidOperationException("Sign in and reopen the leaderboard to retry.");

                string playerId = service.LeaderboardPlayerId;
                var leaderboard = await AwaitRequestInternal(service.GetTopRanks(), token);
                EnsureCurrentPlayerInternal(service, playerId, token);
                if (leaderboard == null) throw new InvalidOperationException("The server returned an invalid leaderboard.");

                // Retain the server's ranking, including its ordering of equal scores.
                var entries = leaderboard.Where(entry => entry != null).OrderBy(entry => entry.Rank)
                    .Take(service.MaxRankToDisplay).ToList();
                for (int i = 0; i < entries.Count; i++)
                {
                    var row = i < m_RankScoreViews.Count ? m_RankScoreViews[i] : await SpawnRowInternal(token);
                    EnsureCurrentPlayerInternal(service, playerId, token);
                    if (row == null) throw new InvalidOperationException("A leaderboard row is missing.");
                    row.SetRankScore(entries[i], entries[i].Rank + 1);
                    row.Show();
                }

                SetStatusInternal(entries.Count == 0 ? "No scores yet." : string.Empty);
                try
                {
                    var ownEntry = entries.FirstOrDefault(entry => entry.PlayerId == playerId)
                        ?? await AwaitRequestInternal(service.LoadPlayerLeaderboardScoreAsync(), token);
                    EnsureCurrentPlayerInternal(service, playerId, token);
                    if (m_MyScoreView == null) throw new InvalidOperationException("Your rank view is not configured.");
                    m_MyScoreView.SetRankScore(ownEntry, ownEntry != null ? ownEntry.Rank + 1 : 0);
                    m_MyScoreView.Show();
                }
                catch (OperationCanceledException) when (token.IsCancellationRequested) { throw; }
                catch (Exception exception)
                {
                    EnsureCurrentPlayerInternal(service, playerId, token);
                    SetStatusInternal("Unable to load your rank. Reopen to retry.");
                    Debug.LogWarning("[LeaderBoardPanel] " + exception.Message, this);
                }
            }
            catch (OperationCanceledException) when (token.IsCancellationRequested || !isActiveAndEnabled || !IsShown) { }
            catch (Exception exception)
            {
                if (token.IsCancellationRequested || this == null) return;
                ClearRowsInternal();
                SetStatusInternal("Unable to load leaderboard. Reopen to retry.");
                Debug.LogWarning("[LeaderBoardPanel] " + exception.Message, this);
            }
        }

        private void EnsureCurrentPlayerInternal(UnityService service, string playerId, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();
            if (!isActiveAndEnabled || !IsShown) throw new OperationCanceledException();
            if (!service.IsLeaderboardReady || service.LeaderboardPlayerId != playerId)
                throw new InvalidOperationException("The signed-in account changed. Reopen the leaderboard.");
        }

        private async Task<RankScoreView> SpawnRowInternal(CancellationToken token)
        {
            if (m_RankScoreViewAsset == null || !m_RankScoreViewAsset.RuntimeKeyIsValid() || m_Spawn == null)
                throw new InvalidOperationException("The leaderboard row prefab or parent is not configured.");
            var handle = Addressables.InstantiateAsync(m_RankScoreViewAsset, m_Spawn);
            bool retained = false;
            try
            {
                var instance = await AwaitRequestInternal(handle.Task, token);
                token.ThrowIfCancellationRequested();
                if (!instance.TryGetComponent<RankScoreView>(out var row))
                    row = instance.GetComponentInChildren<RankScoreView>(true);
                if (row == null) throw new InvalidOperationException("The row prefab has no RankScoreView.");
                m_RowHandles.Add(handle);
                m_RankScoreViews.Add(row);
                retained = true;
                return row;
            }
            finally
            {
                // Also releases an in-flight operation canceled by Hide or destruction.
                if (!retained && handle.IsValid()) Addressables.ReleaseInstance(handle);
            }
        }

        private async Task<T> AwaitRequestInternal<T>(Task<T> request, CancellationToken token)
        {
            using (var timer = CancellationTokenSource.CreateLinkedTokenSource(token))
            {
                var timeout = Task.Delay(c_RequestTimeoutMilliseconds, timer.Token);
                if (await Task.WhenAny(request, timeout) != request)
                {
                    // Observe late failures without allowing a stale request to update the UI.
                    _ = request.ContinueWith(task => { var ignored = task.Exception; },
                        CancellationToken.None, TaskContinuationOptions.OnlyOnFaulted | TaskContinuationOptions.ExecuteSynchronously,
                        TaskScheduler.Default);
                    token.ThrowIfCancellationRequested();
                    throw new TimeoutException("Leaderboard request timed out.");
                }
                timer.Cancel();
                var result = await request;
                token.ThrowIfCancellationRequested();
                return result;
            }
        }

        private void ClearRowsInternal()
        {
            foreach (var row in m_RankScoreViews)
            {
                if (row == null) continue;
                row.Clear();
                row.Hide();
            }
            if (m_MyScoreView == null) return;
            m_MyScoreView.Clear();
            m_MyScoreView.Hide();
        }

        private void ReleaseRowsInternal()
        {
            foreach (var handle in m_RowHandles)
                if (handle.IsValid()) Addressables.ReleaseInstance(handle);
            m_RowHandles.Clear();
            m_RankScoreViews.Clear();
        }

        private void SetStatusInternal(string message)
        {
            if (m_StatusText == null && string.IsNullOrEmpty(message)) return;
            if (m_StatusText == null)
            {
                if (m_Spawn == null) return;
                var status = new GameObject("Leaderboard Status", typeof(RectTransform), typeof(LayoutElement), typeof(TextMeshProUGUI));
                status.transform.SetParent(m_Spawn, false);
                status.transform.SetAsFirstSibling();
                status.TryGetComponent<LayoutElement>(out var layout);
                layout.preferredHeight = c_StatusHeight;
                status.TryGetComponent<TextMeshProUGUI>(out m_StatusText);
                var template = m_MyScoreView != null ? m_MyScoreView.GetComponentInChildren<TextMeshProUGUI>(true) : null;
                if (template != null)
                {
                    m_StatusText.font = template.font;
                    m_StatusText.color = template.color;
                }
                m_StatusText.fontSize = c_StatusFontSize;
                m_StatusText.alignment = TextAlignmentOptions.Center;
                m_StatusText.raycastTarget = false;
                m_StatusText.richText = false;
            }
            m_StatusText.text = message;
            m_StatusText.gameObject.SetActive(!string.IsNullOrEmpty(message));
        }
    }
}
