using UnityEngine;
namespace Rush
{
    public partial class PlayerUnit : Unit
    {
        // Compatibility boundary for the legacy profile; combat consumers use BaseStats.
        protected override void InitializeProgressionInternal()
        {
            base.InitializeProgressionInternal();
            if (!(m_Config is HeroUnitConfig heroConfig))
                return;
            var player = LegionKnight.Player.Instance;
            var collection = player == null ? null : player.HeroesCollection;
            if (collection == null)
                return;
            var profile = collection.GetHeroUnit(heroConfig);
            if (profile == null)
                return;
            var bonus = heroConfig.EvolutionConfig == null
                ? StatField.Zero
                : heroConfig.EvolutionConfig.GetStatBonus(profile.Star);
            SetProgressionInternal(profile.Level, bonus);
        }
    }
    public partial class RushPlayer
    {
        [SerializeField] private PlayerUnit m_Unit;
        public PlayerUnit Unit => m_Unit;
    }
}
