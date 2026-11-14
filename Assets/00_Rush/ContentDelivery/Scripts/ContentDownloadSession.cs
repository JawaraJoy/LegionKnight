using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceLocations;

namespace Rush.ContentDelivery
{
    /// <summary>Owns each Addressables handle until its status has been inspected.</summary>
    public sealed class ContentDownloadSession
    {
        public string Error { get; private set; }
        public long RequiredBytes { get; private set; }
        public long DownloadedBytes { get; private set; }
        public long TotalBytes { get; private set; }
        public float Progress { get; private set; }
        public bool Succeeded => string.IsNullOrEmpty(Error);

        public IEnumerator Initialize()
        {
            Error = null;
            var handle = Addressables.InitializeAsync(false);
            try
            {
                yield return handle;
                if (handle.Status != AsyncOperationStatus.Succeeded)
                    Error = "Could not prepare game content. Check your connection and retry.";
            }
            finally { if (handle.IsValid()) Addressables.Release(handle); }
        }

        public IEnumerator Inspect(object key)
        {
            Error = null;
            RequiredBytes = 0;
            var handle = Addressables.GetDownloadSizeAsync(key);
            try
            {
                yield return handle;
                if (handle.Status == AsyncOperationStatus.Succeeded) RequiredBytes = handle.Result;
                else Error = "Could not check game content. Check your connection and retry.";
            }
            finally { if (handle.IsValid()) Addressables.Release(handle); }
        }

        public IEnumerator Download(object key, Action<float> onProgress = null)
        {
            Error = null;
            Progress = 0f;
            DownloadedBytes = 0;
            TotalBytes = RequiredBytes;
            // Do not auto-release: status and download statistics must be read first.
            var handle = key is IList<IResourceLocation> locations
                ? Addressables.DownloadDependenciesAsync(locations, false)
                : Addressables.DownloadDependenciesAsync(key, false);
            try
            {
                while (!handle.IsDone)
                {
                    var status = handle.GetDownloadStatus();
                    DownloadedBytes = status.DownloadedBytes;
                    TotalBytes = status.TotalBytes > 0 ? status.TotalBytes : RequiredBytes;
                    Progress = TotalBytes > 0 ? (float)((double)DownloadedBytes / TotalBytes) : 0f;
                    onProgress?.Invoke(Progress);
                    // No short "frozen progress" timer. HTTP timeout/retry is configured on bundles.
                    yield return null;
                }
                if (handle.Status != AsyncOperationStatus.Succeeded)
                    Error = "Download interrupted. Check your connection and tap Retry.";
                else
                {
                    var finalStatus = handle.GetDownloadStatus();
                    if (finalStatus.TotalBytes > 0) TotalBytes = finalStatus.TotalBytes;
                    Progress = 1f;
                    DownloadedBytes = TotalBytes;
                    onProgress?.Invoke(Progress);
                }
            }
            finally { if (handle.IsValid()) Addressables.Release(handle); }

            if (!Succeeded) yield break;
            // A completed handle alone is not enough: verify that no bundles are missing.
            yield return Inspect(key);
            if (Succeeded && RequiredBytes != 0)
                Error = "Some game content is missing. Tap Retry to finish the download.";
        }
    }
}
