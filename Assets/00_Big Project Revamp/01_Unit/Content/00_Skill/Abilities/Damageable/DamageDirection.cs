using UnityEngine;
using UnityEngine.Events;

namespace Rush
{
    public class DamageDirection : MonoBehaviour
    {
        [SerializeField]
        private UnityEvent m_OnRight = new();
        [SerializeField]
        private UnityEvent m_OnLeft = new();
        public void OnDirection(BattleContext context)
        {
            if (context.Attacker is Attacker attacker)
            {
                Vector2 attackerDirection = attacker.transform.position;
                if (IsContactFromRight(attackerDirection))
                {
                    m_OnLeft.Invoke();
                }
                else
                {
                    m_OnRight.Invoke();
                }
            }
        }

        private bool IsContactFromRight(Vector2 contactPoint)
        {
            return contactPoint.x > transform.position.x;
        }
    }
}
