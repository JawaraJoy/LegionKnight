using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Events;

namespace LegionKnight
{
    [CreateAssetMenu(fileName = "Flash", menuName = "Legion Knight/Tutorial/Flash")]
    public class TutorFlashDefinition : ScriptableObject
    {
        [SerializeField]
        private string m_Id = string.Empty;
        [SerializeField, TextArea]
        private string[] m_FlashMessages;
        [SerializeField]
        private float m_MessageInterval;
        [SerializeField]
        private bool m_SetIsDoneOnStart = false;
        [SerializeField]
        private AssetReferenceGameObject m_FlashUIAsset;
        [SerializeField]
        private UnityEvent m_OnStart;
        [SerializeField]
        private UnityEvent m_OnEnd;
        public UnityEvent OnStart => m_OnStart;
        public UnityEvent OnEnd => m_OnEnd;
        public float MessageInternal => m_MessageInterval;
        public AssetReferenceGameObject FlashUIAsset => m_FlashUIAsset;
        public string[] FlashMessages => m_FlashMessages;
        public string Id => m_Id;
        public bool IsSetDoneOnStart => m_SetIsDoneOnStart;

        public void StartFlash()
        {
            GameManager.Instance.TutorFlash.StartFlash(this);
        }
    }
}
