#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Build;
using UnityEditor.AddressableAssets.Settings;
using UnityEditor.AddressableAssets.Settings.GroupSchemas;
using UnityEditor.Build.Profile;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.Services.Ccd.Management;
using Unity.Services.Ccd.Management.Models;
using Object = UnityEngine.Object;

namespace Rush.Editor.ContentDelivery
{
    /// <summary>Versioned Android content pipeline. Published badges must stay immutable.</summary>
    public sealed class CcdBuildWorkflow
    {
        private readonly string m_Version;
        private const string c_Environment = "2e648941-3d39-4220-84be-39edc8ca6516";
        private const string c_Bucket = "ddea255c-5b28-4aa6-b031-3e7e043b6e01";
        private const string c_DownloadLabel = "Downloadable";
        private const string c_BootScene = "Assets/00_Rush/ContentDelivery/Scenes/CcdBootstrap.unity";
        private const string c_RemoteScene = "Assets/00_Rush/SceneController/Scenes/Remote/Aether_Home (Used).unity";
        private const string c_Prefab = "Assets/00_Rush/UI/CanvasManager (singleton).prefab";
        private const string c_Evidence = "C:/Users/Pongo/Documents/ChatGPT/AetherJump AI/Fixes/CcdSize100";
        private string RemoteBuildPath => "ServerData/CCD/" + m_Version + "/Android";
        private string RemoteLoadPath => "https://2e84227d-0412-464d-b438-38d319c8f381.client-api.unity3dusercontent.com/client_api/v1/environments/production/buckets/" + c_Bucket + "/release_by_badge/" + m_Version + "/entry_by_path/content/?path=Android";
        private const int c_RequestTimeout = 60;
        private const int c_RetryCount = 2;

        public CcdBuildWorkflow(string version = "size100-v270-20261114")
        {
            if (!Rush.ContentDelivery.ContentReleaseManifest.IsVersion(version)) throw new ArgumentException("Invalid content version.");
            m_Version = version;
        }

        public string Configure()
        {
            var source = SceneManager.GetActiveScene();
            if (source.path != "Assets/00_Rush/SceneController/Scenes/Aether_Home (Used).unity")
                throw new InvalidOperationException("Open the authored Aether_Home scene before preparing its remote copy.");
            Directory.CreateDirectory(Path.GetDirectoryName(c_RemoteScene));
            if (AssetDatabase.LoadAssetAtPath<SceneAsset>(c_BootScene) == null)
                throw new FileNotFoundException("The authored Download Content scene is missing.", c_BootScene);
            if (!EditorSceneManager.SaveScene(source, c_RemoteScene, true))
                throw new IOException("Could not save remote scene copy.");
            AssetDatabase.ImportAsset(c_RemoteScene);

            var settings = AddressableAssetSettingsDefaultObject.Settings;
            var profile = settings.profileSettings.GetProfileId(m_Version);
            if (string.IsNullOrEmpty(profile)) profile = settings.profileSettings.AddProfile(m_Version, settings.activeProfileId);
            settings.activeProfileId = profile;
            settings.profileSettings.SetValue(profile, "Remote.BuildPath", RemoteBuildPath);
            settings.profileSettings.SetValue(profile, "Remote.LoadPath", RemoteLoadPath);
            settings.BuildRemoteCatalog = true;
            settings.DisableCatalogUpdateOnStartup = true;
            settings.CatalogRequestsTimeout = c_RequestTimeout;
            settings.OverridePlayerVersion = m_Version;
            settings.RemoteCatalogBuildPath.SetVariableByName(settings, "Remote.BuildPath");
            settings.RemoteCatalogLoadPath.SetVariableByName(settings, "Remote.LoadPath");
            settings.BuildAddressablesWithPlayerBuild = AddressableAssetSettings.PlayerBuildOption.DoNotBuildWithPlayer;
            settings.AddLabel(c_DownloadLabel);
            var remote = settings.FindGroup("CC");
            if (remote == null) throw new InvalidOperationException("Expected existing CC group.");
            // New shared shader/MonoScript bundles must also be downloadable by an already-installed player.
            settings.DefaultGroup = remote;
            var schema = remote.GetSchema<BundledAssetGroupSchema>();
            schema.UseDefaultSchemaSettings = false;
            schema.BuildPath.SetVariableByName(settings, "Remote.BuildPath");
            schema.LoadPath.SetVariableByName(settings, "Remote.LoadPath");
            schema.Timeout = c_RequestTimeout;
            schema.RetryCount = c_RetryCount;
            schema.UseAssetBundleCache = true;
            schema.UseAssetBundleCrc = true;
            schema.BundleMode = BundledAssetGroupSchema.BundlePackingMode.PackTogether;
            schema.IncludeInBuild = true;
            foreach (var group in settings.groups.Where(g => g != null).ToArray())
                foreach (var entry in group.entries.ToArray())
                {
                    // Built-in scenes/resources are a separate provider, not downloadable entries.
                    if (string.IsNullOrEmpty(entry.AssetPath) || !entry.AssetPath.StartsWith("Assets/")) continue;
                    settings.MoveEntry(entry, remote, false, false);
                    entry.SetLabel(c_DownloadLabel, true);
                }
            var scene = settings.CreateOrMoveEntry(AssetDatabase.AssetPathToGUID(c_RemoteScene), remote);
            scene.address = "Aether_Home (Used)";
            scene.SetLabel(c_DownloadLabel, true);
            settings.SetDirty(AddressableAssetSettings.ModificationEvent.BatchModification, null, true, true);
            foreach (var group in settings.groups.Where(g => g != null))
            {
                EditorUtility.SetDirty(group); AssetDatabase.SaveAssetIfDirty(group);
                foreach (var item in group.Schemas) { EditorUtility.SetDirty(item); AssetDatabase.SaveAssetIfDirty(item); }
            }
            EditorUtility.SetDirty(settings); AssetDatabase.SaveAssetIfDirty(settings);
            var scenes = new[] { new EditorBuildSettingsScene(c_BootScene, true) };
            EditorBuildSettings.scenes = scenes;
            var buildProfile = BuildProfile.GetActiveBuildProfile();
            if (buildProfile != null)
            {
                buildProfile.overrideGlobalScenes = true; buildProfile.scenes = scenes;
                EditorUtility.SetDirty(buildProfile); AssetDatabase.SaveAssetIfDirty(buildProfile);
            }
            var result = "Configured " + remote.entries.Count + " remote entries; original scene dirty=" + source.isDirty;
            File.WriteAllText(c_Evidence + "/configuration.txt", result + "\n" + RemoteLoadPath);
            return result;
        }

