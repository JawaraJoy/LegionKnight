// ==================================================
// File: UnitPool.cs
// ==================================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Profiling;


namespace Rush
{
    internal sealed class UnitPool
    {
        private readonly PoolDefinition m_Definition;
        private readonly Stack<GameObject> m_Inactive;
        private readonly Transform m_Root;


        public UnitPool(PoolDefinition definition, Transform root)
        {
            m_Definition = definition;
            m_Root = root;
            m_Inactive = new Stack<GameObject>(definition.InitialSize);
        }


        public IEnumerator PrewarmAsync()
        {
            Profiler.BeginSample("Pool.Prewarm");
            for (int i = 0; i < m_Definition.InitialSize; i++)
            {
                CreateInstance();
                if (i % 5 == 0)
                    yield return null;
            }
            Profiler.EndSample();
        }


        private GameObject CreateInstance()
        {
            GameObject obj = Object.Instantiate(m_Definition.Prefab, m_Root);
            obj.SetActive(false);
            m_Inactive.Push(obj);
            return obj;
        }
        public bool TrySpawn(Transform target, bool asChild, out GameObject instance)
        {
            Profiler.BeginSample("Pool.Spawn");


            if (m_Inactive.Count > 0)
            {
                instance = m_Inactive.Pop();
            }
            else if (m_Definition.Expandable)
            {
                instance = CreateInstance();
            }
            else
            {
                instance = null;
                Profiler.EndSample();
                return false;
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
                instance.transform.position = target.position;
                instance.transform.rotation = target.rotation;
            }


            instance.SetActive(true);


            if (instance.TryGetComponent(out IPoolable poolable))
                poolable.OnSpawned();


            Profiler.EndSample();
            return true;
        }


        public void Despawn(GameObject instance)
        {
            Profiler.BeginSample("Pool.Despawn");


            if (instance.TryGetComponent(out IPoolable poolable))
                poolable.OnDespawned();


            instance.SetActive(false);
            instance.transform.SetParent(m_Root);
            m_Inactive.Push(instance);


            Profiler.EndSample();
        }
    }
}
