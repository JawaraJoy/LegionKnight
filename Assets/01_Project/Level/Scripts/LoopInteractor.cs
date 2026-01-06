using UnityEngine;
using UnityEngine.Events;

namespace LegionKnight
{
    public class LoopInteractor : MonoBehaviour
    {
        [SerializeField]
        private UnityEvent<GameObject> m_OnLoopTrigger = new();

        public void OnLoopTriggerInvoke(GameObject other)
        {
            if (other != null)
            {
                m_OnLoopTrigger?.Invoke(other);
            }
        }

    }
}
