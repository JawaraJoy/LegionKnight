using UnityEngine;

namespace Rush
{
    [System.Serializable]
    public class PlatformContext
    {
        [SerializeField]
        private Platform2D m_Platform;
        [SerializeField]
        private Unit m_OwnerUnit;
        public Platform2D Platform => m_Platform;
        public Unit OwnerUnitId => m_OwnerUnit;
        public PlatformContext(Platform2D platform, Unit ownerUnit)
        {
            m_Platform = platform;
            m_OwnerUnit = ownerUnit;
        }
    }
}
