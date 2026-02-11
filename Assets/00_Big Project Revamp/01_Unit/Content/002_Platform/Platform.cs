using MoreMountains.Tools;
using UnityEngine;

namespace Rush
{
    public class Platform : MonoBehaviour
    {
        [SerializeField, MMReadOnly]
        private PlatformConfig m_Config;
        
        public void Init(PlatformConfig config)
        {
            m_Config = config;
        }
    }
}
