using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Profiling;

namespace Rush
{
    internal sealed class UnitPool
    {
        private const int c_PrewarmPerFrame = 5;
        private readonly PoolDefinition m_Definition;
        private readonly Stack<GameObject> m_Inactive;
        private readonly HashSet<GameObject> m_Owned = new();
        private readonly HashSet<GameObject> m_Active = new();
        private readonly Transform m_Root;

        public UnitPool(PoolDefinition definition, Transform root)
        {
            m_Definition = definition;
            m_Root = root;
            m_Inactive = new Stack<GameObject>(definition.InitialSize);
        }

        public IEnumerator PrewarmAsync()
        {
            int created = 0;
            while (m_Owned.Count < m_Definition.InitialSize)
            {
                Profiler.BeginSample("Pool.Prewarm");
                try { m_Inactive.Push(CreateInstanceInternal()); }
                finally { Profiler.EndSample(); }
                if (++created % c_PrewarmPerFrame == 0) yield return null;
            }
        }

        private GameObject CreateInstanceInternal()
        {
            GameObject instance = Object.Instantiate(m_Definition.Prefab, m_Root);
            instance.SetActive(false);
            m_Owned.Add(instance);
            return instance;
        }

        public bool TrySpawn(Transform target, bool asChild, out GameObject instance)
        {
            instance = null;
            if (target == null) return false;
            Profiler.BeginSample("Pool.Spawn");
            try
            {
                while (m_Inactive.Count > 0 && instance == null) instance = m_Inactive.Pop();
                if (instance == null)
                {
                    if (!m_Definition.Expandable && m_Owned.Count >= m_Definition.InitialSize) return false;
                    instance = CreateInstanceInternal();
                }
                if (asChild)
                {
                    instance.transform.SetParent(target, false);
                    instance.transform.localPosition = Vector3.zero;
                    instance.transform.localRotation = Quaternion.identity;
                }
                else
                {
                    instance.transform.SetParent(null);
                    instance.transform.SetPositionAndRotation(target.position, target.rotation);
                }
                m_Active.Add(instance);
                instance.SetActive(true);
                if (instance.TryGetComponent(out IPoolable poolable)) poolable.OnSpawned();
                return true;
            }
            finally { Profiler.EndSample(); }
        }

        public void Despawn(GameObject instance)
        {
            DespawnInternal(instance);
        }

        private void DespawnInternal(GameObject instance)
        {
            if (instance == null || !m_Owned.Contains(instance) || !m_Active.Remove(instance)) return;
            Profiler.BeginSample("Pool.Despawn");
            try
            {
                if (instance.TryGetComponent(out IPoolable poolable)) poolable.OnDespawned();
            }
            finally
            {
                if (instance != null)
                {
                    instance.SetActive(false);
                    instance.transform.SetParent(m_Root);
                    m_Inactive.Push(instance);
                }
                Profiler.EndSample();
            }
        }
    }
}
