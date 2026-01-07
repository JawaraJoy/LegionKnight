using UnityEngine;

namespace Rush
{
    public enum TargetPriority
    {
        Nearest = 0,
        Farthest = 1,
        Random = 2,
        LowestHealth = 3,
        HighestHealth = 4,
        LowestHealthRate = 5,
        HighestHealthRate = 6,
        AllAllies = 7,
        AllEnemies = 8,
        All = 9,
        Self = 10,
    }
    public enum ScaningMethod
    {
        RescanEveryTime = 0,
        ScanOnce = 1,
    }
    public enum AbilityPurpose
    {
        Damaging = 0,
        Healing = 1,
        StatModifier = 2,
    }
}
