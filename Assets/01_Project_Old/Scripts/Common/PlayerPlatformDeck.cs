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
        public PlayerPlatformDeck PlatformDeck => m_PlayerPlatformDeck;

    }
}
