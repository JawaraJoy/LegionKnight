using LegionKnight;
using UnityEngine;

namespace Rush
{
    public class PlayerCollectionAgent : MonoBehaviour
    {
        public void CheckIfHeroIsTrial()
        {
            HeroUnitConfig usedHeroConfig = Player.Instance.HeroesCollection.UsedHero;
            HeroUnit usedHeroUnit = Player.Instance.HeroesCollection.GetHeroUnit(usedHeroConfig);
            if (usedHeroUnit.OnTrial)
            {
                usedHeroUnit.SetTrial(false);
                HeroUnitConfig defaultHero = Player.Instance.HeroesCollection.DefaultHero;
                Player.Instance.HeroesCollection.SetSelectedHero(defaultHero);
                Player.Instance.HeroesCollection.SetUsedHero();
            }
        }
    }
}
