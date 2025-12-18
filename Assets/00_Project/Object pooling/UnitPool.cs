using System.Collections.Generic;
using System.Linq;
using UnityEditor.Rendering;
using UnityEngine;

namespace LegionKnight
{
    public class UnitPool
    {
        private readonly PoolDefinition m_Definition;
        private readonly List<GameObject> m_Objects = new();
        public PoolDefinition Definition => m_Definition;
        public List<GameObject> Objects => m_Objects;
        public UnitPool(PoolDefinition defi)
        {
            m_Definition = defi;
        }

        private bool MaxCapacityReachedInternal()
        {
            return m_Objects.Count >= m_Definition.CopyCatAmount;
        }
        public bool MaxCapacityReached()
        {
            return MaxCapacityReachedInternal();
        }
        public bool CapacityZero()
        {
            return m_Objects.Count <= 0;
        }
        public void AddObject(GameObject obj)
        {
            if (!MaxCapacityReachedInternal())
            {
                m_Objects.Add(obj);
            }
        }
        public void RemoveObject(GameObject obj)
        {
            if (m_Objects.Contains(obj))
            {
                m_Objects.Remove(obj);
            }
        }
        private GameObject GetRandomInactiveObj()
        {
            // select gameobject that is inactive in hierarchy
            List<GameObject> inactiveObjects = m_Objects.Where(o => !o.activeInHierarchy).ToList();
            GameObject obj = inactiveObjects[Random.Range(0, inactiveObjects.Count)];
            return obj;
        }
        public void ReSpawn(Transform reSpawnSpotParent, bool detachFromParent, out GameObject selected)
        {
            GameObject obj = GetRandomInactiveObj();
            selected = obj;
            obj.transform.SetParent(reSpawnSpotParent, false);
            if (detachFromParent)
            {
                // positioning to respawn spot parent but detach from it
                reSpawnSpotParent.DetachChildren();
            }
            obj.SetActive(true);
            if (obj.TryGetComponent(out IPoolable poolable))
            {
                poolable.ReActiveOnSpawn();
            }
            Debug.Log($"Unitpool, try to respawn {m_Objects.Count}");
        }
    }

    public interface IPoolable
    {
        void ReActiveOnSpawn();
    }
}
