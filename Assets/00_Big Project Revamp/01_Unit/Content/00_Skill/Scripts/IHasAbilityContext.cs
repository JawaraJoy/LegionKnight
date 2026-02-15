using UnityEngine;

namespace Rush
{
    public interface IHasAbilityContext
    {
        AbilityContext AbilityContext { get; }
        void Init(AbilityContext abilityContext);
    }
}
