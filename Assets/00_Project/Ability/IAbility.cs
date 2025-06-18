using UnityEngine;

namespace LegionKnight
{
    public interface IAbility
    {
        void Initialize(AbilityDefinition defi, int level); // Method to initialize the ability
    }
}
