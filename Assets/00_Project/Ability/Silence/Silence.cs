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

        public void TriggerSilenced(GameObject vfxPrefab, float duration)
        {
            StartCoroutine(TriggeringSilence(vfxPrefab, duration));
        }

        private IEnumerator TriggeringSilence(GameObject vfxPrefab, float duration)
        {
            m_OnTriggered?.Invoke();
            GameObject vfxInstance = null;
            if (vfxPrefab.TryGetComponent(out PoolObject poolObject))
            {
                bool hasPool = ContainerPooling.HasUnitPool(poolObject.Definition.Id);
                if (hasPool)
                {
                    UnitPool pool = ContainerPooling.GetUnitPool(poolObject.Definition.Id);
                    pool.ReSpawn(m_SilenceVFXParent, false, out GameObject selected);
                    vfxInstance = selected;
                }
                else
                {
                    vfxInstance = Instantiate(vfxPrefab, m_SilenceVFXParent);
                    ContainerPooling.AddUnitPool(poolObject);
                }
            }
            else
            {
                vfxInstance = Instantiate(vfxPrefab, m_SilenceVFXParent);
            }
            yield return new WaitForSeconds(duration);
            if (vfxInstance != null)
            {
                vfxInstance.SetActive(false);
            }
            m_OnRemoved?.Invoke();
        }
    }
}
