using UnityEngine;

namespace Rush
{
    public interface IHealer : IHasAbilityContext
    {
        int HealAmount { get; }
    }
}
