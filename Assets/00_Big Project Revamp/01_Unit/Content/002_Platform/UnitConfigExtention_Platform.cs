using UnityEngine;

namespace Rush
{ 
    public class UnitConfigExtention_Platform { }
    public partial class UnitConfig
    {
        [SerializeField]
        private PlatformConfig[] m_UniquePlatform;
        public PlatformConfig[] UniquePlatform => m_UniquePlatform;
    }

}
