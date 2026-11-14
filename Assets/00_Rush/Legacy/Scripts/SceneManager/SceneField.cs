using Rush;
using System.Collections;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Events;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;

namespace LegionKnight
{
    [System.Serializable]
    public partial class SceneField
    {
        [SerializeField] private SceneConfig m_SceneConfig;
        [SerializeField] private UnityEvent m_OnSceneLoaded = new();
        [SerializeField] private UnityEvent m_OnSceneUnLoaded = new();
        private AsyncOperationHandle<SceneInstance> m_RemoteHandle;
        public SceneConfig SceneConfig => m_SceneConfig;

        public void LoadScene()
        {
            GameTimeScale.SetTimeScale(1f);
            CanvasManager.Instance.ShowPanel(PanelId.LoadingPanelId);
            if (Application.CanStreamedLevelBeLoaded(m_SceneConfig.SceneName))
                SceneManager.LoadSceneAsync(m_SceneConfig.SceneName, m_SceneConfig.Mode).completed += HandleSceneLoadedInternal;
            else
            {
                m_RemoteHandle = Addressables.LoadSceneAsync(m_SceneConfig.SceneName, m_SceneConfig.Mode, true);
                m_RemoteHandle.Completed += HandleRemoteSceneLoadedInternal;
            }
        }

        private void HandleSceneLoadedInternal(AsyncOperation handle)
        {
            if (handle.isDone) GameManager.Instance.StartCoroutine(HidingLoadSceneInternal());
        }
        private void HandleRemoteSceneLoadedInternal(AsyncOperationHandle<SceneInstance> handle)
        {
            if (handle.Status == AsyncOperationStatus.Succeeded)
                GameManager.Instance.StartCoroutine(HidingLoadSceneInternal());
            else
            {
                Debug.LogError($"Could not load scene '{m_SceneConfig.SceneName}': {handle.OperationException}");
                CanvasManager.Instance.HidePanel(PanelId.LoadingPanelId);
                if (handle.IsValid()) Addressables.Release(handle);
            }
        }
        public void UnloadScene()
        {
            if (m_RemoteHandle.IsValid() && m_RemoteHandle.Status == AsyncOperationStatus.Succeeded)
                Addressables.UnloadSceneAsync(m_RemoteHandle).Completed += HandleRemoteSceneUnloadedInternal;
            else
            {
                var scene = SceneManager.GetSceneByName(m_SceneConfig.SceneName);
                if (scene.IsValid() && scene.isLoaded)
                    SceneManager.UnloadSceneAsync(scene).completed += HandleSceneUnloadedInternal;
                else Debug.LogWarning($"Scene '{m_SceneConfig.SceneName}' is not loaded.");
            }
        }
        private void HandleSceneUnloadedInternal(AsyncOperation handle)
        {
            if (handle.isDone) OnSceneUnLoadedInvoke();
        }
        private void HandleRemoteSceneUnloadedInternal(AsyncOperationHandle<SceneInstance> handle)
        {
            if (handle.Status == AsyncOperationStatus.Succeeded) OnSceneUnLoadedInvoke();
        }
        private IEnumerator HidingLoadSceneInternal()
        {
            yield return new WaitForEndOfFrame();
            yield return new WaitForSeconds(m_SceneConfig.HideLoadingPanelDelay);
            CanvasManager.Instance.HidePanel(PanelId.LoadingPanelId);
            OnSceneLoadedInvoke();
        }
        private void OnSceneLoadedInvoke() => m_OnSceneLoaded?.Invoke();
        private void OnSceneUnLoadedInvoke() => m_OnSceneUnLoaded?.Invoke();
    }
}
