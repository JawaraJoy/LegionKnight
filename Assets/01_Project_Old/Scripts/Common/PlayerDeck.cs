using Rush;
using UnityEngine;

namespace LegionKnight
{
    public partial class PlayerDeck : CharacterDeck
    {
        
    }
    public partial class Player
    {
        [SerializeField]
        private PlayerDeck m_HeroDeck;
        public PlayerDeck HeroDeck => m_HeroDeck;

    }
    public partial class PlayerAgent
    {
        public void InitPlayerDeck()
        {
            Player.Instance.HeroDeck.Init();
        }

        public void SetOwned(HeroUnitConfig config, bool set)
        {
            Player.Instance.HeroDeck.SetOwned(config, set);
        }
        public void SetSelectedCharacter(HeroUnitConfig config)
        {
            Player.Instance.HeroDeck.SetSelectedCharacter(config);
        }
        public void SetUsedCharacter()
        {
            Player.Instance.HeroDeck.SetUsedCharacter();
        }
    }
}
