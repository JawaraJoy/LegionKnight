using System;
using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace Rush.ContentDelivery
{
    /// <summary>Checks one compatible channel before loading the game; never replaces a live scene catalog.</summary>
    public sealed class CcdContentUpdater : IDisposable
    {
        private const int c_RequestTimeout = 15;
        private const int c_MaxCatalogBytes = 8388608;
        private const int c_MaxManifestBytes = 65536;
        private readonly ContentUpdatePolicy m_Policy;
        private readonly ContentReleaseStore m_Store;
        private ContentReleaseManifest m_LastGood;
        private ContentCatalogSelection m_Selection;
        private byte[] m_Response;
        public ContentCatalogSelection Selection => m_Selection;
        public string Version => m_Selection?.Manifest.Version ?? m_Policy?.BaselineVersion;
        public object DownloadKey => m_Selection?.Downloads ?? (object)m_Policy.DownloadLabel;

        public CcdContentUpdater(ContentUpdatePolicy policy, string persistentPath)
        {
            m_Policy = policy;
            if (policy != null && policy.Valid) m_Store = new ContentReleaseStore(policy, persistentPath);
        }

        public IEnumerator Check()
        {
            if (m_Store == null) yield break;
            m_LastGood = m_Store.LastGood;
            yield return FetchInternal(m_Policy.ManifestUrl, c_MaxManifestBytes);
            var latest = m_Response == null ? null : ContentReleaseManifest.Parse(Encoding.UTF8.GetString(m_Response));
            if (latest != null && latest.Matches(m_Policy) && latest.RetainsHeroes(m_LastGood))
            {
                yield return SelectInternal(latest, true);
                if (m_Selection != null) yield break;
            }
            if (m_LastGood != null) yield return SelectInternal(m_LastGood, false);
        }

        private IEnumerator FetchInternal(string url, int maxBytes)
        {
            m_Response = null;
            using var request = UnityWebRequest.Get(url);
            request.timeout = c_RequestTimeout;
            request.SetRequestHeader("Cache-Control", "no-cache");
            var operation = request.SendWebRequest();
            while (!operation.isDone)
            {
                if (request.downloadedBytes > (ulong)maxBytes) { request.Abort(); yield break; }
                yield return null;
            }
            if (request.result == UnityWebRequest.Result.Success && request.downloadHandler.data.Length <= maxBytes)
                m_Response = request.downloadHandler.data;
        }

        private IEnumerator SelectInternal(ContentReleaseManifest manifest, bool allowNetwork)
        {
            if (!m_Store.HasCatalog(manifest) && allowNetwork)
            {
                yield return FetchInternal(manifest.CatalogUrl, c_MaxCatalogBytes);
                if (m_Response == null) yield break;
                m_Store.Stage(manifest, m_Response);
            }
            if (!m_Store.HasCatalog(manifest)) yield break;
            var candidate = new ContentCatalogSelection(manifest);
            yield return candidate.Load(m_Store.CatalogPath(manifest), m_Policy.SceneKey, m_Policy.DownloadLabel);
            if (candidate.Error != null) { candidate.Dispose(); yield break; }
            m_Selection?.Dispose();
            m_Selection = candidate;
        }

        public IEnumerator UseCachedFallback()
        {
            m_Selection?.Dispose(); m_Selection = null;
            if (m_Store == null || m_LastGood == null) yield break;
            yield return SelectInternal(m_LastGood, false);
            if (m_Selection == null) yield break;
            var check = new ContentDownloadSession();
            yield return check.Inspect(m_Selection.Downloads);
            if (!check.Succeeded || check.RequiredBytes > 0) { m_Selection.Dispose(); m_Selection = null; }
        }

        public void Activate() => m_Selection?.Activate();
        public void UseBaseline()
        {
            m_Selection?.Dispose();
            m_Selection = null;
        }
        public void MarkReady()
        {
            if (m_Selection == null) return;
            m_Selection.MarkReady();
            m_Store.Commit(m_Selection.Manifest);
            if (m_Store.Error != null) Debug.LogWarning(m_Store.Error);
        }
        public void Dispose() => m_Selection?.Dispose();
    }
}
