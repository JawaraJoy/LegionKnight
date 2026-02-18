using UnityEngine;

namespace Rush
{
    public interface IAbilityContext
    {
        ISkillContext SkillContext { get; }
        AbilityDeliver AbilityDeliver { get; }
        bool Initialized { get; }
    }
}
