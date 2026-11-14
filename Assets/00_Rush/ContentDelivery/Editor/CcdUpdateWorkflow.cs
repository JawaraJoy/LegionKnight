#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;
using LegionKnight;
using Rush.ContentDelivery;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Build;
using UnityEditor.AddressableAssets.Settings;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEditor.Compilation;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;
using UnityEngine.AddressableAssets.ResourceLocators;
using UnityEngine.ResourceManagement.ResourceLocations;
using UnityEngine.ResourceManagement.ResourceProviders;
using Object = UnityEngine.Object;

namespace Rush.Editor.ContentDelivery
{
    public sealed class CcdUpdateWorkflow
    {
        public const string PolicyPath = "Assets/00_Rush/ContentDelivery/Config/ContentUpdatePolicy.asset";
        private const string c_Boot = "Assets/00_Rush/ContentDelivery/Scenes/CcdBootstrap.unity";
        private const string c_RemoteScene = "Assets/00_Rush/SceneController/Scenes/Remote/Aether_Home (Used).unity";
        private const string c_Contracts = "Assets/00_Rush/ContentDelivery/Editor/Compatibility";
        private const string c_BucketRoot = "https://2e84227d-0412-464d-b438-38d319c8f381.client-api.unity3dusercontent.com/client_api/v1/environments/production/buckets/ddea255c-5b28-4aa6-b031-3e7e043b6e01";
        private const long c_MaxAabBytes = 100000000;
        private const string c_Status = "Logs/CCD/live-update-status.txt";
        [Serializable] private sealed class ContractField
        {
            [SerializeField] internal string m_Fingerprint;
            [SerializeField] internal string m_CompatibilityId;
            [SerializeField] internal string m_Baseline;
            [SerializeField] internal string[] m_Types;
            [SerializeField] internal string[] m_HeroIds;
            [SerializeField] internal string m_ValidatedAab;
            [SerializeField] internal long m_AabBytes;
        }
        [Serializable] private sealed class CandidateField
        {
            [SerializeField] internal string m_Fingerprint;
            [SerializeField] internal string m_DependencyHash;
            [SerializeField] internal string m_ManifestSha256;
            [SerializeField] internal string[] m_Types;
            [SerializeField] internal string[] m_HeroIds;
        }
        private sealed class ShapeField
        {
            internal readonly HashSet<string> m_Types = new();
            internal readonly HashSet<string> m_HeroIds = new();
        }
        public string Report { get; private set; }
        public ContentUpdatePolicy Policy => AssetDatabase.LoadAssetAtPath<ContentUpdatePolicy>(PolicyPath);
        public string Fingerprint
        {
            get
            {
                var assemblies = CompilationPipeline.GetAssemblies(AssembliesType.Player);
                var names = new HashSet<string>(assemblies.Select(a => a.name));
                var digests = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                string DigestFile(string path)
                {
                    var absolute = Path.GetFullPath(path);
                    if (!digests.TryGetValue(absolute, out var value))
                        digests[absolute] = value = ContentReleaseManifest.Digest(File.ReadAllBytes(absolute));
                    return value;
                }
                var lines = new List<string> { Application.unityVersion, "content-protocol-1", EditorUserBuildSettings.activeBuildTarget.ToString(),
                    PlayerSettings.GetScriptingDefineSymbols(NamedBuildTarget.Android), PlayerSettings.Android.targetArchitectures.ToString(),
                    PlayerSettings.GetScriptingBackend(NamedBuildTarget.Android).ToString(), EditorUserBuildSettings.androidBuildSubtarget.ToString(),
                    PlayerSettings.colorSpace.ToString(), string.Join(",", PlayerSettings.GetGraphicsAPIs(BuildTarget.Android)) };
                foreach (var a in assemblies.OrderBy(a => a.name))
                {
                    lines.Add(a.name);
                    foreach (var path in a.sourceFiles.OrderBy(p => p, StringComparer.Ordinal))
                        lines.Add(path.Replace('\\','/') + ":" + DigestFile(path));
                    foreach (var path in a.compiledAssemblyReferences.OrderBy(p => p, StringComparer.Ordinal))
                        if (!names.Contains(Path.GetFileNameWithoutExtension(path)) && File.Exists(path))
                            lines.Add(Path.GetFileName(path) + ":" + DigestFile(path));
                }
                lines.Add(DigestFile("Packages/manifest.json"));
                if (File.Exists("Packages/packages-lock.json")) lines.Add(DigestFile("Packages/packages-lock.json"));
                foreach (var plugin in PluginImporter.GetAllImporters().Where(p => p.GetCompatibleWithPlatform(BuildTarget.Android)).OrderBy(p => p.assetPath))
                    if (File.Exists(plugin.assetPath)) lines.Add(plugin.assetPath + ":" + DigestFile(plugin.assetPath));
                return ContentReleaseManifest.Digest(Encoding.UTF8.GetBytes(string.Join("\n", lines)));
            }
        }
        private string ContractPath => Path.Combine(c_Contracts, Policy.CompatibilityId + ".json");
        private string ContentPath(string version) => "ServerData/CCD/" + version + "/Android";

