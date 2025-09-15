using UnityEngine;
using UnityEngine.Events;

namespace LegionKnight
{
    public class GuardActivation : MonoBehaviour
    {
        [SerializeField]
        protected Sprite m_GuardSprite;
        [SerializeField]
        protected float m_GuardDuration = 5f;

        [SerializeField]
        private UnityEvent m_OnContact = new();
        [SerializeField]
        private UnityEvent m_OnGuardActive = new();
        private void Start()
        {
            GuardActive();
        }

        protected virtual void GuardActive()
        {
            m_OnGuardActive?.Invoke();
        }
    }
}
