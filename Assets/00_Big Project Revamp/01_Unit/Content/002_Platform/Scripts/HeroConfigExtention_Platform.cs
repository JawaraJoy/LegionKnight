using UnityEngine;

namespace Rush
{ 
    public class HeroConfigExtention_Platform { }
    public partial class HeroConfig : IHasPlatform
    {
        [SerializeField]
        private PlatformConfig[] m_UniquePlatforms;
        public PlatformConfig[] UniquePlatforms => m_UniquePlatforms;
    }

}
