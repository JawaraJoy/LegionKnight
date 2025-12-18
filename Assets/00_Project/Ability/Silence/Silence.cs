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

        public void TriggerSilenced(ParticleSystem vfxPrefab, float duration)
        {

        }
    }
}
