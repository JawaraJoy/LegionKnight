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

                CanvasManager.Instance.GetPanel<TextPopUpPanel>().ShowText("Trial Hero End");
            }
        }

        public void LevelUpHero()
        {
            HeroUnitConfig selected = Player.Instance.HeroesCollection.SelectedHero;
            HeroUnit selectHero = Player.Instance.HeroesCollection.GetHeroUnit(selected);
            selectHero.AddLevel(1);

            CanvasManager.Instance.GetPanel<TextPopUpPanel>().ShowText("Level Up!");
        }
    }
}
