using UnityEngine;
using UnityEngine.Events;

namespace Rush
{
    public interface IHasAttacker : IHasAbilityContext
    {
        AttackerField AttackerField { get; }
        UnityEvent<IDamageable> OnAttackDelivered {  get; }
    }
    
}
