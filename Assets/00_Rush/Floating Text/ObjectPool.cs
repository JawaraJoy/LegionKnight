using System.Collections.Generic;
using UnityEngine;

namespace Rush
{
    public class ObjectPool<T> where T : MonoBehaviour
    {
        private readonly T m_Prefab;
        private readonly Transform m_Parent;

        private readonly Queue<T> m_Pool = new Queue<T>();

        public ObjectPool(T prefab, int prewarm, Transform parent = null)
        {
            m_Prefab = prefab;
            m_Parent = parent;

            for (int i = 0; i < prewarm; i++)
            {
                Create();
            }
        }

        private T Create()
        {
            T obj = GameObject.Instantiate(m_Prefab, m_Parent);
            obj.gameObject.SetActive(false);

            m_Pool.Enqueue(obj);

            return obj;
        }

        public T Get()
        {
            if (m_Pool.Count == 0)
                Create();

            T obj = m_Pool.Dequeue();

            return obj;
        }

        public void Release(T obj)
        {
            obj.gameObject.SetActive(false);
            m_Pool.Enqueue(obj);
        }
    }
}