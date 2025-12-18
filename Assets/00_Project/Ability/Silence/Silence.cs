using System.Collections;
using UnityEngine;
using UnityEngine.Events;

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

        private float m_SilenceDuration;

        public void TriggerSilenced(GameObject vfxPrefab, float duration)
        {

        }

        private IEnumerator TriggeringSilence(GameObject vfxPrefab, float duration)
        {
            m_OnTriggered?.Invoke();
            GameObject vfxInstance = null;
            if (vfxPrefab != null && m_SilenceVFXParent != null)
            {
                vfxInstance = Instantiate(vfxPrefab, m_SilenceVFXParent);
            }
            yield return new WaitForSeconds(duration);
            if (vfxInstance != null)
            {
                Destroy(vfxInstance);
            }
            m_OnRemoved?.Invoke();
        }
    }
}
