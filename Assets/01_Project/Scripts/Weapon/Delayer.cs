using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace LegionKnight
{
    public class Delayer : MonoBehaviour
    {
        [SerializeField]
        private float m_DelaySeconds = 1f;
        [SerializeField]
        private UnityEvent m_OnDelayComplete;

        public void StartDelay()
        {
            StartCoroutine(Delaying());
        }
        private IEnumerator Delaying()
        {
            yield return new WaitForSeconds(m_DelaySeconds);
            m_OnDelayComplete?.Invoke();
        }
    }
}
