using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.IO;

namespace LegionKnight
{
    public class UnUsedAssetWindow : EditorWindow
    {
        private static List<string> m_UnusedAssets = new List<string>();
        private Vector2 m_Scroll;

        [MenuItem("Tools/Find Unused Assets")]
        public static void ShowWindow()
        {
            GetWindow<UnUsedAssetWindow>("Find Unused Assets");
            FindUnused();
        }

        private static void FindUnused()
        {
            m_UnusedAssets.Clear();

            // Step 1: Get all assets in project
            string[] allAssets = AssetDatabase.GetAllAssetPaths();

            // Initialize empty used-assets set, then populate from build scenes
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

            // Step 3: Check each asset
            foreach (string asset in allAssets)
            {
                // Skip non-project assets
                if (!asset.StartsWith("Assets/"))
                    continue;

                // Skip folders (use AssetDatabase.IsValidFolder), editor scripts, resources, streaming assets, packages
                if (AssetDatabase.IsValidFolder(asset)) continue;
                if (asset.Contains("/Editor/")) continue;
                if (asset.Contains("/Resources/")) continue;
                if (asset.Contains("/StreamingAssets/")) continue;
                if (asset.Contains("/AddressableAssetsData/")) continue;

                // Skip assets that are used
                if (allUsedAssets.Contains(asset)) continue;

                m_UnusedAssets.Add(asset);
            }

            Debug.Log($"Found {m_UnusedAssets.Count} unused assets.");
        }

        private void OnGUI()
        {
            if (GUILayout.Button("Re-Scan"))
                FindUnused();

            GUILayout.Space(10);
            GUILayout.Label($"Found {m_UnusedAssets.Count} unused assets", EditorStyles.boldLabel);

            m_Scroll = GUILayout.BeginScrollView(m_Scroll);
            foreach (string asset in m_UnusedAssets)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label(asset, GUILayout.ExpandWidth(true));
                if (GUILayout.Button("Ping", GUILayout.Width(60)))
                    EditorGUIUtility.PingObject(AssetDatabase.LoadAssetAtPath<Object>(asset));
                GUILayout.EndHorizontal();
            }
            GUILayout.EndScrollView();

            GUILayout.Space(10);
            if (m_UnusedAssets.Count > 0 && GUILayout.Button("Select All in Project"))
            {
                List<Object> objs = new List<Object>();
                foreach (string path in m_UnusedAssets)
                    objs.Add(AssetDatabase.LoadAssetAtPath<Object>(path));
                Selection.objects = objs.ToArray();
            }
        }
    }
}
