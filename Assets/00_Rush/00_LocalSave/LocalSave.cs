using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;
using UnityEngine.Events;

namespace LegionKnight
{
    public class LocalSave : MonoBehaviour
    {
        [SerializeField] private bool m_UseLocalSave = true;
        [SerializeField] private string m_KeyPrefix = "LS_";
        [SerializeField] private int m_CurrentVersion = 1;
        [SerializeField] private bool m_UseHashValidation = true;
        [SerializeField] private UnityEvent m_OnDataLoaded = new();
        [SerializeField] private UnityEvent<string> m_OnDataLoadFailed = new();

        private const string c_HashPrefix = "sha256:";
        private readonly Dictionary<string, string> m_Cache = new();
        private Dictionary<string, string> m_PendingWrites;
        private HashSet<string> m_PendingIndex;
        private bool m_IndexValid;
        private readonly List<UnityAction> m_AfterCommit = new();
        public string LastLoadError { get; private set; }
        private string IndexKey => m_KeyPrefix + "__index__";
        private string BackupIndexKey => m_KeyPrefix + "__index_backup__";
        private string JournalKey => m_KeyPrefix + "__transaction__";
        private long Now => DateTimeOffset.UtcNow.ToUnixTimeSeconds();

        private string Prefixed(string key) => m_KeyPrefix + key;
        private bool IsExpired(long last, long ttl) => ttl > 0 && Now - last >= ttl;

        private string GenerateHash(string json)
        {
            using (var sha = SHA256.Create())
                return c_HashPrefix + BitConverter.ToString(sha.ComputeHash(Encoding.UTF8.GetBytes(json))).Replace("-", "");
        }

        public void ExecuteTransaction(Action changes)
        {
            ExecuteTransactionInternal(changes);
        }

        private void ExecuteTransactionInternal(Action changes)
        {
            if (changes == null) throw new ArgumentNullException(nameof(changes));
            if (m_PendingWrites != null)
            {
                changes();
                return;
            }

            RecoverJournalInternal();
            var before = new Dictionary<string, string>(m_Cache);
            m_PendingWrites = new Dictionary<string, string>();
            m_PendingIndex = new HashSet<string>(ReadIndexInternal().keys);
            m_AfterCommit.Clear();
            bool durable = false;
            UnityAction[] callbacks;
            try
            {
                changes();
                var journal = new TransactionJournal();
                foreach (var pair in m_PendingWrites)
                    journal.entries.Add(new JournalEntry { key = pair.Key, json = pair.Value, deleted = pair.Value == null });
                journal.indexJson = JsonUtility.ToJson(new StringSet { keys = new List<string>(m_PendingIndex) });

                if (m_UseLocalSave && journal.entries.Count > 0)
                {
                    PlayerPrefs.SetString(JournalKey, JsonUtility.ToJson(journal));
                    PlayerPrefs.Save();
                    durable = true;
                    ApplyJournalInternal(journal);
                }
                callbacks = m_AfterCommit.ToArray();
            }
            catch
            {
                if (!durable)
                {
                    m_Cache.Clear();
                    foreach (var pair in before) m_Cache.Add(pair.Key, pair.Value);
                }
                throw;
            }
            finally
            {
                m_PendingWrites = null;
                m_PendingIndex = null;
                m_AfterCommit.Clear();
            }

            foreach (var callback in callbacks)
            {
                try { callback?.Invoke(); }
                catch (Exception ex) { Debug.LogException(ex, this); }
            }
        }

        private void ApplyJournalInternal(TransactionJournal journal)
        {
            // Replaying absolute values is idempotent after a process interruption.
            foreach (var entry in journal.entries)
            {
                if (entry.deleted) PlayerPrefs.DeleteKey(Prefixed(entry.key));
                else PlayerPrefs.SetString(Prefixed(entry.key), entry.json);
            }
            PlayerPrefs.SetString(IndexKey, journal.indexJson);
            PlayerPrefs.SetString(BackupIndexKey, journal.indexJson);
            PlayerPrefs.Save();
            PlayerPrefs.DeleteKey(JournalKey);
            PlayerPrefs.Save();
        }