        public string AuditUi(bool remove)
        {
            var root = PrefabUtility.LoadPrefabContents(c_Prefab);
            var lines = new List<string>();
            try
            {
                var candidates = new[] {
                    "MainCanvasView/Content/Shop Panel/Content",
                    "MainCanvasView/Content/Shop Detail Panel/Content",
                    "MainCanvasView/Content/IAP Panel/Content",
                    "MainCanvasView/Content/IAP Detail Panel/Content",
                    "MainCanvasView/Content/Daily Watch Ads Panel/Content",
                    "MainCanvasView/Content/Home Panel/Content/Delete All Data ",
                    "MainCanvasView/Content/Home Panel/Content/Control Button"
                };
                foreach (var path in candidates)
                {
                    var branch = root.transform.Find(path);
                    if (branch == null) { lines.Add("ABSENT " + path); continue; }
                    var members = new HashSet<Object>();
                    foreach (var child in branch.GetComponentsInChildren<Transform>(true))
                    {
                        members.Add(child.gameObject);
                        foreach (var component in child.GetComponents<Component>()) if (component != null) members.Add(component);
                    }
                    var inbound = InboundInternal(root.GetComponentsInChildren<Component>(true), members);
                    // Also inspect authored scene overrides before removing any prefab branch.
                    foreach (var sceneRoot in SceneManager.GetActiveScene().GetRootGameObjects())
                    {
                        var match = sceneRoot.GetComponentsInChildren<Transform>(true).FirstOrDefault(t =>
                            PrefabUtility.GetCorrespondingObjectFromSource(t.gameObject) == PrefabUtility.GetCorrespondingObjectFromSource(branch.gameObject));
                        if (match == null) continue;
                        var sceneMembers = new HashSet<Object>();
                        foreach (var child in match.GetComponentsInChildren<Transform>(true))
                        {
                            sceneMembers.Add(child.gameObject);
                            foreach (var component in child.GetComponents<Component>()) if (component != null) sceneMembers.Add(component);
                        }
                        foreach (var sr in SceneManager.GetActiveScene().GetRootGameObjects())
                            inbound.AddRange(InboundInternal(sr.GetComponentsInChildren<Component>(true), sceneMembers));
                    }
                    bool eligible = !branch.gameObject.activeSelf && inbound.Count == 0;
                    lines.Add((eligible ? (remove ? "REMOVED " : "ELIGIBLE ") : "KEPT ") + path + " objects=" + branch.GetComponentsInChildren<Transform>(true).Length + " active=" + branch.gameObject.activeSelf + " references=" + string.Join("; ", inbound));
                    if (remove && eligible) Object.DestroyImmediate(branch.gameObject);
                }
                if (remove) PrefabUtility.SaveAsPrefabAsset(root, c_Prefab);
            }
            finally { PrefabUtility.UnloadPrefabContents(root); }
            var text = string.Join("\n", lines);
            File.WriteAllText(c_Evidence + (remove ? "/ui-cleanup.txt" : "/ui-audit.txt"), text);
            return text;
        }

