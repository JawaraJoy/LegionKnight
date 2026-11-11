using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace Rush
{
    public class Delayer : MonoBehaviour
    {
        [SerializeField]
        private float m_DelayTime = 1f;

        [SerializeField]
        private UnityEvent m_OnStartExecuted;
        [SerializeField]
        private UnityEvent m_OnExecuted;

        public void Execute()
        {
            StartCoroutine(Executing());
        }
private IEnumerator Executing()
        {
            OnStartExecutedInvoke();
            yield return new WaitForSeconds(m_DelayTime);
            OnExecutedInvoke();
        }

private void OnStartExecutedInvoke()
        {
            m_OnStartExecuted?.Invoke();
        }

        private void OnExecutedInvoke()
        {
            m_OnExecuted?.Invoke();
        }

    }
}
