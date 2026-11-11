using Rush;
using UnityEngine;

namespace LegionKnight
{
    public partial class PlayerHeroesCollection : HeroesCollection
    {
        
    }
    public partial class Player
    {
        [SerializeField]
        private PlayerHeroesCollection m_HeroesCollection;
        public PlayerHeroesCollection HeroesCollection => m_HeroesCollection;

    }
    public partial class PlayerAgent
    {
        public void InitPlayerHeroesCollection()
        {
            Player.Instance.HeroesCollection.Init();
        }

        public void SetOwned(HeroUnitConfig config, bool set)
        {
            Player.Instance.HeroesCollection.SetOwned(config, set);
        }
        public void SetSelectedHero(HeroUnitConfig config)
        {
            Player.Instance.HeroesCollection.SetSelectedHero(config);
        }
        public void SetUsedHero()
        {
            Player.Instance.HeroesCollection.SetUsedHero();
        }
    }
}
