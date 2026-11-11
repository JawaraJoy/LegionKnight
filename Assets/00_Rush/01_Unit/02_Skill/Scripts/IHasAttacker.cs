using UnityEngine;
using UnityEngine.Events;

namespace Rush
{
    public interface IHasAttacker : IHasAbilityContext
    {
        AttackerField AttackerField { get; }
        UnityEvent<IDamageable> OnAttackDelivered {  get; }
        
        void OnAttackDeliveredInvoke(IDamageable damageable);
        void OnAttackDeliveredTargetInvoke(ITargetable target);
UnityEvent<ITargetable> OnAttackDeliveredTarget { get; }
    }
    
}
