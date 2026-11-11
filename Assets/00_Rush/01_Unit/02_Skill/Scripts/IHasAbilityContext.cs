using UnityEngine;

namespace Rush
{
    public interface IHasAbilityContext
    {
        IAbilityContext AbilityContext { get; }
        bool Initialized { get; }
        void Init(IAbilityContext abilityContext);
    }
}
