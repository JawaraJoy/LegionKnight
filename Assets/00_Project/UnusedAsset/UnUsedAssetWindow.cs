using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Linq;

#if UNITY_ADDRESSABLES
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
#endif

namespace LegionKnight
{
    public class UnUsedAssetWindow : EditorWindow
    {
        private class AssetInfo
        {
            public string path;
            public long size;
        }

        private static List<AssetInfo> unusedAssets = new List<AssetInfo>();
        private Vector2 scroll;
        private static bool sortBySize = true;

        [MenuItem("Tools/Find Unused Assets")]
        public static void ShowWindow()
        {
            GetWindow<UnUsedAssetWindow>("Find Unused Assets");
            FindUnused();
        }

        private static void FindUnused()
        {
            unusedAssets.Clear();

            // Step 1: Get all assets in project
            string[] allAssets = AssetDatabase.GetAllAssetPaths();
            HashSet<string> allUsedAssets = new HashSet<string>();

            // Step 2: Add all assets used by scenes in Build Settings
            foreach (EditorBuildSettingsScene scene in EditorBuildSettings.scenes)
            {
                if (scene.enabled)
                {
                    string[] deps = AssetDatabase.GetDependencies(scene.path, true);
                    foreach (var dep in deps)
                        allUsedAssets.Add(dep);
                }
            }

#if UNITY_ADDRESSABLES
            // Step 3: Add all assets used by Addressables
            AddressableAssetSettings settings = AddressableAssetSettingsDefaultObject.Settings;
            if (settings != null)
            {
                foreach (var group in settings.groups)
                {
                    if (group == null) continue;
                    foreach (var entry in group.entries)
                    {
                        if (entry == null) continue;

                        // Add the addressable asset itself
                        allUsedAssets.Add(entry.AssetPath);

                        // Also include its dependencies
                        string[] deps = AssetDatabase.GetDependencies(entry.AssetPath, true);
                        foreach (var dep in deps)
                            allUsedAssets.Add(dep);
                    }
                }
            }
#endif

            // Step 4: Check each asset
            foreach (string asset in allAssets)
            {
                // Skip non-project assets
                if (!asset.StartsWith("Assets/"))
                    continue;

                // Skip folders, editor scripts, resources, streaming assets, packages
                if (AssetDatabase.IsValidFolder(asset)) continue;
                if (asset.Contains("/Editor/")) continue;
                if (asset.Contains("/Resources/")) continue;
                if (asset.Contains("/StreamingAssets/")) continue;
                if (asset.Contains("/AddressableAssetsData/")) continue;

                // Skip assets that are used (by scene or addressable)
                if (allUsedAssets.Contains(asset)) continue;

                // Get file size
                long size = 0;
                try
                {
                    FileInfo fileInfo = new FileInfo(asset);
                    size = fileInfo.Exists ? fileInfo.Length : 0;
                }
                catch { }

                unusedAssets.Add(new AssetInfo { path = asset, size = size });
            }

            // Sort by size descending by default
            unusedAssets = unusedAssets.OrderByDescending(a => a.size).ToList();

            Debug.Log($"[Unused Asset Finder] Found {unusedAssets.Count} unused assets.");
        }

        private void OnGUI()
        {
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Re-Scan", GUILayout.Width(100)))
                FindUnused();

            if (GUILayout.Button(sortBySize ? "Sort: Size ▼" : "Sort: Name ▲", GUILayout.Width(120)))
            {
                sortBySize = !sortBySize;
                if (sortBySize)
                    unusedAssets = unusedAssets.OrderByDescending(a => a.size).ToList();
                else
                    unusedAssets = unusedAssets.OrderBy(a => a.path).ToList();
            }
            GUILayout.EndHorizontal();

            GUILayout.Space(10);
            GUILayout.Label($"Found {unusedAssets.Count} unused assets", EditorStyles.boldLabel);

            scroll = GUILayout.BeginScrollView(scroll);
            foreach (AssetInfo asset in unusedAssets)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label(FormatSize(asset.size), GUILayout.Width(80));
                GUILayout.Label(asset.path, GUILayout.ExpandWidth(true));
                if (GUILayout.Button("Ping", GUILayout.Width(60)))
                    EditorGUIUtility.PingObject(AssetDatabase.LoadAssetAtPath<Object>(asset.path));
                GUILayout.EndHorizontal();
            }
            GUILayout.EndScrollView();

            GUILayout.Space(10);
            if (unusedAssets.Count > 0 && GUILayout.Button("Select All in Project"))
            {
                List<Object> objs = new List<Object>();
                foreach (AssetInfo a in unusedAssets)
                    objs.Add(AssetDatabase.LoadAssetAtPath<Object>(a.path));
                Selection.objects = objs.ToArray();
            }
        }

        private string FormatSize(long bytes)
        {
            if (bytes > 1_000_000)
                return $"{(bytes / 1_000_000f):F1} MB";
            else if (bytes > 1_000)
                return $"{(bytes / 1_000f):F1} KB";
            else
                return $"{bytes} B";
        }
    }
}
