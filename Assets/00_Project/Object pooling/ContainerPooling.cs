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
