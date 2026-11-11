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
                OnSendDirectionRightInvoke();
            }
            else
            {
                OnSendDirectionLeftInvoke();
            }
        }
        private void OnTriggerEnter2D(Collider2D collision)
        {
            Vector2 targetPost = collision.transform.position;
            if (IsContactFromRight(targetPost))
            {
                m_Direction = PlatformDirection.Right;
                OnRightInvoke();
            }
            else
            {
                m_Direction = PlatformDirection.Left;
                OnLeftInvoke(); 
            }
            Debug.Log($"Contact from {(m_Direction == PlatformDirection.Right ? "Right" : "Left")}");
        }
        private bool IsContactFromRight(Vector2 contactPoint)
        {
            return contactPoint.x > transform.position.x;
        }
    

private void OnSendDirectionRightInvoke()
        {
            m_OnSendDirectionRight?.Invoke();
        }

        private void OnSendDirectionLeftInvoke()
        {
            m_OnSendDirectionLeft?.Invoke();
        }

        private void OnRightInvoke()
        {
            m_OnRight?.Invoke();
        }

        private void OnLeftInvoke()
        {
            m_OnLeft?.Invoke();
        }
}
}
