using UnityEngine;

namespace LegionKnight
{
    public class PoolObject : MonoBehaviour
    {
        [SerializeField]
        private PoolDefinition m_Definition;
        public PoolDefinition Definition => m_Definition;
    }
}