        private List<string> InboundInternal(IEnumerable<Component> components, HashSet<Object> members)
        {
            var result = new List<string>();
            foreach (var component in components)
            {
                if (component == null || component is Transform || members.Contains(component)) continue;
                using (var serialized = new SerializedObject(component))
                {
                    var property = serialized.GetIterator();
                    while (property.Next(true))
                        if (property.propertyType == SerializedPropertyType.ObjectReference && property.objectReferenceValue != null && members.Contains(property.objectReferenceValue))
                            result.Add(component.name + "." + component.GetType().Name + "." + property.propertyPath);
                }
            }
            return result;
        }

        public string ScheduleContentBuild()
        {
            File.WriteAllText(c_Evidence + "/content-build-status.txt", "Queued");
            EditorApplication.delayCall += () =>
            {
                try
                {
                    File.WriteAllText(c_Evidence + "/content-build-status.txt", "Building");
                    AddressableAssetSettings.BuildPlayerContent(out AddressablesPlayerBuildResult result);
                    File.WriteAllText(c_Evidence + "/content-build-status.txt", string.IsNullOrEmpty(result.Error) ? "Succeeded\n" + result.OutputPath : "Failed\n" + result.Error);
                }
                catch (Exception error) { File.WriteAllText(c_Evidence + "/content-build-status.txt", "Failed\n" + error); Debug.LogException(error); }
            };
            return "Queued remote Android content build";
        }

        public async Task<string> PublishContent(string compatibleChannel = null)
        {
            if (compatibleChannel != null)
            {
                var validation = new CcdUpdateWorkflow();
                validation.ValidateCandidate(m_Version, compatibleChannel);
                await validation.ValidateChannelHistory(m_Version);
            }
            CcdManagement.SetEnvironmentId(c_Environment);
            var api = CcdManagement.Instance;
            var bucketId = Guid.Parse(c_Bucket);
            var files = new DirectoryInfo(RemoteBuildPath).GetFiles().Where(f => f.Extension == ".bundle" || f.Extension == ".bin" || f.Extension == ".json" || f.Extension == ".hash").OrderBy(f => f.Name).ToArray();
            if (!files.Any(f => f.Name.StartsWith("catalog_"))) throw new IOException("Remote catalog is missing.");
            // Refuse to mutate a badge already used by a shipped binary.
            for (long page = 1; ; page++)
            {
                var badges = await api.ListBadgesAsync(bucketId, new PageOptions { Page = page, PerPage = 100 });
                if (badges.Any(b => b.Name == m_Version)) throw new InvalidOperationException("Badge already exists. Choose a fresh content version.");
                if (badges.Count < 100) break;
            }
            if (compatibleChannel != null && (!Rush.ContentDelivery.ContentReleaseManifest.IsVersion(compatibleChannel) || compatibleChannel == m_Version))
                throw new ArgumentException("Invalid compatible channel.");
            var entries = new List<CcdReleaseEntryCreate>();
            for (int index = 0; index < files.Length; index++)
            {
                var file = files[index];
                File.WriteAllText(c_Evidence + "/publish-status.txt", "Uploading " + (index + 1) + "/" + files.Length + " " + file.Name);
                string hash;
                using (var md5 = System.Security.Cryptography.MD5.Create())
                using (var source = file.OpenRead()) hash = BitConverter.ToString(md5.ComputeHash(source)).Replace("-", "").ToLowerInvariant();
                using (var source = file.OpenRead())
                {
                    var options = new EntryModelOptions("Android/" + file.Name, hash, (int)file.Length) { UpdateIfExists = true };
                    var entry = await api.CreateOrUpdateEntryByPathAsync(new EntryByPathOptions(bucketId, "Android/" + file.Name), options);
                    await api.UploadContentAsync(new UploadContentOptions(bucketId, entry.Entryid, source));
                    entries.Add(new CcdReleaseEntryCreate(entry.Entryid, entry.CurrentVersionid));
                }
            }
            var release = await api.CreateReleaseAsync(new CreateReleaseOptions(bucketId) { Entries = entries, Notes = m_Version + ": original texture quality; bootstrap AAB; preserved ARMv7 and ARM64." });
            await api.AssignBadgeAsync(new AssignBadgeOptions(bucketId, m_Version, release.Releaseid));
            if (compatibleChannel != null) await api.AssignBadgeAsync(new AssignBadgeOptions(bucketId, compatibleChannel, release.Releaseid));
            var result = "Published release=" + release.Releaseid + " badge=" + m_Version + " channel=" + compatibleChannel + " bucket=" + c_Bucket + " files=" + files.Length + " bytes=" + files.Sum(f => f.Length);
            File.WriteAllText(c_Evidence + "/publish-status.txt", result);
            return result;
        }
    }
}
#endif