        public void PreparePlayer(string version, int versionCode)
        {
            RequireAndroidInternal();
            if (!ContentReleaseManifest.IsVersion(version)) throw new ArgumentException("Invalid content version.");
            var fingerprint = Fingerprint;
            var id = "android-" + fingerprint.Substring(0, 12) + "-" + versionCode;
            var newContractPath = Path.Combine(c_Contracts, id + ".json");
            if (File.Exists(newContractPath) && !string.IsNullOrEmpty(JsonUtility.FromJson<ContractField>(File.ReadAllText(newContractPath)).m_ValidatedAab))
                throw new InvalidOperationException("This player code/version already has a validated baseline. Use compatible content updates or a new player version code.");
            Directory.CreateDirectory(Path.GetDirectoryName(PolicyPath));
            Directory.CreateDirectory(c_Contracts);
            var policy = Policy;
            if (policy == null) { policy = ScriptableObject.CreateInstance<ContentUpdatePolicy>(); AssetDatabase.CreateAsset(policy, PolicyPath); }
            using (var serialized = new SerializedObject(policy))
            {
                var client = serialized.FindProperty("m_Client");
                client.FindPropertyRelative("m_CompatibilityId").stringValue = id;
                client.FindPropertyRelative("m_Channel").stringValue = "compatible-" + id;
                client.FindPropertyRelative("m_BucketRoot").stringValue = c_BucketRoot;
                client.FindPropertyRelative("m_BaselineVersion").stringValue = version;
                serialized.ApplyModifiedPropertiesWithoutUndo();
            }
            AssetDatabase.SaveAssetIfDirty(policy);
            AttachPolicyInternal(policy);
            new CcdBuildWorkflow(version).Configure();
            PrepareRemoteSceneInternal();
            var shape = ShapeInternal();
            var contract = new ContractField { m_Fingerprint = fingerprint, m_CompatibilityId = id, m_Baseline = version,
                m_Types = shape.m_Types.OrderBy(x => x).ToArray(), m_HeroIds = shape.m_HeroIds.OrderBy(x => x).ToArray() };
            if (File.Exists(ContractPath))
            {
                var old = ReadContractInternal();
                if (!string.IsNullOrEmpty(old.m_ValidatedAab))
                    throw new InvalidOperationException("This code line already has a validated player. Use compatible content updates, or explicitly create a new runtime code line.");
            }
            File.WriteAllText(ContractPath, JsonUtility.ToJson(contract, true));
            AssetDatabase.ImportAsset(ContractPath);
            PlayerSettings.Android.bundleVersionCode = versionCode;
            Report = "Prepared player " + versionCode + ", compatibility=" + id;
        }

        public void PrepareCompatibleContent(string version)
        {
            ValidateCodeInternal();
            new CcdBuildWorkflow(version).Configure();
            PrepareRemoteSceneInternal();
            ValidateShapeInternal(ShapeInternal(), ReadContractInternal());
            Report = "Prepared compatible content " + version;
        }

