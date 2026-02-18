using UnityEngine;

namespace Rush
{
    public interface IAbilityContext
    {
        ISkillContext SkillContext { get; }
        IAbilityDeliver AbilityDeliver { get; }
        bool Initialized { get; }
    }
}
