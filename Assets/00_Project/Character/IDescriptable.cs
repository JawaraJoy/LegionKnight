using UnityEngine;

namespace LegionKnight
{
    public interface IDescriptable
    {
        string Id { get; }
        string Label { get; }
        string Description { get; }
    }
    public interface IDescriptAmountable : IDescriptable
    {
        int Amount { get; }
    }

    public interface IObjectHasOwner
    {
        Object Owner { get; }
        void SetOwner(Object owner);
    }

    public interface IOwner
    {
        void InitAsOwner();
    }

    public interface IAbilityOwner
    {
        int GetOwnerLevel();
    }

    public static class AbilityUtil
    {
        public static int GetOwnerLevel(Object rawOwner)
        {
            if (rawOwner is IAbilityOwner abilityOwner)
            {
                return abilityOwner.GetOwnerLevel();
            }
            return 1;
        }
    }

}
