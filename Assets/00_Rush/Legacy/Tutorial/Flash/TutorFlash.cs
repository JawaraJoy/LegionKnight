using Rush;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace LegionKnight
{
    public class TutorFlash : MonoBehaviour
    {
        [SerializeField] private Transform m_FlashContainer;
        [SerializeField] private TutorFlashDefinition m_Definition;
        [SerializeField] private TutorialFlashChannel m_Channel;
        private TutorFlashUI m_SpawnedFlashUI;
        private AsyncOperationHandle<GameObject> m_Handle;
        private bool m_Loading;
        private bool m_Disposed;
        private bool m_ShouldShow;
        private string m_Message;
        public TutorFlashDefinition Definition => m_Definition;
        public TutorFlashUI SpawnedFlashUI => m_SpawnedFlashUI;
        public bool IsReady => m_SpawnedFlashUI != null;
        private void OnEnable() { if (Application.isPlaying) m_Channel?.Register(this); }
        private void OnDisable()
        {
            m_Channel?.Unregister(this);
            HideFlashInternal();
        }
        private void OnDestroy()
        {
            m_Disposed = true;
            m_Channel?.Unregister(this);
            if (!m_Loading && m_Handle.IsValid()) Addressables.ReleaseInstance(m_Handle);
        }
        public void ShowFlashUI(string message)
        {
            m_Message = message;
            m_ShouldShow = true;
            if (m_SpawnedFlashUI != null) { m_SpawnedFlashUI.ShowFlash(message); return; }
            if (m_Loading || m_Disposed || m_Definition == null || m_Definition.FlashUIAsset == null || !m_Definition.FlashUIAsset.RuntimeKeyIsValid()) return;
            m_Loading = true;
            m_Handle = m_Definition.FlashUIAsset.InstantiateAsync(m_FlashContainer, false);
            m_Handle.Completed += OnSpawned;
        }
        private void OnSpawned(AsyncOperationHandle<GameObject> handle)
        {
            m_Loading = false;
            if (m_Disposed || this == null)
            {
                if (handle.Status == AsyncOperationStatus.Succeeded) Addressables.ReleaseInstance(handle);
                else Addressables.Release(handle);
                return;
            }
            if (handle.Status != AsyncOperationStatus.Succeeded)
            {
                Addressables.Release(handle);
                return;
            }
            if (!handle.Result.TryGetComponent(out m_SpawnedFlashUI))
            {
                Addressables.ReleaseInstance(handle);
                return;
            }
            if (m_ShouldShow && isActiveAndEnabled) m_SpawnedFlashUI.ShowFlash(m_Message);
            else m_SpawnedFlashUI.Hide();
        }
        public void HideFlashUI() => HideFlashInternal();
        private void HideFlashInternal()
        {
            m_ShouldShow = false;
            m_SpawnedFlashUI?.Hide();
        }
    }
}
