using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace Rush
{
    public class Initiator : MonoBehaviour
    {
        [SerializeField]
        private UnityEvent m_OnStart;

        IEnumerator Start()
        {
            yield return new WaitForSeconds(1f);
            m_OnStart?.Invoke();
        }
    }
}
