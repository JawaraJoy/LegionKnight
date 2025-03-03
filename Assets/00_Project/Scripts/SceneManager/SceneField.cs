using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace LegionKnight
{
    [System.Serializable]
    public partial class SceneField
    {
        [SerializeField]
        private string m_SceneName;
        [SerializeField]
        private LoadSceneMode m_Mode;
        [SerializeField]
        private float m_HideLoadingPanelDelay;
        [SerializeField]
        private UnityEvent m_OnSceneLoaded = new();
        [SerializeField]
        private UnityEvent m_OnSceneUnLoaded = new();
        private AsyncOperation m_Handle;
        public string SceneName => m_SceneName;
        public void LoadScene()
        {
            GameTimeScale.SetTimeScale(1f);
            GameManager.Instance.ShowPanel(PanelId.LoadingPanelId);
            //m_SceneName.LoadSceneAsync(m_Mode).Completed += OnSceneLoadedInvoke;
            SceneManager.LoadSceneAsync(m_SceneName, m_Mode).completed += OnSceneLoadedInvoke;
        }

        private void OnSceneLoadedInvoke(AsyncOperation handle)
        {
            if (handle.isDone)
            {
                Debug.Log($"Scene '{m_SceneName}' loaded successfully!");
                m_Handle = handle; // Store the handle for unloading
                GameManager.Instance.StartCoroutine(HidingLoadScene());
            }
            else
            {
                Debug.LogError($"Failed to load scene '{m_SceneName}'.");
            }
        }
        public void UnloadScene()
        {
            if (m_Handle.isDone)
            {
                //Addressables.UnloadSceneAsync(m_Handle).Completed += OnSceneUnLoadedInvoke;
                SceneManager.UnloadSceneAsync(m_SceneName).completed += OnSceneUnLoadedInvoke;
            }
            else
            {
                Debug.LogWarning("No valid scene handle found. Did you load the scene first?");
            }
        }

        private void OnSceneUnLoadedInvoke(AsyncOperation handle)
        {
            if (handle.isDone)
            {
                Debug.Log($"Scene '{m_SceneName}' unloaded successfully!");
                m_OnSceneUnLoaded?.Invoke();
            }
            else
            {
                Debug.LogError($"Failed to unload scene '{m_SceneName}'.");
            }
        }

        private IEnumerator HidingLoadScene()
        {
            yield return new WaitForSeconds(m_HideLoadingPanelDelay);
            GameManager.Instance.HidePanel(PanelId.LoadingPanelId);
            m_OnSceneLoaded?.Invoke();
        }
    }
}
