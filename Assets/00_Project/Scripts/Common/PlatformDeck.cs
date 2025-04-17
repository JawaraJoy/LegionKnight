using System.Collections.Generic;
using UnityEngine;

namespace LegionKnight
{
    [System.Serializable]
    public partial class PlatformOwned
    {
        [SerializeField]
        private bool m_IsOwned;
        [SerializeField]
        private StanbyPlatform m_StanbyPlatform;
        public bool IsOwned => m_IsOwned;
        public StanbyPlatform StanbyPlatform => m_StanbyPlatform;
        public PlatformOwned(StanbyPlatform stanbyPlatform)
        {
            m_StanbyPlatform = stanbyPlatform;
            m_IsOwned = false;
        }
        public void SetOwned(bool isOwned)
        {
            m_IsOwned = isOwned;
        }
    }
    public partial class PlatformDeck : MonoBehaviour
    {
        [SerializeField]
        private StanbyPlatform m_UsedStanbyPlatform;
        [SerializeField]
        private List<PlatformOwned> m_Deck = new();

        private PlatformOwned GetPlatformOwnedInternal(StanbyPlatform platform)
        {
            foreach (var platformOwned in m_Deck)
            {
                if (platformOwned.StanbyPlatform == platform)
                {
                    return platformOwned;
                }
            }
            return null;
        }
        public PlatformOwned GetPlatformOwned(StanbyPlatform platform)
        {
            return GetPlatformOwnedInternal(platform);
        }
        public StanbyPlatform GetUsedStanbyPlatform()
        {
            return m_UsedStanbyPlatform;
        }
        public void SetIsOwned(StanbyPlatform platform, bool isOwned)
        {
            var platformOwned = GetPlatformOwnedInternal(platform);
            platformOwned?.SetOwned(isOwned);
        }
        public void SetUsedStandbyPlatform(StanbyPlatform platform)
        {
            m_UsedStanbyPlatform = platform;
        }
    }
}
