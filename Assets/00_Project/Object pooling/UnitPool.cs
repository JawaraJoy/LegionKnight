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
        public void ReSpawn(Transform reSpawnSpotParent, bool detachFromParent)
        {
            if (detachFromParent)
            {
                // positioning to respawn spot parent but detach from it
                GetRandomInactiveObj().transform.SetParent(reSpawnSpotParent, false);
                reSpawnSpotParent.DetachChildren();
            }
            else
            {
                // positioning to respawn spot parent and keep it as child
                GetRandomInactiveObj().transform.SetParent(reSpawnSpotParent, false);
            }
            GetRandomInactiveObj().SetActive(true);
            if (GetRandomInactiveObj().TryGetComponent(out IUnitPoolable poolable))
            {
                poolable.ReActiveOnSpawn();
            }
        }
    }

    public interface IUnitPoolable
    {
        void ReActiveOnSpawn();
    }
}
