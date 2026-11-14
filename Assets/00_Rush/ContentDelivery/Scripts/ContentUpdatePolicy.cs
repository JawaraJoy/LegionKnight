using System;
using UnityEngine;

namespace Rush.ContentDelivery
{
    [CreateAssetMenu(menuName = "Rush/Content Delivery/Update Policy")]
    public sealed class ContentUpdatePolicy : ScriptableObject
    {
        [Serializable]
        private sealed class ClientField
        {
            [SerializeField] internal string m_CompatibilityId;
            [SerializeField] internal string m_BucketRoot;
            [SerializeField] internal string m_Channel;
            [SerializeField] internal string m_BaselineVersion;
            [SerializeField] internal string m_SceneKey = "Aether_Home (Used)";
            [SerializeField] internal string m_DownloadLabel = "Downloadable";
        }
        [SerializeField] private ClientField m_Client = new();
        public string CompatibilityId => m_Client.m_CompatibilityId;
        public string Channel => m_Client.m_Channel;
        public string BaselineVersion => m_Client.m_BaselineVersion;
        public string SceneKey => m_Client.m_SceneKey;
        public string DownloadLabel => m_Client.m_DownloadLabel;
        public string ManifestUrl => m_Client.m_BucketRoot + "/release_by_badge/" + Channel + "/entry_by_path/content/?path=Android/content.json";
        public bool Valid => ContentReleaseManifest.IsVersion(CompatibilityId) && ContentReleaseManifest.IsVersion(Channel) &&
            ContentReleaseManifest.IsVersion(BaselineVersion) && Uri.TryCreate(m_Client.m_BucketRoot, UriKind.Absolute, out var uri) &&
            uri.Scheme == Uri.UriSchemeHttps && uri.Host.EndsWith(".client-api.unity3dusercontent.com", StringComparison.Ordinal) &&
            !string.IsNullOrEmpty(SceneKey) && !string.IsNullOrEmpty(DownloadLabel);
        public string CatalogUrl(string version) => m_Client.m_BucketRoot + "/release_by_badge/" + version +
            "/entry_by_path/content/?path=Android/catalog_" + version + ".bin";
    }
}
