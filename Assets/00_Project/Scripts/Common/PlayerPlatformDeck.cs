using System.Collections.Generic;
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
        public PlatformUnit GetPlatformOwned(StandbyPlatformDefinition platform)
        {
            return m_PlayerPlatformDeck.GetPlatformOwned(platform);
        }
        public bool IsPlatformOwned(StandbyPlatformDefinition platform)
        {
            return m_PlayerPlatformDeck.IsPlatformOwned(platform);
        }
        public StandbyPlatformDefinition GetUsedStanbyPlatform()
        {
            return m_PlayerPlatformDeck.GetUsedStanbyPlatform();
        }
        public void AddPlatformAmount(StandbyPlatformDefinition platform, int add)
        {
            m_PlayerPlatformDeck.AddPlatformAmount(platform, add);
        }
        public void SelectStandbyPlatform(StandbyPlatformDefinition platform)
        {
            m_PlayerPlatformDeck.SelectStandbyPlatform(platform);
        }
        public void SetUsedStanbyPlatform()
        {
            m_PlayerPlatformDeck.SetUsedStandbyPlatform();
        }
        public void AddPlayerStandbyPlatform()
        {
           m_PlayerPlatformDeck.AddPlayerStandbyPlatform();
        }
    }
}