        public void BuildContent(string version)
        {
            ValidateCodeInternal();
            var settings = AddressableAssetSettingsDefaultObject.Settings;
            if (settings.OverridePlayerVersion != version) throw new InvalidOperationException("Prepare this content version first.");
            var shape = ShapeInternal();
            ValidateShapeInternal(shape, ReadContractInternal());
            AddressableAssetSettings.BuildPlayerContent(out AddressablesPlayerBuildResult result);
            if (!string.IsNullOrEmpty(result.Error)) throw new BuildFailedException(result.Error);
            var folder = ContentPath(version);
            var catalog = Path.Combine(folder, "catalog_" + version + ".bin");
            var hash = File.ReadAllText(Path.ChangeExtension(catalog, ".hash")).Trim();
            var manifest = ContentReleaseManifest.Create(Policy.CompatibilityId, version, Policy.CatalogUrl(version),
                ContentReleaseManifest.Digest(File.ReadAllBytes(catalog)), hash, Policy.SceneKey, Policy.DownloadLabel, shape.m_HeroIds.OrderBy(x => x).ToArray());
            if (!manifest.Matches(Policy)) throw new BuildFailedException("Invalid content manifest.");
            File.WriteAllText(Path.Combine(folder, "content.json"), manifest.Json);
            var candidate = new CandidateField { m_Fingerprint = Fingerprint, m_Types = shape.m_Types.OrderBy(x => x).ToArray(),
                m_HeroIds = shape.m_HeroIds.OrderBy(x => x).ToArray(), m_DependencyHash = DependencyHashInternal(),
                m_ManifestSha256 = ContentReleaseManifest.Digest(Encoding.UTF8.GetBytes(manifest.Json)) };
            // Kept outside the upload folder: server clients do not need the compatibility audit data.
            File.WriteAllText(Path.Combine(Path.GetDirectoryName(folder), "candidate.json"), JsonUtility.ToJson(candidate, true));
            Report = "Built " + version + "; types=" + candidate.m_Types.Length + "; heroes=" + candidate.m_HeroIds.Length;
        }

        public void ScheduleBuild(string version)
        {
            Directory.CreateDirectory("Logs/CCD");
            File.WriteAllText(c_Status, "Queued " + version);
            EditorApplication.CallbackFunction run = null;
            run = () =>
            {
                EditorApplication.update -= run;
                try { File.WriteAllText(c_Status, "Building " + version); BuildContent(version); File.WriteAllText(c_Status, "Succeeded " + Report); }
                catch (Exception error) { File.WriteAllText(c_Status, "Failed " + error); Debug.LogException(error); }
            };
            EditorApplication.update += run;
        }

        public async Task<string> Publish(string version)
        {
            ValidateCandidate(version, Policy.Channel);
            var result = await new CcdBuildWorkflow(version).PublishContent(Policy.Channel);
            var contract = ReadContractInternal();
            var published = ContentReleaseManifest.Parse(File.ReadAllText(Path.Combine(ContentPath(version), "content.json")));
            contract.m_HeroIds = contract.m_HeroIds.Union(published.HeroIds).OrderBy(x => x).ToArray();
            File.WriteAllText(ContractPath, JsonUtility.ToJson(contract, true));
            Directory.CreateDirectory("Logs/CCD");
            File.WriteAllText("Logs/CCD/last-live-publication.txt", result);
            return result;
        }

