using UnityEngine;

namespace Rush
{
    [System.Serializable]
    public class PlatformContext
    {
        [SerializeField]
        private Platform2D m_Platform;
        [SerializeField]
        private GameObject m_OwnerObject;
        public Platform2D Platform => m_Platform;
        public GameObject OwnerObject => m_OwnerObject;
        public PlatformContext(Platform2D platform, GameObject ownerUnit)
        {
            m_Platform = platform;
            m_OwnerObject = ownerUnit;
        }
    }
}
