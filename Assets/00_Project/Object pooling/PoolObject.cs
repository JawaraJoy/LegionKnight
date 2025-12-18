using UnityEngine;
using UnityEngine.Events;

namespace LegionKnight
{
    public interface IPoolable
    {
        void OnSpawned();
        void OnDespawned();
    }


    // =============================
    // POOL OBJECT (OPTIONAL BASE)
    // =============================
    public class PoolObject : MonoBehaviour, IPoolable
    {
        [SerializeField]
        private PoolDefinition m_Definition;
        public PoolDefinition Definition => m_Definition;
        public virtual void OnSpawned() { }
        public virtual void OnDespawned() 
        {
            
        }
    }

}
