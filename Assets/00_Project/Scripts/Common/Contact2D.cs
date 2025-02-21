using Rush;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace LegionKnight
{
    public partial class Contact2D : MonoBehaviour, IContactable
    {
        [SerializeField]
        protected bool m_CanContact = true;
        [SerializeField]
        private LayerMask m_ContactLayer;

        [SerializeField]
        private UnityEvent<GameObject> m_OnContacted = new();
        [SerializeField]
        private UnityEvent<GameObject> m_OnDeContacted = new();

        private List<GameObject> m_ContactableList = new();
        protected void SetCanContactInternal(bool set)
        {
            m_CanContact = set;
        }
        private bool HasContact(GameObject other)
        {
            return m_ContactableList.Contains(other);
        }

        private void OnTriggerEnter(Collider other)
        {
            ContactBehaviour(other.gameObject);
        }
        private void OnTriggerEnter2D(Collider2D collision)
        {
            ContactBehaviour(collision.gameObject);
        }
        private void OnTriggerExit2D(Collider2D collision)
        {
            ContactBehaviour(collision.gameObject);
        }
        private void OnTriggerExit(Collider other)
        {
            DeContactBehaviour(other.gameObject);
        }

        protected void ContactBehaviour(GameObject other)
        {
            if (!HasContact(other))
            {
                if (!CanContactInternal(other, out IContactable contactable)) return;
                OnContactedBehaviourInvoke(contactable);
            }
        }

        protected void DeContactBehaviour(GameObject other)
        {
            if (HasContact(other))
            {

                Debug.Log($"Remove Contactable");
                if (!CanContactInternal(other, out IContactable contactable)) return;
                OnDeContactBehaviourInvoke(contactable);
            }
        }
        public bool GetCanContact()
        {
            return m_CanContact;
        }
        protected virtual bool CanContactInternal<T>(GameObject other, out T contactable) where T : IContactable
        {
            bool layerMatch = ((1 << other.layer) & m_ContactLayer) != 0;
            bool isContactable = other.TryGetComponent(out contactable);
            bool can = layerMatch && m_CanContact && contactable.GetCanContact() && isContactable;
            Debug.Log($"Can Contact {can}");

            return can;
        }
        public bool IsContacting()
        {
            return m_ContactableList.Count > 0;
        }
        protected virtual void OnContactedBehaviourInvoke(IContactable other)
        {
            m_ContactableList.Add(other.GetSelf());
            m_OnContacted?.Invoke(other.GetSelf());

        }
        protected virtual void OnDeContactBehaviourInvoke(IContactable other)
        {
            m_ContactableList.Remove(other.GetSelf());
            m_OnDeContacted?.Invoke(other.GetSelf());
        }
        public void SetCanContact(bool can)
        {
            m_CanContact = can;
        }

        public GameObject GetSelf()
        {
            return GetSelfInternal();
        }

        protected GameObject GetSelfInternal()
        {
            return gameObject;
        }
    }
}
