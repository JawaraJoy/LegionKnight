using System.Collections.Generic;
using UnityEngine;

namespace LegionKnight
{
    public static class ContainerPooling
    {
        private readonly static List<UnitPool> m_UnitPool = new ();
        public static void AddUnitPool(PoolObject pool)
        {
            UnitPool existingPool = GetUnitPoolInternal(pool.Definition.Id);
            if (existingPool != null)
            {
                existingPool.AddObject(pool.gameObject);
            }
            else
            { 
                UnitPool newPool = new(pool.Definition);
                newPool.AddObject(pool.gameObject);
                m_UnitPool.Add(newPool);
            }
            Debug.Log($"Container, add Unit {m_UnitPool.Count}, {existingPool.Objects.Count}");
        }
        public static void RemoveUnitPool(PoolObject pool)
        {
            UnitPool existingPool = GetUnitPoolInternal(pool.Definition.Id);
            existingPool?.RemoveObject(pool.gameObject);
            if (existingPool.CapacityZero())
            {
                m_UnitPool.Remove(existingPool);
            }
        }

        public static bool HasUnitPool(string id)
        {
            bool hasUnit = GetUnitPoolInternal(id) != null;
            bool atMax = hasUnit && GetUnitPoolInternal(id).MaxCapacityReached();
            return atMax;
        }
        private static UnitPool GetUnitPoolInternal(string id)
        {
            return m_UnitPool.Find(pool => pool.Definition.Id == id);
        }

        public static UnitPool GetUnitPool(string id)
        {
            return GetUnitPoolInternal(id);
        }
    }

}
