
using UnityEngine;
using UnityEngine.Events;

namespace LegionKnight
{
    public partial class CloudSaveManager : CloudSave
    {
        
    }

    public partial class UnityService
    {
        [SerializeField]
        private CloudSaveManager m_CloudSaveManager;
        public CloudSaveManager CloudSave => m_CloudSaveManager;

        public void LoadAllData()
        {
            //m_CloudSaveManager.LoadAllData();
            m_CloudSaveManager.LoadAllDataWithExpiry();
        }
        public void SaveData(string key, object val, UnityAction callback = null)
        {
            Debug.Log("----- " + key + " - " + val.GetType().ToString());

            m_CloudSaveManager.SaveData(key, val, callback);
        }
        public bool HasData(string key)
        {
            return m_CloudSaveManager.HasData(key);
        }
        public void LoadData(string key, UnityAction callback = null)
        {
            m_CloudSaveManager.LoadData(key, callback);
        }

        [System.Obsolete]
        public void DeleteData(string key)
        {
            m_CloudSaveManager.DeleteData(key);
        }
        public void DeleteAllData()
        {
            m_CloudSaveManager.DeleteAllData();
        }
        public T GetData<T>(string key)
        {
            return m_CloudSaveManager.GetData<T>(key);
        }
    }
}
