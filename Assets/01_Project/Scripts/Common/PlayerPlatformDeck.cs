using System.Collections.Generic;
using UnityEngine;
using Rush;

namespace LegionKnight
{
    public partial class PlayerPlatformDeck : PlatformDeck
    {
        
    }
    public partial class Player
    {
        [SerializeField]
        private PlayerPlatformDeck m_PlayerPlatformDeck;
        public PlayerPlatformDeck GetPlayerPlatformDeck()
        {
            return m_PlayerPlatformDeck;
        }
        public PlatformUnit GetPlatformOwned(PlatformConfig platform)
        {
            return m_PlayerPlatformDeck.GetPlatformOwned(platform);
        }
        public PlatformUnit[] GetPlatformUnits()
        {
            return m_PlayerPlatformDeck.GetPlatformUnits();
        }
        public bool IsPlatformOwned(PlatformConfig platform)
        {
            return m_PlayerPlatformDeck.IsPlatformOwned(platform);
        }
        public PlatformConfig GetUsedStanbyPlatform()
        {
            return m_PlayerPlatformDeck.GetUsedStanbyPlatform();
        }
        public void AddPlatformAmount(PlatformConfig platform, int add)
        {
            m_PlayerPlatformDeck.AddPlatformAmount(platform, add);
        }
        public void SelectStandbyPlatform(PlatformConfig platform)
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
        public void SetPlatformUnitIsEquiped(PlatformConfig defi, bool isEquiped)
        {
           m_PlayerPlatformDeck.SetIsEquiped(defi, isEquiped);
        }
    }
}
