using UnityEngine;

namespace Rush
{
    public interface IHasPlatform
    {
        PlatformConfig[] UniquePlatforms { get; }
    }
}
