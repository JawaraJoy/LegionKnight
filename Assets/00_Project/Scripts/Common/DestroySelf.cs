using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace LegionKnight
{
    public partial class DestroySelf : MonoBehaviour
    {
        [SerializeField]
        private float m_Delay = 1f;
        [SerializeField]
        private UnityEvent m_OnStartDestroy = new();
        [SerializeField]
        private UnityEvent m_OnDestroyDone = new();
        public void DestroyMe()
        {
            StartCoroutine(DestroyingMe());
        }
        private IEnumerator DestroyingMe()
        {
            m_OnStartDestroy?.Invoke();
            yield return new WaitForSeconds(m_Delay);
            m_OnDestroyDone?.Invoke();
            Destroy(gameObject);
        }
    }
}
