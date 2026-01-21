using UnityEngine;

namespace LegionKnight
{
    public static class AbilityUtil
    {
        public static int GetOwnerLevel(Object rawOwner)
        {
            if (rawOwner is IAbilityOwner abilityOwner)
            {
                return abilityOwner.GetUnitLevel();
            }
            return 1;
        }
    }
}
