using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace LegionKnight
{
    public partial class ObjectPause : MonoBehaviour
    {
        [SerializeField]
        private List<MonoBehaviour> m_Behaviours = new();

        [SerializeField]
        private UnityEvent m_OnPaused = new();
        [SerializeField]
        private UnityEvent m_OnUnPaused = new();

        private bool m_Paused;
        public void SetPause(bool set)
        {
            m_Paused = set;
            foreach (MonoBehaviour mono in m_Behaviours)
            {
                mono.enabled = !m_Paused;
            }
            if (m_Paused)
            {
                OnPausedInvoke();
            }
            else
            {
                OnUnPausedInvoke();
            }
        }

        private void OnPausedInvoke()
        {
            m_OnPaused?.Invoke();
        }
        private void OnUnPausedInvoke()
        {
            m_OnUnPaused?.Invoke();
        }
    }
}
