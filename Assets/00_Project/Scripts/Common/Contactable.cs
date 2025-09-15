using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Rush
{
    public interface IContactable
    {
        bool GetCanContact();
        bool IsContacting();
        GameObject GetSelf();
        void SetCanContact(bool can);
        void RemoveSelf();
        List<GameObject> GetContactableList();
    }

    public abstract class Contactable : MonoBehaviour, IContactable
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
                if (!CanContactInternal(other)) return;
                OnContactedBehaviourInvoke(other);
            }
        }

        protected void DeContactBehaviour(GameObject other)
        {
            if (HasContact(other))
            {

                Debug.Log($"Remove Contactable");
                if (!CanContactInternal(other)) return;
                OnDeContactBehaviourInvoke(other);
            }
        }
        public bool GetCanContact()
        {
            return m_CanContact;
        }
        protected virtual bool CanContactInternal(GameObject other)
        {
            bool layerMatch = ((1 << other.layer) & m_ContactLayer) != 0;
            bool isContactable = other.TryGetComponent(out IContactable contactable);
            bool can = layerMatch && m_CanContact && contactable.GetCanContact() && isContactable;
            Debug.Log($"Can Contact {can}");

            return can;
        }
        public bool IsContacting()
        {
            return m_ContactableList.Count > 0;
        }
        protected virtual void OnContactedBehaviourInvoke(GameObject other)
        {
            m_ContactableList.Add(other);
            m_OnContacted?.Invoke(other);

        }
        protected virtual void OnDeContactBehaviourInvoke(GameObject other)
        {
            m_ContactableList.Remove(other);
            m_OnDeContacted?.Invoke(other);
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
        private void OnDestroy()
        {
            RemoveSelf();
        }

        public void RemoveSelf()
        {
            foreach (var item in m_ContactableList)
            {
                if (item.TryGetComponent(out IContactable contactable))
                {
                    contactable.GetContactableList().Remove(gameObject);
                }
            }
        }

        public List<GameObject> GetContactableList()
        {
            return m_ContactableList;
        }
    }
}

