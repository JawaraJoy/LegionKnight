using UnityEngine;

namespace Rush
{
    public class PlatformManager : PlatformHandler
    {
        
    }
    public partial class RushGameManager
    {
        [SerializeField]
        private PlatformManager m_PlatformManager;
        public PlatformManager PlatformManager => m_PlatformManager;
    }
}
