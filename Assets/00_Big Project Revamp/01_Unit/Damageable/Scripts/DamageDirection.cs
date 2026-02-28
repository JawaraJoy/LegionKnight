using MoreMountains.Tools;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace Rush
{
    public class DamageDirection : MonoBehaviour
    {
        [SerializeField, MMReadOnly]
        private PlatformDirection m_Direction = PlatformDirection.Left;
        [SerializeField]
        private UnityEvent m_OnLeft;
        [SerializeField]
        private UnityEvent m_OnRight;

        [SerializeField]
        private UnityEvent m_OnSendDirectionRight;
        [SerializeField]
        private UnityEvent m_OnSendDirectionLeft;

        public void SendDirection()
        {
            StartCoroutine(SendingDirection());
        }
        private IEnumerator SendingDirection()
        {
            yield return new WaitForEndOfFrame();
            if (m_Direction == PlatformDirection.Right)
            {
                m_OnSendDirectionRight.Invoke();
            }
            else
            {
                m_OnSendDirectionLeft.Invoke();
            }
        }
        private void OnTriggerEnter2D(Collider2D collision)
        {
            Vector2 targetPost = collision.transform.position;
            if (IsContactFromRight(targetPost))
            {
                m_Direction = PlatformDirection.Right;
                m_OnRight.Invoke();
            }
            else
            {
                m_Direction = PlatformDirection.Left;
                m_OnLeft.Invoke(); 
            }
            Debug.Log($"Contact from {(m_Direction == PlatformDirection.Right ? "Right" : "Left")}");
        }
        private bool IsContactFromRight(Vector2 contactPoint)
        {
            return contactPoint.x > transform.position.x;
        }
    }
}
