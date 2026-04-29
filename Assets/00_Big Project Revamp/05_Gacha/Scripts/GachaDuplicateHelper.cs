namespace Rush
{
    // Centralized helper so both preview and result panel
    // use the same duplicate check logic
    public static class GachaDuplicateHelper
    {
        // Returns true if the collectible is a HeroUnitConfig that player already owns
        public static bool IsDuplicateHero(CollectibleConfig collectible,
            out HeroUnitConfig heroConfig)
        {
            heroConfig = collectible as HeroUnitConfig;
            if (heroConfig == null) return false;

            var heroUnit = LegionKnight.Player.Instance
                .HeroesCollection.GetHeroUnit(heroConfig);

            return heroUnit != null && heroUnit.Owned;
        }
    }
}