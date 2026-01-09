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
    }
    public enum TargetObject
    {
        Enemy = 0,
        Ally = 1,
        Self = 2,
        Player = 3,
        All = 4,
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
