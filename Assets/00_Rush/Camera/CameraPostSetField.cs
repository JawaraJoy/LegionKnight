using UnityEngine;
using UnityEngine.Events;

namespace Rush
{
    [System.Serializable]
    public class CameraPostSetField
    {
        [SerializeField]
        private CameraPostSetConfig m_Config;
        [SerializeField]
        private UnityEvent m_OnPostStartSet = new();
        [SerializeField]
        private UnityEvent m_OnPostEndSet = new();
        public CameraPostSetConfig Config => m_Config;
        public void OnPostStartSetInvoke()
        {
            m_OnPostStartSet?.Invoke();
        }
        public void OnPostEndSetInvoke()
        {
            m_OnPostEndSet?.Invoke();
        }
    }
}