        private void RecoverJournalInternal()
        {
            if (!m_UseLocalSave || !PlayerPrefs.HasKey(JournalKey)) return;
            var journal = JsonUtility.FromJson<TransactionJournal>(PlayerPrefs.GetString(JournalKey));
            if (journal?.entries == null || !TryReadIndex(journal.indexJson, out _))
                throw new InvalidOperationException("Save transaction journal is invalid; original records were preserved.");
            foreach (var entry in journal.entries)
                if (entry == null || string.IsNullOrEmpty(entry.key) || (!entry.deleted && entry.json == null))
                    throw new InvalidOperationException("Save transaction contains an invalid entry.");
            ApplyJournalInternal(journal);
            m_Cache.Clear();
        }

        public void SaveData<T>(string key, T value, long ttl = 0, UnityAction callback = null)
        {
            if (!m_UseLocalSave) return;
            if (m_PendingWrites != null)
            {
                SaveDataInternal(key, value, ttl, callback);
                return;
            }
            // Legacy standalone callers keep the non-throwing API. Transactions propagate failures.
            try { ExecuteTransactionInternal(() => SaveDataInternal(key, value, ttl, callback)); }
            catch (Exception ex) { Debug.LogError($"SaveData<{typeof(T)}> failed: {ex}", this); }
        }

        private void SaveDataInternal<T>(string key, T value, long ttl, UnityAction callback)
        {
            if (string.IsNullOrEmpty(key)) throw new ArgumentException("Save key cannot be empty.", nameof(key));
            var wrapper = new SaveWrapper<T> { value = value, lastUpdateUnix = Now, ttlSeconds = ttl, version = m_CurrentVersion };
            if (m_UseHashValidation) wrapper.hash = GenerateHash(JsonUtility.ToJson(wrapper));
            string json = JsonUtility.ToJson(wrapper);
            m_Cache[Prefixed(key)] = json;
            m_PendingWrites[key] = json;
            m_PendingIndex.Add(key);
            if (callback != null) m_AfterCommit.Add(callback);
        }

        public T GetDataValue<T>(string key) => TryGetDataInternal<T>(key, out var value) ? value : default;

        private bool TryGetDataInternal<T>(string key, out T value)
        {
            value = default;
            if (!HasDataInternal(key)) return false;
            try
            {
                var wrapper = JsonUtility.FromJson<SaveWrapper<T>>(m_Cache[Prefixed(key)]);
                if (wrapper == null) return false;
                string hash = wrapper.hash;
                wrapper.hash = null;
                string raw = JsonUtility.ToJson(wrapper);
                if (m_UseHashValidation && hash != null && hash.StartsWith(c_HashPrefix, StringComparison.Ordinal)
                    && !string.Equals(hash, GenerateHash(raw), StringComparison.Ordinal))
                {
                    ReportLoadFailureInternal($"Hash validation failed for {key}; the original record was preserved.");
                    return false;
                }
                value = wrapper.value;
                // Legacy numeric GetHashCode values are not portable across runtimes.
                // Preserve readable legacy records and migrate to deterministic hashing.
                if (m_UseHashValidation && (hash == null || !hash.StartsWith(c_HashPrefix, StringComparison.Ordinal)))
                {
                    wrapper.hash = GenerateHash(raw);
                    string migrated = JsonUtility.ToJson(wrapper);
                    ExecuteTransactionInternal(() =>
                    {
                        m_Cache[Prefixed(key)] = migrated;
                        m_PendingWrites[key] = migrated;
                        m_PendingIndex.Add(key);
                    });
                }
                return true;
            }
            catch (Exception ex)
            {
                ReportLoadFailureInternal($"Could not read {key}; original data preserved. {ex.Message}");
                return false;
            }
        }

        public bool HasData(string key) => HasDataInternal(key);

        private bool HasDataInternal(string key)
        {
            if (!m_UseLocalSave || string.IsNullOrEmpty(key)) return false;
            string fullKey = Prefixed(key);
            if (!m_Cache.TryGetValue(fullKey, out string json))
            {
                if (m_PendingWrites != null && m_PendingWrites.TryGetValue(key, out json) && json == null) return false;
                if (!PlayerPrefs.HasKey(fullKey)) return false;
                json = PlayerPrefs.GetString(fullKey);
            }
            try
            {
                var peek = JsonUtility.FromJson<ExpiryPeek>(json);
                if (peek == null || IsExpired(peek.lastUpdateUnix, peek.ttlSeconds)) return false;
                m_Cache[fullKey] = json;
                return true;
            }
            catch (Exception ex)
            {
                ReportLoadFailureInternal($"Could not read {key}; original data preserved. {ex.Message}");
                return false;
            }
        }

