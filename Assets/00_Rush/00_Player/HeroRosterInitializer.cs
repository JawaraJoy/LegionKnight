using LegionKnight;
using UnityEngine;

namespace Rush
{
    public sealed class HeroRosterInitializer : MonoBehaviour
    {
        [SerializeField] private PlayerHeroesCollection m_HeroesCollection;
        private const string c_UsedHeroKey = "usedcharacter";

        public void Initialize() => InitializeInternal();
        private void InitializeInternal()
        {
            RestoreAvailableHeroInternal();
            m_HeroesCollection.Init();
        }

        private void RestoreAvailableHeroInternal()
        {
            UnityService service = UnityService.Instance;
            if (!service.HasData(c_UsedHeroKey)) return;
            string savedId = service.GetData<string>(c_UsedHeroKey);
            foreach (HeroUnit hero in m_HeroesCollection.HeroUnits)
                if (hero != null && hero.HeroConfig != null && hero.HeroConfig.BaseInfo.Id == savedId)
                    return;
            service.SaveData(c_UsedHeroKey, m_HeroesCollection.DefaultHero.BaseInfo.Id);
        }
    }
}

