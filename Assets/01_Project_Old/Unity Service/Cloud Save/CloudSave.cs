using System;
using System.Collections.Generic;
using Unity.Services.CloudSave;
using Unity.Services.CloudSave.Models;
using UnityEngine;
using UnityEngine.Events;

namespace LegionKnight
{
    public partial class CloudSave : MonoBehaviour
    {
        [SerializeField]
        private bool m_UseCloudSave = true;
        private readonly Dictionary<string, object> m_PlayerData = new();
        [SerializeField]
        private UnityEvent m_OnDataLoaded = new();

        public async void SaveData(string key, object value, UnityAction callback = null)
        {
            if (!m_UseCloudSave)
            {
                Debug.LogWarning("Cloud Save is disabled. Data will not be saved.");
                return;
            }
            try
            {
                // Update local cache
                if (m_PlayerData.ContainsKey(key))
                {
                    m_PlayerData[key] = value;
                }
                else
                {
                    m_PlayerData.Add(key, value);
                }

                // Save to cloud
                var dataToSave = new Dictionary<string, object> { { key, value } };
                await CloudSaveService.Instance.Data.Player.SaveAsync(dataToSave);

                Debug.Log($"Successfully saved data: {key} = {value}");
                callback?.Invoke();
            }
            catch (Exception ex)
            {
                Debug.LogError($"Failed to save data for key: {key}. Exception: {ex.Message}");
            }
        }

        public async void LoadData(string key, UnityAction callback = null)
        {
            if (!m_UseCloudSave)
            {
                Debug.LogWarning("Cloud Save is disabled. Data will not be loaded.");
                return;
            }
            try
            {
                var keys = new HashSet<string> { key };
                var playerData = await CloudSaveService.Instance.Data.Player.LoadAsync(keys);

                if (playerData.TryGetValue(key, out var value))
                {
                    Debug.Log($"Successfully loaded data: {key} = {value}");

                    // Update local cache
                    if (m_PlayerData.ContainsKey(key))
                    {
                        m_PlayerData[key] = value;
                    }
                    else
                    {
                        m_PlayerData.Add(key, value);
                    }
                }
                else
                {
                    Debug.Log($"No data found for key: {key}");
                }

                callback?.Invoke();
            }
            catch (Exception ex)
            {
                Debug.LogError($"Failed to load data for key: {key}. Exception: {ex.Message}");
            }
        }
        [Obsolete("Use LoadAllDataWithExpiry instead to handle data expiration.")]
        public async void LoadAllData()
        {
            if (!m_UseCloudSave)
            {
                Debug.LogWarning("Cloud Save is disabled. Data will not be loaded.");
                return;
            }
            try
            {
                var playerData = await CloudSaveService.Instance.Data.Player.LoadAllAsync();

                var keys = new List<string>(playerData.Keys);
                for (int i = 0; i < keys.Count; i++)
                {
                    var key = keys[i];
                    var value = playerData[key];

                    Debug.Log($"Loaded key: {key}, value: {value}");

                    // Update local cache
                    if (m_PlayerData.ContainsKey(key))
                    {
                        m_PlayerData[key] = value;
                    }
                    else
                    {
                        m_PlayerData.Add(key, value);
                    }
                }
                OnDataLoadedInvoke();
            }
            catch (Exception ex)
            {
                Debug.LogError($"Failed to load all data. Exception: {ex.Message}");
            }
        }

        public T GetData<T>(string key)
        {
            if (!m_UseCloudSave)
            {
                Debug.LogWarning("Cloud Save is disabled. Data will not be loaded.");
                return default;
            }
            if (m_PlayerData.TryGetValue(key, out var value))
            {
                if (value is Item item)
                {
                    return item.Value.GetAs<T>();
                }
                else
                {
                    // Attempt to convert the value to the requested type
                    if (value is T typedValue)
                    {
                        return typedValue;
                    }
                    // If conversion fails, log an error
                    Debug.LogError($"Failed to convert data for key: {key} to type: {typeof(T)}. Value: {value}");
                }
            }
            else
            {
                Debug.LogWarning($"No data found for key: {key}");
            }

            return default;
        }

        public bool HasData(string key)
        {
            if (!m_UseCloudSave)
            {
                Debug.LogWarning("Cloud Save is disabled. Data will not be loaded.");
                return false;
            }
            return m_PlayerData.ContainsKey(key);
        }

        public async void DeleteData(string key)
        {
            try
            {
                // Updated to use the new DeleteAsync method with the correct namespace and options
                await CloudSaveService.Instance.Data.Player.DeleteAsync(key, new Unity.Services.CloudSave.Models.Data.Player.DeleteOptions());

                if (m_PlayerData.ContainsKey(key))
                {
                    m_PlayerData.Remove(key);
                }

                Debug.Log($"Successfully deleted data for key: {key}");
            }
            catch (Exception ex)
            {
                Debug.LogError($"Failed to delete data for key: {key}. Exception: {ex.Message}");
            }
        }

