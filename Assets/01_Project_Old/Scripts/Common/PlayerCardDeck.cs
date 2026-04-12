
using UnityEngine;

namespace LegionKnight
{
    public partial class PlayerCardDeck : CardDeck
    {
        
    }
    // this is singleton
    // you can call Player.Instance.PlayerCardDeck
    public partial class Player
    {
        [SerializeField]
        private PlayerCardDeck m_PlayerCardDeck;
        public PlayerCardDeck PlayerCardDeck => m_PlayerCardDeck;

    }
}
