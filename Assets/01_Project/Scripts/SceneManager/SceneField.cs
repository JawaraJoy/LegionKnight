using System.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace LegionKnight
{
    [System.Serializable]
    public partial class SceneField
    {
        [SerializeField]
        private SceneAsset m_SceneAsset;
        [SerializeField]
        private LoadSceneMode m_Mode;
        [SerializeField]
        private float m_HideLoadingPanelDelay;
        [SerializeField]
        private UnityEvent m_OnSceneLoaded = new();
        [SerializeField]
        private UnityEvent m_OnSceneUnLoaded = new();
        private AsyncOperation m_Handle;
        public SceneAsset SceneAsset => m_SceneAsset;
        public void LoadScene()
        {
            GameTimeScale.SetTimeScale(1f);
            CanvasManager.Instance.ShowPanel(PanelId.LoadingPanelId);
            //m_SceneName.LoadSceneAsync(m_Mode).Completed += OnSceneLoadedInvoke;
            SceneManager.LoadSceneAsync(m_SceneAsset.name, m_Mode).completed += OnSceneLoadedInvoke;
        }

        private void OnSceneLoadedInvoke(AsyncOperation handle)
        {
            if (handle.isDone)
            {
                Debug.Log($"Scene '{m_SceneAsset}' loaded successfully!");
                m_Handle = handle; // Store the handle for unloading
                GameManager.Instance.StartCoroutine(HidingLoadScene());
            }
            else
            {
                Debug.LogError($"Failed to load scene '{m_SceneAsset}'.");
            }
        }
        public void UnloadScene()
        {
            if (m_Handle.isDone)
            {
                //Addressables.UnloadSceneAsync(m_Handle).Completed += OnSceneUnLoadedInvoke;
                SceneManager.UnloadSceneAsync(m_SceneAsset.name).completed += OnSceneUnLoadedInvoke;
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
                Debug.Log($"Scene '{m_SceneAsset}' unloaded successfully!");
                m_OnSceneUnLoaded?.Invoke();
            }
            else
            {
                Debug.LogError($"Failed to unload scene '{m_SceneAsset}'.");
            }
        }

        private IEnumerator HidingLoadScene()
        {
            yield return new WaitForEndOfFrame();
            
            yield return new WaitForSeconds(m_HideLoadingPanelDelay);
            CanvasManager.Instance.HidePanel(PanelId.LoadingPanelId);
            m_OnSceneLoaded?.Invoke();
        }
    }
}
