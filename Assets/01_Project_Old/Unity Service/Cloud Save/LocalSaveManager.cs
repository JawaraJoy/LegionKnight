
using UnityEngine;
using UnityEngine.Events;

namespace LegionKnight
{
    // terpaksa memakai local karena cloud penuh
    public partial class LocalSaveManager : LocalSave
    {
        
    }

    public partial class UnityService
    {
        [SerializeField]
        private LocalSaveManager m_LocalSaveManager;
        public LocalSaveManager LocalSave => m_LocalSaveManager;

        public void LoadAllData()
        {
            //m_CloudSaveManager.LoadAllData();
            m_LocalSaveManager.LoadAllDataWithExpiry();
        }
        public void SaveData(string key, object val, UnityAction callback = null)
        {
            Debug.Log("----- " + key + " - " + val.GetType().ToString());

            m_LocalSaveManager.SaveData(key, val, callback);
        }
        public bool HasData(string key)
        {
            return m_LocalSaveManager.HasData(key);
        }
        public void LoadData(string key, UnityAction callback = null)
        {
            m_LocalSaveManager.LoadData(key, callback);
        }

        [System.Obsolete]
        public void DeleteData(string key)
        {
            m_LocalSaveManager.DeleteData(key);
        }
        public void DeleteAllData()
        {
            m_LocalSaveManager.DeleteAllData();
        }
        public T GetData<T>(string key)
        {
            return m_LocalSaveManager.GetData<T>(key);
        }
    }
}
