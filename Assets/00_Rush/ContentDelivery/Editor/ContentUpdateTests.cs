#if UNITY_EDITOR
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using NUnit.Framework;
using Rush.ContentDelivery;
using Rush.Editor.ContentDelivery;
using UnityEditor;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.AddressableAssets.ResourceLocators;
using UnityEngine.ResourceManagement.ResourceLocations;

namespace Rush.Tests
{
    public sealed class ContentUpdateTests
    {
        private ContentUpdatePolicy m_Policy;
        private string m_Temporary;
        private ContentReleaseStore m_Store;
        private byte[] m_Catalog;
        private ContentReleaseManifest m_Release;
        private IResourceLocator[] m_OriginalLocators;

        [SetUp]
        public void Before()
        {
            m_Policy = ScriptableObject.CreateInstance<ContentUpdatePolicy>();
            using (var serialized = new SerializedObject(m_Policy))
            {
                var client = serialized.FindProperty("m_Client");
                client.FindPropertyRelative("m_CompatibilityId").stringValue = "android-test";
                client.FindPropertyRelative("m_Channel").stringValue = "compatible-android-test";
                client.FindPropertyRelative("m_BaselineVersion").stringValue = "content-base";
                client.FindPropertyRelative("m_BucketRoot").stringValue = "https://test.client-api.unity3dusercontent.com/client_api/v1/environments/production/buckets/test";
                serialized.ApplyModifiedPropertiesWithoutUndo();
            }
            m_Temporary = Path.Combine(Path.GetTempPath(), "RushCcdTests", Guid.NewGuid().ToString("N"));
            m_Store = new ContentReleaseStore(m_Policy, m_Temporary);
            m_Catalog = Encoding.UTF8.GetBytes("verified-test-catalog");
            m_Release = ReleaseInternal("content-first", m_Catalog);
            m_OriginalLocators = Addressables.ResourceLocators.ToArray();
        }
        [TearDown]
        public void After()
        {
            Addressables.ClearResourceLocators();
            foreach (var locator in m_OriginalLocators) Addressables.AddResourceLocator(locator);
            UnityEngine.Object.DestroyImmediate(m_Policy);
            var root = Path.GetFullPath(Path.Combine(Path.GetTempPath(), "RushCcdTests")) + Path.DirectorySeparatorChar;
            var target = Path.GetFullPath(m_Temporary);
            if (!target.StartsWith(root, StringComparison.OrdinalIgnoreCase)) throw new InvalidOperationException("Unexpected test directory.");
            if (Directory.Exists(target)) Directory.Delete(target, true);
        }

        private ContentReleaseManifest ReleaseInternal(string version, byte[] bytes) => ContentReleaseManifest.Create(
            m_Policy.CompatibilityId, version, m_Policy.CatalogUrl(version), ContentReleaseManifest.Digest(bytes),
            new string('a', 32), m_Policy.SceneKey, m_Policy.DownloadLabel, new[] { "allain" });

