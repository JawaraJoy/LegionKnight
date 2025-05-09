using System;
using System.Collections.Generic;
using Unity.Services.CloudSave;
using Unity.Services.CloudSave.Models;
using Unity.Services.Core;
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
                await CloudSaveService.Instance.Data.Player.DeleteAsync(key);

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
    }
}
