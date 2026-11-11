using UnityEngine;

namespace Rush
{ 
    public partial class HeroUnitConfig : IHasPlatform
    {
        [SerializeField]
        private PlatformConfig[] m_UniquePlatforms;
        public PlatformConfig[] UniquePlatforms => m_UniquePlatforms;
    }

}
