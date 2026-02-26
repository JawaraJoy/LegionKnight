using UnityEngine;
using UnityEngine.Events;

namespace Rush
{
    public class Initiator : Singleton<Initiator>
    {
        [SerializeField]
        private UnityEvent m_OnStart;

        private void Start()
        {
            m_OnStart?.Invoke();
        }
    }
}
