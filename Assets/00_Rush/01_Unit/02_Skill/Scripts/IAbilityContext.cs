using UnityEngine;

namespace Rush
{
    public interface IAbilityContext : IHasRarityConfig
    {
        ISkillContext SkillContext { get; }
        IAbilityDeliver AbilityDeliver { get; }
        bool Initialized { get; }
    }
}
