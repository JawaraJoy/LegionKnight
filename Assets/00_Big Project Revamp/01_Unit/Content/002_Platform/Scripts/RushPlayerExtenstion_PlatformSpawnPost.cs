using UnityEngine;

namespace Rush
{
    public class RushPlayerExtenstion_PlatformSpawnPost
    {
        
    }

    public partial class RushPlayer
    {
        [SerializeField]
        private Transform m_PlatformSpawnPost;
        public Transform PlatformSpawnPost => m_PlatformSpawnPost;
    }
}