        [Test] public void CompatibleManifestRoundTrips() => Assert.That(ContentReleaseManifest.Parse(m_Release.Json).Matches(m_Policy), Is.True);
        [TestCase("")][TestCase("not-json")][TestCase("null")]
        public void MalformedManifestIsRejected(string value) => Assert.That(ContentReleaseManifest.Parse(value), Is.Null);
        [Test] public void DifferentClientLineIsRejected()
        {
            var release = ContentReleaseManifest.Create("android-other", m_Release.Version, m_Release.CatalogUrl,
                m_Release.CatalogSha256, m_Release.CatalogHash, m_Release.SceneKey, m_Release.DownloadLabel, m_Release.HeroIds);
            Assert.That(release.Matches(m_Policy), Is.False);
        }
        [Test] public void CatalogCannotRedirectToAnotherBucket()
        {
            var release = ContentReleaseManifest.Create(m_Release.CompatibilityId, m_Release.Version,
                "https://other.client-api.unity3dusercontent.com/catalog.bin", m_Release.CatalogSha256, m_Release.CatalogHash,
                m_Release.SceneKey, m_Release.DownloadLabel, m_Release.HeroIds);
            Assert.That(release.Matches(m_Policy), Is.False);
        }
        [Test] public void StageDoesNotReplaceLastGood()
        {
            m_Store.Stage(m_Release, m_Catalog); m_Store.Commit(m_Release);
            var next = ReleaseInternal("content-next", Encoding.UTF8.GetBytes("new"));
            m_Store.Stage(next, Encoding.UTF8.GetBytes("new"));
            Assert.That(m_Store.LastGood.Version, Is.EqualTo(m_Release.Version));
        }
        [Test] public void CorruptDownloadCannotCommit()
        {
            m_Store.Stage(m_Release, Encoding.UTF8.GetBytes("damaged")); m_Store.Commit(m_Release);
            Assert.That(m_Store.LastGood, Is.Null); Assert.That(m_Store.Error, Is.Not.Null);
        }
        [Test] public void CachedCatalogCorruptionIsDetected()
        {
            m_Store.Stage(m_Release, m_Catalog); m_Store.Commit(m_Release);
            File.WriteAllText(m_Store.CatalogPath(m_Release), "damaged");
            Assert.That(m_Store.HasCatalog(m_Release), Is.False); Assert.That(m_Store.LastGood, Is.Null);
        }
        [Test] public void InterruptedPointerWriteRestoresPreviousGoodVersion()
        {
            m_Store.Stage(m_Release, m_Catalog); m_Store.Commit(m_Release);
            var bytes = Encoding.UTF8.GetBytes("next"); var next = ReleaseInternal("content-next", bytes);
            m_Store.Stage(next, bytes); m_Store.Commit(next);
            var pointer = Path.Combine(m_Temporary, "ccd-content", m_Policy.CompatibilityId, "last-good.json");
            File.WriteAllText(pointer, "partial-json");
            Assert.That(m_Store.LastGood.Version, Is.EqualTo(m_Release.Version));
        }
        [Test] public void AddedHeroWithKnownTypesIsCompatible() =>
            Assert.That(CcdUpdateWorkflow.CompatibilityError(new[] { "Hero", "Sprite" }, new[] { "allain", "new-hero" },
                new[] { "Hero", "Sprite" }, new[] { "allain" }), Is.Null);
        [Test] public void NewRuntimeTypeRequiresPlayerBuild() =>
            Assert.That(CcdUpdateWorkflow.CompatibilityError(new[] { "Hero", "NewAbility" }, new[] { "allain" },
                new[] { "Hero" }, new[] { "allain" }), Does.Contain("NewAbility"));
        [Test] public void ExistingSavedHeroCannotBeRemoved() =>
            Assert.That(CcdUpdateWorkflow.CompatibilityError(new[] { "Hero" }, new[] { "new-hero" },
                new[] { "Hero" }, new[] { "allain" }), Does.Contain("allain"));
        [Test] public void ClientRejectsRollbackThatRemovesDownloadedHero()
        {
            var newer = ContentReleaseManifest.Create(m_Release.CompatibilityId, "content-newer", m_Policy.CatalogUrl("content-newer"),
                m_Release.CatalogSha256, m_Release.CatalogHash, m_Release.SceneKey, m_Release.DownloadLabel, new[] { "allain", "new-hero" });
            Assert.That(m_Release.RetainsHeroes(newer), Is.False);
            Assert.That(newer.RetainsHeroes(m_Release), Is.True);
        }
        [Test] public void CatalogSwitchHasOneSceneKeyAndCanRollback()
        {
            var old = new ResourceLocationMap("test-original");
            old.Add("scene", new ResourceLocationBase("scene", "old-scene", "provider", typeof(UnityEngine.Object)));
            var next = new ResourceLocationMap("test-candidate");
            next.Add("scene", new ResourceLocationBase("scene", "new-scene", "provider", typeof(UnityEngine.Object)));
            Addressables.ClearResourceLocators(); Addressables.AddResourceLocator(old);
            using var selected = new ContentCatalogSelection(m_Release);
            typeof(ContentCatalogSelection).GetField("m_Locator", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(selected, next);
            selected.Activate();
            Assert.That(Addressables.ResourceLocators.Contains(old), Is.False);
            Assert.That(Addressables.ResourceLocators.Contains(next), Is.True);
            selected.Rollback();
            Assert.That(Addressables.ResourceLocators.Contains(old), Is.True);
            Assert.That(Addressables.ResourceLocators.Contains(next), Is.False);
        }
    }
}
#endif
