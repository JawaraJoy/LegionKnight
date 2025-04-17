using UnityEngine;

namespace LegionKnight
{
    public partial class PlayerPlatformDeck : PlatformDeck
    {
        
    }
    public partial class Player
    {
        [SerializeField]
        private PlayerPlatformDeck m_PlayerPlatformDeck;
        public PlatformOwned GetPlatformOwned(StanbyPlatform platform)
        {
            return m_PlayerPlatformDeck.GetPlatformOwned(platform);
        }
        public StanbyPlatform GetUsedStanbyPlatform()
        {
            return m_PlayerPlatformDeck.GetUsedStanbyPlatform();
        }
        public void SetIsOwned(StanbyPlatform platform, bool isOwned)
        {
            m_PlayerPlatformDeck.SetIsOwned(platform, isOwned);
        }
        public void SetUsedStandbyPlatform(StanbyPlatform platform)
        {
            m_PlayerPlatformDeck.SetUsedStandbyPlatform(platform);
        }
    }
}