        public void ValidateCandidate(string version, string channel)
        {
            ValidateCodeInternal();
            if (channel != Policy.Channel) throw new InvalidOperationException("Unexpected content compatibility channel.");
            var contract = ReadContractInternal();
            if (string.IsNullOrEmpty(contract.m_ValidatedAab) || contract.m_AabBytes <= 0 || contract.m_AabBytes >= c_MaxAabBytes)
                throw new InvalidOperationException("Build and validate the initial AAB for this compatibility line before publishing content.");
            var folder = ContentPath(version);
            var candidate = JsonUtility.FromJson<CandidateField>(File.ReadAllText(Path.Combine(Path.GetDirectoryName(folder), "candidate.json")));
            var json = File.ReadAllText(Path.Combine(folder, "content.json"));
            var manifest = ContentReleaseManifest.Parse(json);
            if (candidate.m_Fingerprint != contract.m_Fingerprint || candidate.m_ManifestSha256 != ContentReleaseManifest.Digest(Encoding.UTF8.GetBytes(json)) ||
                manifest == null || !manifest.Matches(Policy) || manifest.Version != version ||
                manifest.CatalogSha256 != ContentReleaseManifest.Digest(File.ReadAllBytes(Path.Combine(folder, "catalog_" + version + ".bin"))))
                throw new InvalidOperationException("Candidate artifacts no longer match their compatibility audit.");
            if (candidate.m_DependencyHash != DependencyHashInternal()) throw new InvalidOperationException("Assets changed after the content build. Rebuild before publishing.");
            ValidateSets(candidate.m_Types, candidate.m_HeroIds, contract.m_Types, contract.m_HeroIds);
            if (version != Policy.BaselineVersion && LocalBundlesInternal(version).Length > 0)
                throw new InvalidOperationException("Compatible content contains local bundle dependencies. Shared and content bundles must be remote.");
        }

        public async Task ValidateChannelHistory(string version)
        {
            var next = ContentReleaseManifest.Parse(File.ReadAllText(Path.Combine(ContentPath(version), "content.json")));
            using var request = UnityWebRequest.Get(Policy.ManifestUrl);
            request.timeout = 30;
            request.SetRequestHeader("Cache-Control", "no-cache");
            var operation = request.SendWebRequest();
            var completed = new TaskCompletionSource<bool>();
            operation.completed += _ => completed.TrySetResult(true);
            EditorApplication.CallbackFunction pump = () => EditorApplication.QueuePlayerLoopUpdate();
            EditorApplication.update += pump;
            try { if (!operation.isDone) await completed.Task; }
            finally { EditorApplication.update -= pump; }
            if (request.responseCode == (long)HttpStatusCode.NotFound) return;
            if (request.result != UnityWebRequest.Result.Success) throw new IOException("Cannot verify current channel history: " + request.error);
            var previous = ContentReleaseManifest.Parse(request.downloadHandler.text);
            if (previous == null || !previous.Matches(Policy) || !next.RetainsHeroes(previous))
                throw new InvalidOperationException("The channel update would remove published hero IDs or replace another compatibility line.");
        }

        public void ValidatePlayerBuild()
        {
            ValidateCodeInternal();
            var settings = AddressableAssetSettingsDefaultObject.Settings;
            if (settings.OverridePlayerVersion != Policy.BaselineVersion)
                throw new BuildFailedException("Use the frozen baseline catalog when rebuilding this player, or create a new code line for changed runtime code.");
        }

        public void RecordPlayerBuild(string path)
        {
            var bytes = new FileInfo(path).Length;
            if (bytes >= c_MaxAabBytes) throw new BuildFailedException("AetherJump AAB must be below 100,000,000 bytes. Actual=" + bytes);
            var contract = ReadContractInternal();
            contract.m_ValidatedAab = Path.GetFullPath(path);
            contract.m_AabBytes = bytes;
            File.WriteAllText(ContractPath, JsonUtility.ToJson(contract, true));
        }

