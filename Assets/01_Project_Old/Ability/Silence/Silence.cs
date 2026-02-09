using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using Rush;

namespace LegionKnight
{
    public class Silence : MonoBehaviour
    {
        [SerializeField]
        private UnityEvent m_OnTriggered;
        [SerializeField]
        private UnityEvent m_OnRemoved;
        [SerializeField]
        private Transform m_SilenceVFXParent;

        public Transform SilenceVFXParent => m_SilenceVFXParent;

        private Coroutine m_Coroutine;
        public void TriggerSilenced(float duration)
        {
            if (m_Coroutine == null)
            {
                m_Coroutine = StartCoroutine(TriggeringSilence(duration));
            }
        }

        private IEnumerator TriggeringSilence(float duration)
        {
            m_OnTriggered?.Invoke();
            yield return new WaitForSeconds(duration);
            m_OnRemoved?.Invoke();
            foreach (MonoBehaviour vfx in GetComponentsInChildren<MonoBehaviour>())
            {
                if (vfx.TryGetComponent(out PoolObject poolObject))
                {
                    PoolManager.Instance.Despawn(poolObject.Definition.Id, poolObject.gameObject);
                }
            }
            StopCoroutine(m_Coroutine);
            m_Coroutine = null;
        }
    }
}
