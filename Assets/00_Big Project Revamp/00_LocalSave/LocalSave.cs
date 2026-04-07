using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace LegionKnight
{
    public class LocalSave : MonoBehaviour
    {
        [Header("Config")]
        [SerializeField] private bool m_UseLocalSave = true;
        [SerializeField] private string m_KeyPrefix = "LS_";
        [SerializeField] private int m_CurrentVersion = 1;

        [Header("Security")]
        [SerializeField] private bool m_UseHashValidation = true;

        [Header("Events")]
        [SerializeField] private UnityEvent m_OnDataLoaded = new();

        private readonly Dictionary<string, string> m_Cache = new();

        // ─────────────────────────────────────────────
        // UTIL
        // ─────────────────────────────────────────────

        private string Prefixed(string key) => m_KeyPrefix + key;
        private long Now() => DateTimeOffset.UtcNow.ToUnixTimeSeconds();

        private bool IsExpired(long last, long ttl)
        {
            if (ttl <= 0) return false;
            return Now() - last >= ttl;
        }

        // ─────────────────────────────────────────────
        // HASH
        // ─────────────────────────────────────────────

        private string GenerateHash(string json)
        {
            return json.GetHashCode().ToString();
        }

        private bool ValidateHash(string json, string hash)
        {
            return GenerateHash(json) == hash;
        }

        // ─────────────────────────────────────────────
        // SAVE
        // ─────────────────────────────────────────────

        public void SaveData<T>(string key, T value, long ttl = 0, UnityAction callback = null)
        {
            if (!m_UseLocalSave) return;

            try
            {
                var wrapper = new SaveWrapper<T>
                {
                    value = value,
                    lastUpdateUnix = Now(),
                    ttlSeconds = ttl,
                    version = m_CurrentVersion
                };

                // hash dihitung TANPA field hash
                string jsonWithoutHash = JsonUtility.ToJson(wrapper);

                if (m_UseHashValidation)
                    wrapper.hash = GenerateHash(jsonWithoutHash);

                string finalJson = JsonUtility.ToJson(wrapper);

                string fullKey = Prefixed(key);

                m_Cache[fullKey] = finalJson;
                PlayerPrefs.SetString(fullKey, finalJson);

                AddToIndex(key); // index pakai raw key
                PlayerPrefs.Save();

                callback?.Invoke();
            }
            catch (Exception ex)
            {
                Debug.LogError($"SaveData<{typeof(T)}> failed: {ex}");
            }
        }

        // ─────────────────────────────────────────────
        // GET
        // ─────────────────────────────────────────────

        public T GetDataValue<T>(string key)
        {
            if (TryGetData<T>(key, out var value))
                return value;

            return default;
        }

        private bool TryGetData<T>(string key, out T value)
        {
            value = default;
            string fullKey = Prefixed(key);

            if (!m_Cache.TryGetValue(fullKey, out string json))
                return false;

            try
            {
                var wrapper = JsonUtility.FromJson<SaveWrapper<T>>(json);
                if (wrapper == null)
                    return false;

                if (IsExpired(wrapper.lastUpdateUnix, wrapper.ttlSeconds))
                {
                    DeleteData(key);
                    return false;
                }

                if (m_UseHashValidation)
                {
                    string originalHash = wrapper.hash;

                    // remove hash dulu sebelum generate ulang
                    wrapper.hash = null;
                    string rawJson = JsonUtility.ToJson(wrapper);
                    wrapper.hash = originalHash;

                    if (!ValidateHash(rawJson, originalHash))
                    {
                        Debug.LogWarning($"Tampered data: {key}");
                        DeleteData(key);
                        return false;
                    }
                }

                value = wrapper.value;
                return true;
            }
            catch
            {
                Debug.LogWarning($"Corrupted data: {key}");
                DeleteData(key);
                return false;
            }
        }

        // ─────────────────────────────────────────────
        // LOAD (LEGACY SUPPORT)
        // ─────────────────────────────────────────────

        public void LoadData(string key, UnityAction callback = null)
        {
            if (!m_UseLocalSave)
            {
                callback?.Invoke();
                return;
            }

            string fullKey = Prefixed(key);

            if (!PlayerPrefs.HasKey(fullKey))
            {
                callback?.Invoke();
                return;
            }

            try
            {
                string json = PlayerPrefs.GetString(fullKey);

                var peek = JsonUtility.FromJson<ExpiryPeek>(json);

                if (peek != null && IsExpired(peek.lastUpdateUnix, peek.ttlSeconds))
                {
                    DeleteData(key);
                    callback?.Invoke();
                    return;
                }

                // ✅ FIX: pakai prefixed key
                m_Cache[fullKey] = json;

                callback?.Invoke();
            }
            catch
            {
                DeleteData(key);
                callback?.Invoke();
            }
        }

        public void LoadDataWithExpiry(string key, UnityAction<bool> callback)
        {
            if (!m_UseLocalSave)
            {
                callback?.Invoke(false);
                return;
            }

            string fullKey = Prefixed(key);

            if (!PlayerPrefs.HasKey(fullKey))
            {
                callback?.Invoke(false);
                return;
            }

            try
            {
                string json = PlayerPrefs.GetString(fullKey);

                var peek = JsonUtility.FromJson<ExpiryPeek>(json);

                if (peek != null && IsExpired(peek.lastUpdateUnix, peek.ttlSeconds))
                {
                    DeleteData(key);
                    callback?.Invoke(false);
                    return;
                }

                // ✅ FIX: pakai prefixed key
                m_Cache[fullKey] = json;

                callback?.Invoke(true);
            }
            catch
            {
                DeleteData(key);
                callback?.Invoke(false);
            }
        }

        // ─────────────────────────────────────────────
        // HAS DATA
        // ─────────────────────────────────────────────

        public bool HasData(string key)
        {
            if (!m_UseLocalSave)
                return false;

            string fullKey = Prefixed(key);

            if (!m_Cache.ContainsKey(fullKey))
                return false;

            try
            {
                var peek = JsonUtility.FromJson<ExpiryPeek>(m_Cache[fullKey]);

                if (peek != null && IsExpired(peek.lastUpdateUnix, peek.ttlSeconds))
                {
                    DeleteData(key);
                    return false;
                }
            }
            catch
            {
                DeleteData(key);
                return false;
            }

            return true;
        }

        // ─────────────────────────────────────────────
        // LOAD ALL
        // ─────────────────────────────────────────────

        public void LoadAll()
        {
            m_Cache.Clear();

            string indexJson = PlayerPrefs.GetString(m_KeyPrefix + "__index__", "");
            var index = string.IsNullOrEmpty(indexJson)
                ? new StringSet()
                : JsonUtility.FromJson<StringSet>(indexJson);

            if (index.keys == null)
                index.keys = new List<string>();

            var toRemove = new List<string>();

            foreach (var key in index.keys)
            {
                string fullKey = Prefixed(key);

                if (!PlayerPrefs.HasKey(fullKey))
                {
                    toRemove.Add(key);
                    continue;
                }

                string json = PlayerPrefs.GetString(fullKey);

                try
                {
                    var peek = JsonUtility.FromJson<ExpiryPeek>(json);

                    if (peek != null && IsExpired(peek.lastUpdateUnix, peek.ttlSeconds))
                    {
                        PlayerPrefs.DeleteKey(fullKey);
                        toRemove.Add(key);
                        continue;
                    }

                    m_Cache[fullKey] = json;
                }
                catch
                {
                    toRemove.Add(key);
                }
            }

            foreach (var k in toRemove)
                index.keys.Remove(k);

            PlayerPrefs.SetString(m_KeyPrefix + "__index__", JsonUtility.ToJson(index));
            PlayerPrefs.Save();

            m_OnDataLoaded?.Invoke();
        }

        // ─────────────────────────────────────────────
        // DELETE
        // ─────────────────────────────────────────────

        public void DeleteData(string key)
        {
            string fullKey = Prefixed(key);

            PlayerPrefs.DeleteKey(fullKey);
            m_Cache.Remove(fullKey);

            RemoveFromIndex(key);
        }

        public void DeleteAllData()
        {
            var index = GetIndex();

            foreach (var key in index.keys)
                PlayerPrefs.DeleteKey(Prefixed(key));

            PlayerPrefs.DeleteKey(m_KeyPrefix + "__index__");
            m_Cache.Clear();
            PlayerPrefs.Save();
        }

        // ─────────────────────────────────────────────
        // INDEX
        // ─────────────────────────────────────────────

        private void AddToIndex(string key)
        {
            var index = GetIndex();

            if (!index.keys.Contains(key))
                index.keys.Add(key);

            PlayerPrefs.SetString(m_KeyPrefix + "__index__", JsonUtility.ToJson(index));
        }

        private void RemoveFromIndex(string key)
        {
            var index = GetIndex();
            index.keys.Remove(key);

            PlayerPrefs.SetString(m_KeyPrefix + "__index__", JsonUtility.ToJson(index));
        }

        private StringSet GetIndex()
        {
            string json = PlayerPrefs.GetString(m_KeyPrefix + "__index__", "");
            return string.IsNullOrEmpty(json)
                ? new StringSet()
                : JsonUtility.FromJson<StringSet>(json);
        }

        // ─────────────────────────────────────────────
        // INTERNAL TYPES
        // ─────────────────────────────────────────────

        [Serializable]
        private class StringSet
        {
            public List<string> keys = new();
        }

        [Serializable]
        private class ExpiryPeek
        {
            public long lastUpdateUnix;
            public long ttlSeconds;
        }

        [Serializable]
        private class SaveWrapper<T>
        {
            public T value;
            public long lastUpdateUnix;
            public long ttlSeconds;
            public int version;
            public string hash;
        }
    }
}