        public static string CompatibilityError(string[] candidateTypes, string[] candidateHeroes, string[] knownTypes, string[] knownHeroes)
        {
            var missing = candidateTypes.Except(knownTypes).ToArray();
            if (missing.Length > 0) return "Content uses types absent from the frozen player: " + string.Join(", ", missing);
            var removed = knownHeroes.Except(candidateHeroes).ToArray();
            if (removed.Length > 0) return "Content removes saved hero IDs: " + string.Join(", ", removed);
            return null;
        }
        private void ValidateSets(string[] types, string[] heroes, string[] knownTypes, string[] knownHeroes)
        {
            var error = CompatibilityError(types, heroes, knownTypes, knownHeroes);
            if (error != null) throw new InvalidOperationException(error + ". A new player build is required.");
        }
        private void ValidateShapeInternal(ShapeField shape, ContractField contract) =>
            ValidateSets(shape.m_Types.ToArray(), shape.m_HeroIds.ToArray(), contract.m_Types, contract.m_HeroIds);
        private void RequireAndroidInternal()
        {
            if (EditorUserBuildSettings.activeBuildTarget != BuildTarget.Android) throw new InvalidOperationException("Select Android before preparing CCD content.");
        }
        private ContractField ReadContractInternal() => JsonUtility.FromJson<ContractField>(File.ReadAllText(ContractPath));
        private void ValidateCodeInternal()
        {
            RequireAndroidInternal();
            if (Policy == null || !Policy.Valid) throw new InvalidOperationException("Prepare the first player compatibility line.");
            if (ReadContractInternal().m_Fingerprint != Fingerprint)
                throw new InvalidOperationException("Runtime source, binary plugin, defines, architecture or Unity/package version changed. Create a new player/code line.");
        }

        private void AttachPolicyInternal(ContentUpdatePolicy policy)
        {
            var previous = SceneManager.GetActiveScene();
            var scene = EditorSceneManager.OpenScene(c_Boot, OpenSceneMode.Additive);
            try
            {
                var bootstrap = scene.GetRootGameObjects().SelectMany(g => g.GetComponentsInChildren<CcdBootstrap>(true)).Single();
                using var serialized = new SerializedObject(bootstrap);
                serialized.FindProperty("m_UpdatePolicy").objectReferenceValue = policy;
                serialized.ApplyModifiedPropertiesWithoutUndo();
                EditorSceneManager.SaveScene(scene);
            }
            finally { EditorSceneManager.CloseScene(scene, true); SceneManager.SetActiveScene(previous); }
        }

        private void PrepareRemoteSceneInternal()
        {
            var previous = SceneManager.GetActiveScene();
            var scene = EditorSceneManager.OpenScene(c_RemoteScene, OpenSceneMode.Additive);
            try
            {
                foreach (var service in scene.GetRootGameObjects().SelectMany(g => g.GetComponentsInChildren<UnityService>(true)))
                {
                    using var serialized = new SerializedObject(service);
                    serialized.FindProperty("m_InitCondition").enumValueIndex = (int)InitCondition.Build;
                    serialized.ApplyModifiedPropertiesWithoutUndo();
                }
                EditorSceneManager.SaveScene(scene);
            }
            finally { EditorSceneManager.CloseScene(scene, true); SceneManager.SetActiveScene(previous); }
        }

        private string[] EntryPathsInternal() => AddressableAssetSettingsDefaultObject.Settings.groups.Where(g => g != null)
            .SelectMany(g => g.entries).Select(e => e.AssetPath).Where(p => p.StartsWith("Assets/")).Distinct().OrderBy(p => p).ToArray();
        private string DependencyHashInternal()
        {
            var entries = AddressableAssetSettingsDefaultObject.Settings.groups.Where(g => g != null).SelectMany(g => g.entries)
                .Where(e => e.AssetPath.StartsWith("Assets/")).OrderBy(e => e.guid)
                .Select(e => e.guid + ":" + e.address + ":" + string.Join(",", e.labels.OrderBy(l => l)));
            return ContentReleaseManifest.Digest(Encoding.UTF8.GetBytes(string.Join("\n", EntryPathsInternal()
                .Select(p => p + ":" + AssetDatabase.GetAssetDependencyHash(p)).Concat(entries))));
        }

