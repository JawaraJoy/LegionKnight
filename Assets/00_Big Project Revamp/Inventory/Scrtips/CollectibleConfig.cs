
using UnityEngine;

namespace Rush
{
    public abstract partial class CollectibleConfig : Configuration
    {
        [SerializeField]
        private CollectibleField m_CollectibleField;
        public CollectibleField CollectibleField => m_CollectibleField;
    }
}
