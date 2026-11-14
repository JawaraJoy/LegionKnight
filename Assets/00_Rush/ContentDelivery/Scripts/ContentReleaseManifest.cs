using System;
using System.Security.Cryptography;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

namespace Rush.ContentDelivery
{
    [Serializable]
    public sealed class ContentReleaseManifest
    {
        private const int c_FormatVersion = 1;
        private const int c_MaxJsonLength = 65536;
        [SerializeField] private int m_FormatVersion;
        [SerializeField] private string m_CompatibilityId;
        [SerializeField] private string m_Version;
        [SerializeField] private string m_CatalogUrl;
        [SerializeField] private string m_CatalogSha256;
        [SerializeField] private string m_CatalogHash;
        [SerializeField] private string m_SceneKey;
        [SerializeField] private string m_DownloadLabel;
        [SerializeField] private string[] m_HeroIds;
        public string CompatibilityId => m_CompatibilityId;
        public string Version => m_Version;
        public string CatalogUrl => m_CatalogUrl;
        public string CatalogSha256 => m_CatalogSha256;
        public string CatalogHash => m_CatalogHash;
        public string SceneKey => m_SceneKey;
        public string DownloadLabel => m_DownloadLabel;
        public string[] HeroIds => m_HeroIds ?? Array.Empty<string>();
        public string Json => JsonUtility.ToJson(this);

        public static ContentReleaseManifest Create(string compatibilityId, string version,
            string catalogUrl, string sha256, string catalogHash, string sceneKey, string label, string[] heroIds = null) => new()
        {
            m_FormatVersion = c_FormatVersion, m_CompatibilityId = compatibilityId, m_Version = version,
            m_CatalogUrl = catalogUrl, m_CatalogSha256 = sha256, m_CatalogHash = catalogHash,
            m_SceneKey = sceneKey, m_DownloadLabel = label, m_HeroIds = heroIds ?? Array.Empty<string>()
        };

        public static ContentReleaseManifest Parse(string json)
        {
            if (string.IsNullOrWhiteSpace(json) || json.Length > c_MaxJsonLength) return null;
            try { return JsonUtility.FromJson<ContentReleaseManifest>(json); }
            catch (ArgumentException) { return null; }
        }

        public bool Matches(ContentUpdatePolicy policy) => policy != null && m_FormatVersion == c_FormatVersion &&
            m_CompatibilityId == policy.CompatibilityId && IsVersion(m_Version) &&
            IsDigest(m_CatalogSha256, 64) && IsDigest(m_CatalogHash, 32) &&
            HeroIds.Length > 0 && !HeroIds.Any(string.IsNullOrEmpty) && HeroIds.Distinct().Count() == HeroIds.Length &&
            m_SceneKey == policy.SceneKey && m_DownloadLabel == policy.DownloadLabel &&
            string.Equals(m_CatalogUrl, policy.CatalogUrl(m_Version), StringComparison.Ordinal);

        public bool RetainsHeroes(ContentReleaseManifest previous) => previous == null || !previous.HeroIds.Except(HeroIds).Any();

        public static bool IsVersion(string value) => value != null && Regex.IsMatch(value, "^[a-z0-9][a-z0-9-]{0,95}$");
        public static bool IsDigest(string value, int length) => value != null && value.Length == length &&
            Regex.IsMatch(value, "^[a-fA-F0-9]+$");
        public static string Digest(byte[] bytes)
        {
            using var hash = SHA256.Create();
            return BitConverter.ToString(hash.ComputeHash(bytes)).Replace("-", string.Empty).ToLowerInvariant();
        }
    }
}
