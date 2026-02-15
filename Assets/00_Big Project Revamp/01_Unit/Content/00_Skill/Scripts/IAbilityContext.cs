using UnityEngine;

namespace Rush
{
    public interface IAbilityContext
    {
        SkillContext SkillContext { get; }
        AbilityDeliver AbilityDeliver { get; }
    }
}
