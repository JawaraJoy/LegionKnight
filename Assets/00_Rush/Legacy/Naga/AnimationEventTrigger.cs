using UnityEngine;
using UnityEngine.Events;

namespace LegionKnight
{
    public class AnimationEventTrigger : MonoBehaviour
    {
        [SerializeField]
        private UnityEvent m_OnEventTriggered;
        public void OnGone()
        {
            // You can add additional logic here that should happen when the event is triggered
            m_OnEventTriggered?.Invoke();
        }
    }
}
