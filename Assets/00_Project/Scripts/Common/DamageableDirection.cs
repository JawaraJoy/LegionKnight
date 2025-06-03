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
        protected override void OnContactedBehaviourInvoke(GameObject other)
        {
            base.OnContactedBehaviourInvoke(other);
            OnDeathDirection(other);
        }

        private void OnDeathDirection(GameObject other)
        {
            
            if (other.TryGetComponent(out Contact2D contact))
            {
                
                if (m_CurrentHealth <= 0)
                {
                    Vector2 contactPoint = contact.transform.position;
                    if (IsContactFromRight(contactPoint))
                    {
                        m_ContactDeathOnRight?.Invoke();
                    }
                    else
                    {
                        m_ContactDeathOnLeft?.Invoke();
                    }
                    OnDeathInvoke();
                }
            }
        }

        private bool IsContactFromRight(Vector2 contactPoint)
        {
            return contactPoint.x > transform.position.x;
        }
        protected override void DeathHandler()
        {
            //base.DeathHandler();
        }
    }
}
