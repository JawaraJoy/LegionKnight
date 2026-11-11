using Spine.Unity;
using UnityEngine;

namespace Rush
{
    public interface IHasSekeletonAsset
    {
        SkeletonDataAsset SkeletonDataAsset { get; }
    }
}
