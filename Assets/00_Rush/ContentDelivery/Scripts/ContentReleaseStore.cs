using System;
using System.IO;

namespace Rush.ContentDelivery
{
    /// <summary>The last-good pointer is separate from staged files and survives an interrupted write.</summary>
    public sealed class ContentReleaseStore
    {
        private readonly ContentUpdatePolicy m_Policy;
        private readonly string m_Root;
        public string Error { get; private set; }
        private string LastGoodPath => Path.Combine(m_Root, "last-good.json");

        public ContentReleaseStore(ContentUpdatePolicy policy, string persistentPath)
        {
            m_Policy = policy;
            m_Root = Path.Combine(persistentPath, "ccd-content", policy.CompatibilityId);
        }

        public string CatalogPath(ContentReleaseManifest manifest) => Path.Combine(m_Root, manifest.CatalogSha256, "catalog.bin");
        public bool HasCatalog(ContentReleaseManifest manifest)
        {
            if (manifest == null || !manifest.Matches(m_Policy)) return false;
            try
            {
                var path = CatalogPath(manifest);
                return File.Exists(path) && ContentReleaseManifest.Digest(File.ReadAllBytes(path)) == manifest.CatalogSha256.ToLowerInvariant();
            }
            catch (Exception error) when (error is IOException || error is UnauthorizedAccessException) { return false; }
        }

        public ContentReleaseManifest LastGood => ReadInternal(LastGoodPath) ?? ReadInternal(LastGoodPath + ".bak");

        private ContentReleaseManifest ReadInternal(string path)
        {
            try
            {
                if (!File.Exists(path)) return null;
                var manifest = ContentReleaseManifest.Parse(File.ReadAllText(path));
                return HasCatalog(manifest) ? manifest : null;
            }
            catch (Exception error) when (error is IOException || error is UnauthorizedAccessException) { return null; }
        }

        public void Stage(ContentReleaseManifest manifest, byte[] bytes)
        {
            Error = null;
            if (manifest == null || !manifest.Matches(m_Policy) || bytes == null ||
                ContentReleaseManifest.Digest(bytes) != manifest.CatalogSha256.ToLowerInvariant())
            { Error = "The content catalog did not pass verification."; return; }
            try
            {
                var path = CatalogPath(manifest);
                Directory.CreateDirectory(Path.GetDirectoryName(path));
                if (!HasCatalog(manifest))
                {
                    File.WriteAllBytes(path + ".tmp", bytes);
                    if (File.Exists(path)) File.Delete(path);
                    File.Move(path + ".tmp", path);
                }
                File.WriteAllText(Path.ChangeExtension(path, ".hash"), manifest.CatalogHash);
            }
            catch (Exception error) when (error is IOException || error is UnauthorizedAccessException)
            { Error = "Cannot save game content. Free some storage and retry."; }
        }

        public void Commit(ContentReleaseManifest manifest)
        {
            Error = null;
            if (!HasCatalog(manifest)) { Error = "The content catalog is not ready."; return; }
            try
            {
                File.WriteAllText(LastGoodPath + ".tmp", manifest.Json);
                if (ReadInternal(LastGoodPath) != null) File.Copy(LastGoodPath, LastGoodPath + ".bak", true);
                if (File.Exists(LastGoodPath)) File.Delete(LastGoodPath);
                File.Move(LastGoodPath + ".tmp", LastGoodPath);
            }
            catch (Exception error) when (error is IOException || error is UnauthorizedAccessException)
            { Error = "Could not remember the content version on this device."; }
        }
    }
}
