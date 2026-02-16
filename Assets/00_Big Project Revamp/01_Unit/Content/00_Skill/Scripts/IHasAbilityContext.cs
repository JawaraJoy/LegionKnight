using UnityEngine;

namespace Rush
{
    public interface IHasAbilityContext
    {
        AbilityContext AbilityContext { get; }
        bool Initialized { get; }
        void Init(AbilityContext abilityContext);
    }
}
