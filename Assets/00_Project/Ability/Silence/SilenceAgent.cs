using UnityEngine;

namespace LegionKnight
{
    public class SilenceAgent : MonoBehaviour
    {
        [SerializeField]
        private GameObject m_VfxPrefab;
        [SerializeField]
        private float m_Duration;
        public void ApplySilenceOnSilentableObject(GameObject targetSilence)
        {
            Silence s = targetSilence.GetComponentInChildren<Silence>();
            if (s != null)
            {
                s.TriggerSilenced(m_VfxPrefab, m_Duration);
                Debug.Log("Silenced");
            }
            else
            {
                Debug.Log("This cant be Silenced");
            }
        }
    }
}
