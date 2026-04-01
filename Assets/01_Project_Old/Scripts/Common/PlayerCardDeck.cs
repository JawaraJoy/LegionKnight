
using UnityEngine;

namespace LegionKnight
{
    public partial class PlayerCardDeck : CardDeck
    {
        
    }
    public partial class Player
    {
        [SerializeField]
        private PlayerCardDeck m_PlayerCardDeck;
        public PlayerCardDeck PlayerCardDeck => m_PlayerCardDeck;

    }
}
