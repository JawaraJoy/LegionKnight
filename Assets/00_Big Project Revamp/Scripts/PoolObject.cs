using UnityEngine;
using UnityEngine.Events;

namespace Rush
{
    public interface IPoolable
    {
        void OnSpawned();
        void OnDespawned();
        bool IsActive { get; }
    }


    // =============================
    // POOL OBJECT (OPTIONAL BASE)
    // =============================
    public class PoolObject : MonoBehaviour, IPoolable
    {
        [SerializeField]
        private PoolDefinition m_Definition;
        public PoolDefinition Definition => m_Definition;

        public bool IsActive => gameObject.activeInHierarchy;

        public virtual void OnSpawned() { }
        public virtual void OnDespawned() 
        {
            
        }
    }

}