        public void LoadData(string key, UnityAction callback = null)
        {
            HasDataInternal(key);
            callback?.Invoke();
        }

        public void LoadDataWithExpiry(string key, UnityAction<bool> callback)
        {
            bool exists = HasDataInternal(key);
            callback?.Invoke(exists);
        }

        public void LoadAll()
        {
            LastLoadError = null;
            m_Cache.Clear();
            try
            {
                RecoverJournalInternal();
                var index = ReadIndexInternal();
                foreach (string key in index.keys) HasDataInternal(key);
                // Only persist a verified index. Invalid originals remain available for recovery.
                if (m_IndexValid && m_UseLocalSave)
                {
                    string json = JsonUtility.ToJson(index);
                    PlayerPrefs.SetString(IndexKey, json);
                    PlayerPrefs.SetString(BackupIndexKey, json);
                    PlayerPrefs.Save();
                }
            }
            catch (Exception ex)
            {
                ReportLoadFailureInternal($"Local save load failed: {ex.Message}");
            }
            OnDataLoadedInvoke();
        }

        public void DeleteData(string key)
        {
            ExecuteTransactionInternal(() => DeleteDataInternal(key));
        }

        private void DeleteDataInternal(string key)
        {
            if (string.IsNullOrEmpty(key)) return;
            m_Cache.Remove(Prefixed(key));
            m_PendingWrites[key] = null;
            m_PendingIndex.Remove(key);
        }

        public void DeleteAllData()
        {
            ExecuteTransactionInternal(() =>
            {
                foreach (string key in new List<string>(m_PendingIndex)) DeleteDataInternal(key);
                m_Cache.Clear();
            });
        }

        private StringSet ReadIndexInternal()
        {
            string raw = PlayerPrefs.GetString(IndexKey, "");
            m_IndexValid = false;
            if (TryReadIndex(raw, out var index) && (!string.IsNullOrEmpty(raw) || !PlayerPrefs.HasKey(BackupIndexKey)))
            {
                m_IndexValid = true;
                return index;
            }
            if (!string.IsNullOrEmpty(raw) && !PlayerPrefs.HasKey(IndexKey + "_corrupt"))
                PlayerPrefs.SetString(IndexKey + "_corrupt", raw);
            string backup = PlayerPrefs.GetString(BackupIndexKey, "");
            if (TryReadIndex(backup, out index) && !string.IsNullOrEmpty(backup))
            {
                m_IndexValid = true;
                ReportLoadFailureInternal("Save index is invalid; using its backup. Original records were preserved.");
                return index;
            }
            ReportLoadFailureInternal("Save index and backup are invalid. Records are preserved and can still be loaded by key.");
            // Include records already recovered by a caller that knows their key.
            index = new StringSet();
            foreach (string fullKey in m_Cache.Keys)
                if (fullKey.StartsWith(m_KeyPrefix, StringComparison.Ordinal)) index.keys.Add(fullKey.Substring(m_KeyPrefix.Length));
            return index;
        }

        private bool TryReadIndex(string json, out StringSet index)
        {
            index = null;
            if (string.IsNullOrEmpty(json)) { index = new StringSet(); return true; }
            try
            {
                index = JsonUtility.FromJson<StringSet>(json);
                if (index?.keys == null || !json.Contains("\"keys\"")) return false;
                var unique = new HashSet<string>();
                foreach (string key in index.keys)
                    if (string.IsNullOrEmpty(key) || !unique.Add(key)) return false;
                return true;
            }
            catch { return false; }
        }

        private void ReportLoadFailureInternal(string message)
        {
            LastLoadError = message;
            Debug.LogWarning(message, this);
            OnDataLoadFailedInvoke(message);
        }

        private void OnDataLoadedInvoke() => m_OnDataLoaded?.Invoke();
        private void OnDataLoadFailedInvoke(string message) => m_OnDataLoadFailed?.Invoke(message);

        [Serializable] private class StringSet { public List<string> keys = new(); }
        [Serializable] private class ExpiryPeek { public long lastUpdateUnix; public long ttlSeconds; }
        [Serializable] private class SaveWrapper<T>
        {
            public T value;
            public long lastUpdateUnix;
            public long ttlSeconds;
            public int version;
            public string hash;
        }
        [Serializable] private class JournalEntry { public string key; public string json; public bool deleted; }
        [Serializable] private class TransactionJournal { public List<JournalEntry> entries = new(); public string indexJson; }
    }
}
