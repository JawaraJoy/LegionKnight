using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

namespace LegionKnight
{
    public class DamageableDirection : Damageable
    {
        [SerializeField]
        private UnityEvent m_ContactDeathOnRight = new();
        [SerializeField]
        private UnityEvent m_ContactDeathOnLeft = new();

        private int m_lastDirection = 0;
        protected override void OnContactedBehaviourInvoke(GameObject other)
        {
            base.OnContactedBehaviourInvoke(other);
            OnDeathDirection(other);
        }
        private int debugCount = 0;
        private void OnDeathDirection(GameObject other)
        {
            if (other.TryGetComponent(out Contact2D contact))
            {
                if (!IsAlive())
                {
                    Vector2 contactPoint = contact.transform.position;
                    if (IsContactFromRight(contactPoint))
                    {
                        m_lastDirection = 1;
                    }
                    else
                    {
                        m_lastDirection = -1;
                    }
                    OnDeathInvoke();
                    Debug.Log($"[Death direction] invoked {debugCount++} times.");
                }
            }
        }

        private bool IsContactFromRight(Vector2 contactPoint)
        {
            return contactPoint.x > transform.position.x;
        }
        protected override void DeathHandler()
        {
            if (IsAlive()) return;
            if (m_lastDirection > 0)
            {
                m_ContactDeathOnLeft?.Invoke();
            }
            else
            {
                m_ContactDeathOnRight?.Invoke();
            }
            OnDeathInvoke();
            //m_ContactDeathOnRight?.Invoke();
        }
    }
}
