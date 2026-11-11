using Rush;
using UnityEngine;

namespace LegionKnight
{
    public class HeroViewAgent : MonoBehaviour
    {
        public void OnCharacterLevelUpCharacterView(HeroUnitConfig heroConfig)
        {
            HeroPanel cp = CanvasManager.Instance.GetPanel<HeroPanel>();
            //cp.SetHeroSelected(heroConfig);
        }
    }
}