        private string[] LocalBundlesInternal(string version)
        {
            // Binary catalog inspection uses the API provided by this pinned Addressables package.
            var type = typeof(ContentCatalogData);
            var catalog = type.GetMethod("LoadFromFile", BindingFlags.Static | BindingFlags.NonPublic)
                .Invoke(null, new object[] { Path.Combine(ContentPath(version), "catalog_" + version + ".bin"), false });
            var locator = (IResourceLocator)type.GetMethod("CreateCustomLocator", BindingFlags.Instance | BindingFlags.NonPublic)
                .Invoke(catalog, new object[] { "compatibility-audit", null });
            var pending = new Queue<IResourceLocation>();
            foreach (var key in locator.Keys)
                if (locator.Locate(key, null, out var locations)) foreach (var location in locations) pending.Enqueue(location);
            var seen = new HashSet<IResourceLocation>();
            while (pending.Count > 0)
            {
                var location = pending.Dequeue();
                if (!seen.Add(location)) continue;
                foreach (var dependency in location.Dependencies) pending.Enqueue(dependency);
            }
            return seen.Where(l => l.Data is AssetBundleRequestOptions && !l.InternalId.StartsWith("https://", StringComparison.Ordinal))
                .Select(l => l.InternalId).Distinct().ToArray();
        }

        private ShapeField ShapeInternal()
        {
            var result = new ShapeField();
            var scripts = new HashSet<string>(CompilationPipeline.GetAssemblies(AssembliesType.Player).Select(a => a.name));
            foreach (var path in EntryPathsInternal())
            {
                if (AssetDatabase.IsValidFolder(path)) throw new InvalidOperationException("Use explicit content entries instead of Addressable folder entries for compatibility auditing.");
                if (path.EndsWith(".unity", StringComparison.OrdinalIgnoreCase))
                {
                    var previous = SceneManager.GetActiveScene();
                    var scene = EditorSceneManager.OpenScene(path, OpenSceneMode.Additive);
                    try
                    {
                        var roots = scene.GetRootGameObjects();
                        AddTypesInternal(roots.Cast<Object>().ToArray(), result, scripts);
                        foreach (var collection in roots.SelectMany(g => g.GetComponentsInChildren<HeroesCollection>(true)))
                        {
                            var ids = collection.HeroUnits.Select(h => h.HeroConfig.BaseInfo.Id).ToArray();
                            if (ids.Any(string.IsNullOrEmpty) || ids.Distinct().Count() != ids.Length)
                                throw new InvalidOperationException("Hero IDs must be nonempty and unique.");
                            result.m_HeroIds.UnionWith(ids);
                        }
                    }
                    finally { EditorSceneManager.CloseScene(scene, true); SceneManager.SetActiveScene(previous); }
                }
                else
                {
                    var asset = AssetDatabase.LoadMainAssetAtPath(path);
                    if (asset != null) AddTypesInternal(new[] { asset }, result, scripts);
                }
            }
            return result;
        }

        private void AddTypesInternal(Object[] assets, ShapeField shape, HashSet<string> scripts)
        {
            foreach (var asset in EditorUtility.CollectDependencies(assets))
            {
                if (asset == null) continue;
                var type = asset is MonoScript script ? script.GetClass() : asset.GetType();
                if (type == null) continue;
                var assembly = type.Assembly.GetName().Name;
                if (assembly.StartsWith("UnityEngine", StringComparison.Ordinal) || scripts.Contains(assembly))
                    shape.m_Types.Add(assembly + "|" + type.FullName);
            }
        }
    }

    public sealed class CcdPlayerBuildGuard : IPreprocessBuildWithReport, IPostprocessBuildWithReport
    {
        public int callbackOrder => int.MaxValue;
        public void OnPreprocessBuild(BuildReport report)
        {
            if (report.summary.platform == BuildTarget.Android && report.summary.outputPath.EndsWith(".aab", StringComparison.OrdinalIgnoreCase))
                new CcdUpdateWorkflow().ValidatePlayerBuild();
        }
        public void OnPostprocessBuild(BuildReport report)
        {
            if (report.summary.platform == BuildTarget.Android && report.summary.outputPath.EndsWith(".aab", StringComparison.OrdinalIgnoreCase))
                new CcdUpdateWorkflow().RecordPlayerBuild(report.summary.outputPath);
        }
    }
}
#endif
