using UnityEngine;

namespace LegionKnight
{
    public enum ScaningType
    {
        Nearest = 0,
        Farthest = 1,
        Random = 2,
        LowestHealth = 3,
        HighestHealth = 4,
        LowestHealthRate = 5,
        HighestHealthRate = 6,
    }
    public enum ScaningMethod
    {
        RescanEveryTime = 0,
        ScanOnce = 1,
    }
}
