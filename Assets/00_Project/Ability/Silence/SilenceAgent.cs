using UnityEngine;
using Rush;

namespace LegionKnight
{
    public class SilenceAgent : MonoBehaviour
    {
        [SerializeField]
        private PoolDefinition m_VfxDefi;
        [SerializeField]
        private float m_Duration;

        private GameObject m_Spawned;
        public void ApplySilenceOnSilentableObject(GameObject targetSilence)
        {
            Silence s = targetSilence.GetComponentInChildren<Silence>();
            if (s != null)
            {
                PoolManager.Instance.Spawn(m_VfxDefi.Id, s.SilenceVFXParent, true, out m_Spawned);
                s.TriggerSilenced(m_Duration);
                Debug.Log("Silenced");
            }
            else
            {
                Debug.Log("This cant be Silenced");
            }
        }

        private void DisableSpawned()
        {
            m_Spawned.SetActive(false);
        }
    }
}
