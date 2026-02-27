using MoreMountains.Tools;
using UnityEngine;
using UnityEngine.Events;

namespace Rush
{
    public class DamageDirection : MonoBehaviour
    {
        [SerializeField]
        private UnityEvent m_OnLeft;
        [SerializeField]
        private UnityEvent m_OnRight;

        public void OnDeathDirection(Transform hitter)
        {
            if (IsContactFromRight(hitter.position))
            {
                m_OnRight.Invoke();
            }
            else
            {
                m_OnLeft.Invoke();
            }
        }
        private bool IsContactFromRight(Vector2 contactPoint)
        {
            return contactPoint.x > transform.position.x;
        }
    }
}