        public async void DeleteAllData()
        {
            try
            {
                await CloudSaveService.Instance.Data.Player.DeleteAllAsync();
                m_PlayerData.Clear();

                Debug.Log("Successfully deleted all data");
            }
            catch (Exception ex)
            {
                Debug.LogError($"Failed to delete all data. Exception: {ex.Message}");
            }
        }

        private void OnDataLoadedInvoke()
        {
            Debug.Log("Data loaded successfully");
            m_OnDataLoaded?.Invoke();
        }
        private bool IsExpired(long lastUpdateUnix, long ttlSeconds, long serverNow)
        {
            if (ttlSeconds <= 0)
                return false;

            return serverNow - lastUpdateUnix >= ttlSeconds;
        }
        private bool TryProcessExpiry(string key, long lastUpdateUnix, long ttlSeconds, long serverNow)
        {
            if (!IsExpired(lastUpdateUnix, ttlSeconds, serverNow))
                return false;

            // Expired → delete data
            CloudSaveService.Instance.Data.Player.DeleteAsync(key, new Unity.Services.CloudSave.Models.Data.Player.DeleteOptions());
            m_PlayerData.Remove(key);

            Debug.Log($"Cloud data expired: {key}");
            return true;
        }
        private long GetServerTimeUnix()
        {
            return DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        }
        public async void SaveData<T>(string key, T value, long ttlSeconds = 0, UnityAction callback = null)
        {
            if (!m_UseCloudSave)
                return;

            try
            {
                var wrapped = new CloudValue<T>
                {
                    value = value,
                    lastUpdateUnix = GetServerTimeUnix(),
                    ttlSeconds = ttlSeconds
                };

                m_PlayerData[key] = wrapped;

                await CloudSaveService.Instance.Data.Player.SaveAsync(
                    new Dictionary<string, object> { { key, wrapped } }
                );

                callback?.Invoke();
            }
            catch (Exception ex)
            {
                Debug.LogError($"SaveData failed for {key}: {ex.Message}");
            }
        }
        public async void LoadDataWithExpiry(string key, UnityAction<bool> callback = null)
        {
            if (!m_UseCloudSave)
                return;

            try
            {
                var keys = new HashSet<string> { key };
                var result = await CloudSaveService.Instance.Data.Player.LoadAsync(keys);

                if (!result.TryGetValue(key, out var item))
                {
                    callback?.Invoke(false);
                    return;
                }

                var raw = item.Value.GetAs<Dictionary<string, object>>();

                long lastUpdate = Convert.ToInt64(raw["lastUpdateUnix"]);
                long ttl = Convert.ToInt64(raw["ttlSeconds"]);
                long now = GetServerTimeUnix();

                if (TryProcessExpiry(key, lastUpdate, ttl, now))
                {
                    callback?.Invoke(false);
                    return;
                }

                m_PlayerData[key] = item;
                callback?.Invoke(true);
            }
            catch (Exception ex)
            {
                Debug.LogError($"LoadDataWithExpiry failed for {key}: {ex.Message}");
            }
        }
        public T GetDataValue<T>(string key)
        {
            if (!m_PlayerData.TryGetValue(key, out var obj))
                return default;

            if (obj is Item item)
            {
                var wrapped = item.Value.GetAs<CloudValue<T>>();
                return wrapped.value;
            }

            return default;
        }
        public async void LoadAllDataWithExpiry()
        {
            if (!m_UseCloudSave)
            {
                Debug.LogWarning("Cloud Save is disabled. Data will not be loaded.");
                return;
            }

            try
            {
                var playerData = await CloudSaveService.Instance.Data.Player.LoadAllAsync();
                long serverNow = GetServerTimeUnix();

                m_PlayerData.Clear();

                foreach (var pair in playerData)
                {
                    string key = pair.Key;
                    Item item = pair.Value;

                    // Try to read wrapped format
                    try
                    {
                        var raw = item.Value.GetAs<Dictionary<string, object>>();

                        if (!raw.ContainsKey("lastUpdateUnix") || !raw.ContainsKey("ttlSeconds"))
                        {
                            // Legacy data → keep as-is
                            m_PlayerData[key] = item;
                            continue;
                        }

                        long lastUpdate = Convert.ToInt64(raw["lastUpdateUnix"]);
                        long ttl = Convert.ToInt64(raw["ttlSeconds"]);

                        if (IsExpired(lastUpdate, ttl, serverNow))
                        {
                            // Expired → delete from cloud
                            await CloudSaveService.Instance.Data.Player.DeleteAsync(key);
                            Debug.Log($"Expired cloud data deleted: {key}");
                            continue;
                        }

                        // Valid → cache
                        m_PlayerData[key] = item;
                    }
                    catch
                    {
                        // Non-wrapped / unknown data → keep
                        m_PlayerData[key] = item;
                    }
                }

                OnDataLoadedInvoke();
            }
            catch (Exception ex)
            {
                Debug.LogError($"LoadAllDataWithExpiry failed: {ex.Message}");
            }
        }
    }
}
