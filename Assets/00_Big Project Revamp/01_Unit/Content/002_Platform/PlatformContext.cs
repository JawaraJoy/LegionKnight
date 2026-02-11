using UnityEngine;

namespace Rush
{
    [System.Serializable]
    public class PlatformContext
    {
        [SerializeField]
        private Platform m_Platform;
        [SerializeField]
        private Unit m_OwnerUnit;
        public Platform Platform => m_Platform;
        public Unit OwnerUnitId => m_OwnerUnit;
    }
}
