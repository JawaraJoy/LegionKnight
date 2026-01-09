using UnityEngine;
using UnityEngine.Events;

namespace Rush
{
    [System.Serializable]
    public class ClipEventField
    {
        [SerializeField]
        private ClipEventConfig m_EventConfig;
        [SerializeField]
        private UnityEvent<ClipEventConfig> m_OnTriggered;
        public ClipEventConfig EventConfig => m_EventConfig;
        public void OnTriggeredInvoke()
        {
            m_OnTriggered?.Invoke(m_EventConfig);
            Debug.Log($"[Spine Event]{m_EventConfig.BaseInfo.Name} is triggered");
        }
    }
}
