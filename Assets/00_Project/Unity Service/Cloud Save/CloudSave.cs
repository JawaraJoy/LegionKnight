using System.Collections.Generic;
using Unity.Services.CloudSave;
using UnityEngine;

namespace LegionKnight
{
    public partial class CloudSave : MonoBehaviour
    {
        private readonly Dictionary<string, object> m_PlayerData = new ();
        public async void SaveData(string key, object val)
        {
            var playerData = new Dictionary<string, object> { { key, val } };
            // Save the data to the cloud
            if (m_PlayerData.ContainsKey(key))
            {
                m_PlayerData[key] = val;
            }
            else
            {
                m_PlayerData.Add(key, val);
            }
            await CloudSaveService.Instance.Data.Player.SaveAsync(m_PlayerData);
            Debug.Log($"Saved data {string.Join(',', playerData)}");
        }
        public async void LoadData(string key)
        {
            // Load the data from the cloud
            var keys = new HashSet<string> { key }; // Convert List<string> to ISet<string>
            var playerData = await CloudSaveService.Instance.Data.Player.LoadAsync(keys);
            if (playerData.TryGetValue(key, out var val))
            {
                Debug.Log($"Loaded data {key} : {val}");
            }
            else
            {
                Debug.Log($"No data found for key: {key}");
            }
            foreach (var kvp in playerData)
            {
                Debug.Log($"Key: {kvp.Key}, Value: {kvp.Value}");
                if (m_PlayerData.ContainsKey(kvp.Key))
                {
                    m_PlayerData[kvp.Key] = kvp.Value;
                }
                else
                {
                    m_PlayerData.Add(kvp.Key, kvp.Value);
                }
            }
        }

        [System.Obsolete]
        public async void DeleteData(string key)
        {
            // Delete the data from the cloud
            await CloudSaveService.Instance.Data.Player.DeleteAsync(key); // Pass a single string instead of HashSet<string>
            if (m_PlayerData.ContainsKey(key))
            {
                m_PlayerData.Remove(key);
            }
            Debug.Log($"Deleted data for key: {key}");
        }
        public async void DeleteAllData()
        {
            // Delete all data from the cloud
            await CloudSaveService.Instance.Data.Player.DeleteAllAsync();
            m_PlayerData.Clear();
            Debug.Log("Deleted all data");
        }
        public object GetData(string key)
        {
            return GetDataInternal(key);
        }
        private object GetDataInternal(string key)
        {
            // Get the data from the local dictionary
            if (m_PlayerData.TryGetValue(key, out var val))
            {
                Debug.Log($"Get data {key} : {val}");
                return val;
            }
            else
            {
                Debug.Log($"No data found for key: {key}");
                return null;
            }
        }
        public T GetData<T>(string key)
        {
            // Get the data from the local dictionary and cast it to the specified type
            if (m_PlayerData.TryGetValue(key, out var val))
            {
                Debug.Log($"Get data {key} : {val}");
                if (val is T t)
                {
                    return t;
                }
                else
                {
                    Debug.Log($"Data for key {key} is not of type {typeof(T)}");
                    return default;
                }
            }
            else
            {
                Debug.Log($"No data found for key: {key}");
                return default;
            }
        }
    }
}
