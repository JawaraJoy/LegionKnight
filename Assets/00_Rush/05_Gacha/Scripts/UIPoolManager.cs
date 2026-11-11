using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Rush
{
    // Generic MonoBehaviour pool
    // T harus MonoBehaviour agar bisa di-instantiate dan di-parent
    public class UIPoolManager<T> : MonoBehaviour where T : MonoBehaviour
    {
        [SerializeField] private T m_Prefab;
        [SerializeField] private Transform m_Container;
        [SerializeField] private int m_PrewarmCount = 4;

        private readonly Queue<T> m_Pool = new();
        private readonly List<T> m_Active = new();

        [SerializeField]
        private UnityEvent m_OnResultDone;

        public IReadOnlyList<T> Active => m_Active;

        protected virtual void Awake() => PrewarmInternal();

        private void PrewarmInternal()
        {
            for (int i = 0; i < m_PrewarmCount; i++)
                m_Pool.Enqueue(CreateInstanceInternal());
        }

        private T CreateInstanceInternal()
        {
            var instance = Instantiate(m_Prefab, m_Container);
            instance.gameObject.SetActive(false);
            return instance;
        }

        public T Rent()
        {
            var instance = m_Pool.Count > 0 ? m_Pool.Dequeue() : CreateInstanceInternal();
            instance.gameObject.SetActive(true);
            m_Active.Add(instance);
            return instance;
        }

        public void Return(T instance)
        {
            if (instance == null) return;
            instance.gameObject.SetActive(false);
            m_Active.Remove(instance);
            m_Pool.Enqueue(instance);
        }

        public void ReturnAll()
        {
            // iterate copy karena Return memodifikasi m_Active
            for (int i = m_Active.Count - 1; i >= 0; i--)
                Return(m_Active[i]);
        }
    }
}