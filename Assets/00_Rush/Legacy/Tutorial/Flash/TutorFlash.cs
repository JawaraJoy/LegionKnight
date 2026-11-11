using System.Collections;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace LegionKnight
{
    public class TutorFlash : MonoBehaviour
    {
        [SerializeField]
        private Transform m_FlashContainer;

        private TutorFlashUI m_SpawnedFlashUI;
        [SerializeField]
        private TutorFlashDefinition m_Definition;
        public TutorFlashDefinition Definition => m_Definition;

        private TutorFlashHandler m_FlashHandler;
        public TutorFlashUI SpawnedFlashUI => m_SpawnedFlashUI;

        private TutorFlashHandler GetHandler()
        {
            if (m_FlashHandler == null)
            {
                m_FlashHandler = GameManager.Instance.TutorFlash;
            }
            return m_FlashHandler;
        }
        private void Start()
        {
            GetHandler().AddFlash(this);
        }
        private void OnDestroy()
        {
            GetHandler().RemoveFlash(this);
        }
        public void ShowFlashUI(string message)
        {
            if (m_SpawnedFlashUI != null)
            {
                m_SpawnedFlashUI.ShowFlash(message);
            }
            else
            {
                StartCoroutine(SpawningFlashUI(message));
            }    
        }
        public void HideFlashUI()
        {
            if (m_SpawnedFlashUI == null) return;
            m_SpawnedFlashUI.Hide();
        }
        private IEnumerator SpawningFlashUI(string message)
        {
            AssetReferenceGameObject flasshAsset = m_Definition.FlashUIAsset;
            AsyncOperationHandle<GameObject> handle = flasshAsset.InstantiateAsync(m_FlashContainer, false);
            yield return handle;
            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                GameObject result = handle.Result;
                if (result.TryGetComponent(out TutorFlashUI view))
                {
                    view.ShowFlash(message);
                    m_SpawnedFlashUI = view;
                }
            }
        }
    }
}
