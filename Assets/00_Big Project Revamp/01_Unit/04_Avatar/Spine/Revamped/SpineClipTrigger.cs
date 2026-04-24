using UnityEngine;
using UnityEngine.Events;

namespace Rush
{
    public class SpineClipTrigger : MonoBehaviour
    {
        [SerializeField]
        private AnimationClipConfig m_ClipConfig;
        [SerializeField]
        private UnityEvent m_OnStart;
        [SerializeField]
        private UnityEvent m_OnDone;
        public AnimationClipConfig ClipConfig => m_ClipConfig;

        public void OnStartInvoke(AnimationClipConfig clipConfig)
        {
            if (m_ClipConfig != clipConfig) return;
            m_OnStart?.Invoke();
            Debug.Log($"Clip is start {clipConfig.BaseInfo.Name}");
        }
        public void OnDoneInvoke(AnimationClipConfig clipConfig)
        {
            if (m_ClipConfig != clipConfig) return;
            m_OnDone?.Invoke();
            Debug.Log($"Clip is Done {clipConfig.BaseInfo.Name}");
        }
    }
}
