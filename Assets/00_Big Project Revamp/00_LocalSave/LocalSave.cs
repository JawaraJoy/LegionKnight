using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace LegionKnight
{
    /// <summary>
    /// Wrapper stored in PlayerPrefs for every saved value.
    /// Mirrors CloudValue&lt;T&gt; so TTL / expiry logic stays identical.
    /// </summary>
    [Serializable]
    public class LocalValue<T>
    {
        public T value;
        public long lastUpdateUnix;
        public long ttlSeconds;       // 0 = never expires
    }

    public class LocalSave : MonoBehaviour
    {
        // ── Inspector ────────────────────────────────────────────────────────────
        [SerializeField] private bool m_UseLocalSave = true;

        /// <summary>Optional prefix so keys never collide with raw PlayerPrefs.</summary>
        [SerializeField] private string m_KeyPrefix = "LS_";

        [SerializeField] private UnityEvent m_OnDataLoaded = new();

        // ── Internal cache ───────────────────────────────────────────────────────
        // Values stored here are the raw JSON strings exactly as written to PlayerPrefs.
        // GetData / GetDataValue deserialise on demand.
        private readonly Dictionary<string, string> m_PlayerData = new();

        // ────────────────────────────────────────────────────────────────────────
        // Unity lifecycle
        // ────────────────────────────────────────────────────────────────────────
        private void Awake()
        {
            LoadAllDataWithExpiry();
        }

        // ────────────────────────────────────────────────────────────────────────
        // Helpers
        // ────────────────────────────────────────────────────────────────────────
        private string PrefixedKey(string key) => m_KeyPrefix + key;

        private long GetServerTimeUnix() => DateTimeOffset.UtcNow.ToUnixTimeSeconds();

        private bool IsExpired(long lastUpdateUnix, long ttlSeconds, long serverNow)
        {
            if (ttlSeconds <= 0) return false;
            return serverNow - lastUpdateUnix >= ttlSeconds;
        }

        /// <summary>
        /// Returns true when the key was expired (and removes it from cache + PlayerPrefs).
        /// </summary>
        private bool TryProcessExpiry(string key, long lastUpdateUnix, long ttlSeconds, long serverNow)
        {
            if (!IsExpired(lastUpdateUnix, ttlSeconds, serverNow)) return false;

            PlayerPrefs.DeleteKey(PrefixedKey(key));
            m_PlayerData.Remove(key);
            Debug.Log($"Local data expired and removed: {key}");
            return true;
        }

        private void OnDataLoadedInvoke()
        {
            Debug.Log("LocalSave: all data loaded successfully.");
            m_OnDataLoaded?.Invoke();
        }

        // ────────────────────────────────────────────────────────────────────────
        // Save
        // ────────────────────────────────────────────────────────────────────────

        /// <summary>
        /// Save a raw value (no TTL). Signature matches CloudSave.SaveData(string, object, UnityAction).
        /// </summary>
        public void SaveData(string key, object value, UnityAction callback = null)
        {
            if (!m_UseLocalSave)
            {
                Debug.LogWarning("Local Save is disabled. Data will not be saved.");
                return;
            }

            try
            {
                // Wrap in LocalValue<object> so the format stays consistent.
                var wrapped = new LocalValue<object>
                {
                    value = value,
                    lastUpdateUnix = GetServerTimeUnix(),
                    ttlSeconds = 0
                };

                string json = JsonUtility.ToJson(wrapped);
                m_PlayerData[key] = json;
                PlayerPrefs.SetString(PrefixedKey(key), json);
                AddToIndex(key);
                PlayerPrefs.Save();

                Debug.Log($"LocalSave: saved {key} = {value}");
                callback?.Invoke();
            }
            catch (Exception ex)
            {
                Debug.LogError($"LocalSave: SaveData failed for '{key}'. {ex.Message}");
            }
        }

        /// <summary>
        /// Save a typed value with optional TTL (seconds). TTL 0 = never expires.
        /// Signature matches CloudSave.SaveData&lt;T&gt;(string, T, long, UnityAction).
        /// </summary>
        public void SaveData<T>(string key, T value, long ttlSeconds = 0, UnityAction callback = null)
        {
            if (!m_UseLocalSave) return;

            try
            {
                var wrapped = new LocalValue<T>
                {
                    value = value,
                    lastUpdateUnix = GetServerTimeUnix(),
                    ttlSeconds = ttlSeconds
                };

                string json = JsonUtility.ToJson(wrapped);
                m_PlayerData[key] = json;
                PlayerPrefs.SetString(PrefixedKey(key), json);
                AddToIndex(key);
                PlayerPrefs.Save();

                Debug.Log($"LocalSave: saved <{typeof(T).Name}> {key}");
                callback?.Invoke();
            }
            catch (Exception ex)
            {
                Debug.LogError($"LocalSave: SaveData<T> failed for '{key}'. {ex.Message}");
            }
        }

        // ────────────────────────────────────────────────────────────────────────
        // Load (single key)
        // ────────────────────────────────────────────────────────────────────────

        /// <summary>
        /// Load a single key into the cache. Matches CloudSave.LoadData(string, UnityAction).
        /// </summary>
        public void LoadData(string key, UnityAction callback = null)
        {
            if (!m_UseLocalSave)
            {
                Debug.LogWarning("Local Save is disabled. Data will not be loaded.");
                return;
            }

            string prefixed = PrefixedKey(key);
            if (!PlayerPrefs.HasKey(prefixed))
            {
                Debug.Log($"LocalSave: no data found for key '{key}'.");
                callback?.Invoke();
                return;
            }

            try
            {
                string json = PlayerPrefs.GetString(prefixed);
                m_PlayerData[key] = json;
                Debug.Log($"LocalSave: loaded '{key}'.");
                callback?.Invoke();
            }
            catch (Exception ex)
            {
                Debug.LogError($"LocalSave: LoadData failed for '{key}'. {ex.Message}");
            }
        }

        /// <summary>
        /// Load a single key, respecting TTL expiry.
        /// Callback receives true if data is valid, false if missing or expired.
        /// Matches CloudSave.LoadDataWithExpiry(string, UnityAction&lt;bool&gt;).
        /// </summary>
        public void LoadDataWithExpiry(string key, UnityAction<bool> callback = null)
        {
            if (!m_UseLocalSave) return;

            string prefixed = PrefixedKey(key);
            if (!PlayerPrefs.HasKey(prefixed))
            {
                callback?.Invoke(false);
                return;
            }

            try
            {
                string json = PlayerPrefs.GetString(prefixed);

                // Peek at the expiry fields via a plain wrapper
                var peek = JsonUtility.FromJson<LocalValue<object>>(json);

                if (TryProcessExpiry(key, peek.lastUpdateUnix, peek.ttlSeconds, GetServerTimeUnix()))
                {
                    callback?.Invoke(false);
                    return;
                }

                m_PlayerData[key] = json;
                callback?.Invoke(true);
            }
            catch (Exception ex)
            {
                Debug.LogError($"LocalSave: LoadDataWithExpiry failed for '{key}'. {ex.Message}");
                callback?.Invoke(false);
            }
        }

        // ────────────────────────────────────────────────────────────────────────
        // Load all
        // ────────────────────────────────────────────────────────────────────────

        /// <summary>
        /// Loads every LS_* key from PlayerPrefs, purges expired ones, then fires OnDataLoaded.
        /// Called automatically in Awake. Matches CloudSave.LoadAllDataWithExpiry().
        /// </summary>
        public void LoadAllDataWithExpiry()
        {
            if (!m_UseLocalSave)
            {
                Debug.LogWarning("Local Save is disabled. Data will not be loaded.");
                return;
            }

            try
            {
                m_PlayerData.Clear();
                long serverNow = GetServerTimeUnix();

                // Unity doesn't expose a key-list API, so we persist an index ourselves.
                string indexJson = PlayerPrefs.GetString(m_KeyPrefix + "__index__", "{}");
                var index = JsonUtility.FromJson<StringSet>(indexJson) ?? new StringSet();

                var toRemove = new List<string>();

                foreach (string key in index.keys)
                {
                    string prefixed = PrefixedKey(key);
                    if (!PlayerPrefs.HasKey(prefixed)) continue;

                    string json = PlayerPrefs.GetString(prefixed);

                    try
                    {
                        var peek = JsonUtility.FromJson<LocalValue<object>>(json);

                        // Legacy data without the wrapper fields
                        bool hasExpiry = peek.lastUpdateUnix != 0 || peek.ttlSeconds != 0;

                        if (hasExpiry && IsExpired(peek.lastUpdateUnix, peek.ttlSeconds, serverNow))
                        {
                            PlayerPrefs.DeleteKey(prefixed);
                            toRemove.Add(key);
                            Debug.Log($"LocalSave: expired key removed on load: {key}");
                            continue;
                        }

                        m_PlayerData[key] = json;
                    }
                    catch
                    {
                        // Non-wrapped legacy entry → keep
                        m_PlayerData[key] = json;
                    }
                }

                // Update the index after purging expired keys
                foreach (string k in toRemove) index.keys.Remove(k);
                PlayerPrefs.SetString(m_KeyPrefix + "__index__", JsonUtility.ToJson(index));
                PlayerPrefs.Save();

                OnDataLoadedInvoke();
            }
            catch (Exception ex)
            {
                Debug.LogError($"LocalSave: LoadAllDataWithExpiry failed. {ex.Message}");
            }
        }

        // ────────────────────────────────────────────────────────────────────────
        // Get
        // ────────────────────────────────────────────────────────────────────────

        /// <summary>
        /// Returns the cached value as T. Handles both raw and LocalValue-wrapped entries.
        /// Matches CloudSave.GetData&lt;T&gt;(string).
        /// </summary>
        public T GetData<T>(string key)
        {
            if (!m_UseLocalSave)
            {
                Debug.LogWarning("Local Save is disabled.");
                return default;
            }

            if (!m_PlayerData.TryGetValue(key, out string json))
            {
                Debug.LogWarning($"LocalSave: no cached data for key '{key}'.");
                return default;
            }

            try
            {
                var wrapped = JsonUtility.FromJson<LocalValue<T>>(json);
                if (wrapped != null) return wrapped.value;
            }
            catch { /* fall through */ }

            // Attempt direct deserialisation as T
            try
            {
                return JsonUtility.FromJson<T>(json);
            }
            catch (Exception ex)
            {
                Debug.LogError($"LocalSave: GetData<{typeof(T).Name}> failed for '{key}'. {ex.Message}");
                return default;
            }
        }

        /// <summary>
        /// Returns the inner .value from a LocalValue&lt;T&gt; wrapper.
        /// Matches CloudSave.GetDataValue&lt;T&gt;(string).
        /// </summary>
        public T GetDataValue<T>(string key)
        {
            if (!m_PlayerData.TryGetValue(key, out string json))
                return default;

            try
            {
                var wrapped = JsonUtility.FromJson<LocalValue<T>>(json);
                return wrapped != null ? wrapped.value : default;
            }
            catch
            {
                return default;
            }
        }

        // ────────────────────────────────────────────────────────────────────────
        // Has / Delete
        // ────────────────────────────────────────────────────────────────────────

        /// <summary>Matches CloudSave.HasData(string).</summary>
        public bool HasData(string key)
        {
            if (!m_UseLocalSave)
            {
                Debug.LogWarning("Local Save is disabled.");
                return false;
            }
            return m_PlayerData.ContainsKey(key);
        }

        /// <summary>Matches CloudSave.DeleteData(string).</summary>
        public void DeleteData(string key)
        {
            try
            {
                PlayerPrefs.DeleteKey(PrefixedKey(key));
                m_PlayerData.Remove(key);
                RemoveFromIndex(key);
                PlayerPrefs.Save();
                Debug.Log($"LocalSave: deleted key '{key}'.");
            }
            catch (Exception ex)
            {
                Debug.LogError($"LocalSave: DeleteData failed for '{key}'. {ex.Message}");
            }
        }

        /// <summary>Matches CloudSave.DeleteAllData().</summary>
        public void DeleteAllData()
        {
            try
            {
                // Delete every key tracked in the index
                string indexJson = PlayerPrefs.GetString(m_KeyPrefix + "__index__", "{}");
                var index = JsonUtility.FromJson<StringSet>(indexJson) ?? new StringSet();

                foreach (string key in index.keys)
                    PlayerPrefs.DeleteKey(PrefixedKey(key));

                PlayerPrefs.DeleteKey(m_KeyPrefix + "__index__");
                m_PlayerData.Clear();
                PlayerPrefs.Save();

                Debug.Log("LocalSave: all data deleted.");
            }
            catch (Exception ex)
            {
                Debug.LogError($"LocalSave: DeleteAllData failed. {ex.Message}");
            }
        }

        // ────────────────────────────────────────────────────────────────────────
        // Index management (needed because PlayerPrefs has no key-enumeration API)
        // ────────────────────────────────────────────────────────────────────────

        private void AddToIndex(string key)
        {
            string indexJson = PlayerPrefs.GetString(m_KeyPrefix + "__index__", "{}");
            var index = JsonUtility.FromJson<StringSet>(indexJson) ?? new StringSet();
            if (!index.keys.Contains(key)) index.keys.Add(key);
            PlayerPrefs.SetString(m_KeyPrefix + "__index__", JsonUtility.ToJson(index));
        }

        private void RemoveFromIndex(string key)
        {
            string indexJson = PlayerPrefs.GetString(m_KeyPrefix + "__index__", "{}");
            var index = JsonUtility.FromJson<StringSet>(indexJson) ?? new StringSet();
            index.keys.Remove(key);
            PlayerPrefs.SetString(m_KeyPrefix + "__index__", JsonUtility.ToJson(index));
        }

        // ────────────────────────────────────────────────────────────────────────
        // Internal serializable types
        // ────────────────────────────────────────────────────────────────────────

        [Serializable]
        private class StringSet
        {
            public List<string> keys = new();
        }
    }
}