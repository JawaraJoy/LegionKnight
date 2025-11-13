#if UNITY_EDITOR
using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;

namespace LegionKnight
{
    public class UnUsedAssetWindow : EditorWindow
    {
        private static List<(string path, long size)> unusedAssets = new List<(string, long)>();
        private static Vector2 scroll;
        private static bool isScanning = false;
        private static float progress = 0f;
        private static string progressText = "";

        private static string[] allAssets;
        private static HashSet<string> allUsedAssets;
        private static int currentIndex;
        private static int totalCount;

        [MenuItem("Tools/Find Unused Assets (Async)")]
        public static void ShowWindow()
        {
            GetWindow<UnUsedAssetWindow>("Find Unused Assets");
        }

        private void OnGUI()
        {
            GUILayout.Label("Unused Asset Finder", EditorStyles.boldLabel);
            GUILayout.Space(10);

            GUI.enabled = !isScanning;
            if (GUILayout.Button("Start Scan"))
                StartScan();

            GUI.enabled = true;

            if (isScanning)
            {
                EditorGUILayout.HelpBox($"Scanning... {progressText}", MessageType.Info);
                EditorGUI.ProgressBar(EditorGUILayout.GetControlRect(), progress, $"{(progress * 100f):0}%");
                Repaint();
                return;
            }

            GUILayout.Space(10);
            GUILayout.Label($"Found {unusedAssets.Count} unused assets", EditorStyles.boldLabel);

            scroll = GUILayout.BeginScrollView(scroll);
            foreach (var (path, size) in unusedAssets)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label($"{path} ({FormatSize(size)})", GUILayout.ExpandWidth(true));
                if (GUILayout.Button("Ping", GUILayout.Width(60)))
                    EditorGUIUtility.PingObject(AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(path));
                GUILayout.EndHorizontal();
            }
            GUILayout.EndScrollView();

            GUILayout.Space(10);
            if (unusedAssets.Count > 0 && GUILayout.Button("Select All in Project"))
            {
                List<UnityEngine.Object> objs = new List<UnityEngine.Object>();
                foreach (var (path, _) in unusedAssets)
                    objs.Add(AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(path));
                Selection.objects = objs.ToArray();
            }
        }

        // ----------------------------------------------------------
        // Async Scan Logic
        // ----------------------------------------------------------
        private static void StartScan()
        {
            if (isScanning) return;

            unusedAssets.Clear();
            allUsedAssets = new HashSet<string>();
            allAssets = AssetDatabase.GetAllAssetPaths()
                .Where(a => a.StartsWith("Assets/") && !Directory.Exists(a))
                .ToArray();

            totalCount = allAssets.Length;
            currentIndex = 0;
            progress = 0f;
            progressText = "Preparing...";
            isScanning = true;

            // Collect dependencies from build scenes + addressables
            CollectUsedAssets();

            EditorApplication.update += ScanStep;
        }

        private static void ScanStep()
        {
            const int batchSize = 100; // process 100 assets per frame
            int end = Mathf.Min(currentIndex + batchSize, totalCount);

            for (int i = currentIndex; i < end; i++)
            {
                string asset = allAssets[i];

                // Skip editor-related or special folders
                if (asset.Contains("/Editor/") || asset.Contains("/Resources/") ||
                    asset.Contains("/StreamingAssets/") || asset.Contains("/AddressableAssetsData/"))
                    continue;

                if (!allUsedAssets.Contains(asset))
                {
                    long size = GetFileSize(asset);
                    unusedAssets.Add((asset, size));
                }
            }

            currentIndex = end;
            progress = (float)currentIndex / totalCount;
            progressText = $"{currentIndex} / {totalCount}";

            EditorUtility.DisplayProgressBar("Scanning Unused Assets", progressText, progress);

            if (currentIndex >= totalCount)
            {
                EditorUtility.ClearProgressBar();
                EditorApplication.update -= ScanStep;
                isScanning = false;
                progressText = "Completed!";
                Debug.Log($"✅ Found {unusedAssets.Count} unused assets.");
            }
        }

        // ----------------------------------------------------------
        // Dependency Collector
        // ----------------------------------------------------------
        private static void CollectUsedAssets()
        {
            // 1. Build scenes
            foreach (var scene in EditorBuildSettings.scenes)
            {
                if (scene.enabled)
                {
                    string[] deps = AssetDatabase.GetDependencies(scene.path, true);
                    foreach (var dep in deps)
                        allUsedAssets.Add(dep);
                }
            }

            // 2. Addressables (if installed)
            AddressableAssetSettings settings = AddressableAssetSettingsDefaultObject.Settings;
            if (settings != null)
            {
                List<string> addressablePaths = new List<string>();
                foreach (var group in settings.groups)
                {
                    if (group == null) continue;
                    foreach (var entry in group.entries)
                    {
                        if (entry == null) continue;
                        addressablePaths.Add(entry.AssetPath);
                    }
                }

                if (addressablePaths.Count > 0)
                {
                    string[] addrDeps = AssetDatabase.GetDependencies(addressablePaths.ToArray(), true);
                    foreach (var dep in addrDeps)
                        allUsedAssets.Add(dep);
                }
            }
        }

        private static long GetFileSize(string path)
        {
            try
            {
                FileInfo fi = new FileInfo(path);
                return fi.Exists ? fi.Length : 0;
            }
            catch { return 0; }
        }

        private static string FormatSize(long bytes)
        {
            string[] sizes = { "B", "KB", "MB", "GB" };
            double len = bytes;
            int order = 0;
            while (len >= 1024 && order < sizes.Length - 1)
            {
                order++;
                len /= 1024;
            }
            return $"{len:0.##} {sizes[order]}";
        }
    }
}
#endif
