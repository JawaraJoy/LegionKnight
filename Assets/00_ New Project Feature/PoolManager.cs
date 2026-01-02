using System.Collections.Generic;
using UnityEngine;
using LegionKnight;

namespace Rush
{
    public class PoolManager : Singleton<PoolManager>
    {
        [SerializeField] private List<PoolDefinition> m_Pools;
        private readonly Dictionary<string, UnitPool> m_RuntimePools = new();

        protected override void Awake()
        {
            base.Awake();
            InitializePools();
        }
        private void InitializePools()
        {
            foreach (PoolDefinition def in m_Pools)
            {
                if (m_RuntimePools.ContainsKey(def.Id))
                {
                    Debug.LogError($"Duplicate pool ID: {def.Id}");
                    continue;
                }


                Transform root = new GameObject($"[Pool] {def.Id}").transform;
                root.SetParent(transform);


                UnitPool pool = new UnitPool(def, root);
                m_RuntimePools.Add(def.Id, pool);


                StartCoroutine(pool.PrewarmAsync());
            }
        }

        public bool Spawn(string poolId, Transform target, bool asChild, out GameObject instance)
        {
            if (!m_RuntimePools.TryGetValue(poolId, out UnitPool pool))
            {
                instance = null;
                return false;
            }
            return pool.TrySpawn(target, asChild, out instance);
        }


        public void Despawn(string poolId, GameObject instance)
        {
            if (m_RuntimePools.TryGetValue(poolId, out UnitPool pool))
            {
                pool.Despawn(instance);
            }
        }
    }
}
