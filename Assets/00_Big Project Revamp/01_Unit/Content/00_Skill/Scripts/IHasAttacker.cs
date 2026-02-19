using UnityEngine;

namespace Rush
{
    public interface IHasAttacker : IHasAbilityContext
    {
        AttackerField AttackerField { get; }
    }
    
}
