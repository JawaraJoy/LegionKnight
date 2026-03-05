using UnityEngine;
using UnityEditor;

namespace Rush
{
    public static class MissingScriptScanner
    {
        // ----------------------------
        // SCAN CURRENT SCENE
        // ----------------------------
        [MenuItem("Tools/Rush/Missing Script/Scan Current Scene")]      
        private static void ScanScene()
        {
            GameObject[] objects = GameObject.FindObjectsByType<GameObject>(FindObjectsInactive.Include, FindObjectsSortMode.None);

            int count = 0;

            foreach (GameObject go in objects)
            {
                Component[] components = go.GetComponents<Component>();

                for (int i = 0; i < components.Length; i++)
                {
                    if (components[i] == null)
                    {
                        Debug.Log($"Missing Script on: {GetFullPath(go)}", go);
                        count++;
                    }
                }
            }

            Debug.Log($"Scan Complete. Missing Scripts Found: {count}");
        }

        // ----------------------------
        // REMOVE FROM CURRENT SCENE
        // ----------------------------
        [MenuItem("Tools/Rush/Missing Script/Remove From Scene")]
        private static void RemoveFromScene()
        {
            GameObject[] objects = GameObject.FindObjectsByType<GameObject>(FindObjectsInactive.Include, FindObjectsSortMode.None);

            int count = 0;

            foreach (GameObject go in objects)
            {
                int removed = GameObjectUtility.RemoveMonoBehavioursWithMissingScript(go);
                count += removed;
            }

            Debug.Log($"Removed Missing Scripts From Scene: {count}");
        }

        // ----------------------------
        // SCAN ALL PREFABS
        // ----------------------------
        [MenuItem("Tools/Rush/Missing Script/Scan All Prefabs")]
        private static void ScanPrefabs()
        {
            string[] prefabGUIDs = AssetDatabase.FindAssets("t:Prefab");

            int count = 0;

            foreach (string guid in prefabGUIDs)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);

                Component[] components = prefab.GetComponentsInChildren<Component>(true);

                foreach (Component comp in components)
                {
                    if (comp == null)
                    {
                        Debug.Log($"Missing Script in Prefab: {path}", prefab);
                        count++;
                    }
                }
            }

            Debug.Log($"Prefab Scan Complete. Missing Scripts Found: {count}");
        }

        // ----------------------------
        // REMOVE FROM PREFABS
        // ----------------------------
        [MenuItem("Tools/Rush/Missing Script/Remove From Prefabs")]
        private static void RemoveFromPrefabs()
        {
            string[] prefabGUIDs = AssetDatabase.FindAssets("t:Prefab");

            int removedTotal = 0;

            foreach (string guid in prefabGUIDs)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                GameObject prefab = PrefabUtility.LoadPrefabContents(path);

                int removed = 0;

                foreach (Transform t in prefab.GetComponentsInChildren<Transform>(true))
                {
                    removed += GameObjectUtility.RemoveMonoBehavioursWithMissingScript(t.gameObject);
                }

                if (removed > 0)
                {
                    PrefabUtility.SaveAsPrefabAsset(prefab, path);
                    removedTotal += removed;
                }

                PrefabUtility.UnloadPrefabContents(prefab);
            }

            Debug.Log($"Removed Missing Scripts From Prefabs: {removedTotal}");
        }

        // ----------------------------
        // HELPER
        // ----------------------------
        private static string GetFullPath(GameObject go)
        {
            string path = go.name;

            while (go.transform.parent != null)
            {
                go = go.transform.parent.gameObject;
                path = go.name + "/" + path;
            }

            return path;
        }
    }
}