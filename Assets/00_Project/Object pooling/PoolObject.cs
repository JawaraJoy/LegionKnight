using UnityEngine;
using UnityEngine.Events;

namespace LegionKnight
{
    public class PoolObject : MonoBehaviour, IPoolable
    {
        [SerializeField]
        private PoolDefinition m_Definition;
        public PoolDefinition Definition => m_Definition;
        [SerializeField]
        private UnityEvent m_OnReSpawn;
        public void ReActiveOnSpawn()
        {
            m_OnReSpawn.Invoke();
        }
    }
}
