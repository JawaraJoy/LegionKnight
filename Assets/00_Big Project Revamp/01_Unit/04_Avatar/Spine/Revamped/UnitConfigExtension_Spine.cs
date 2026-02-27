using Spine.Unity;
using UnityEngine;

namespace Rush
{
    public class UnitConfigExtension_Spine
    {   
    }

    public abstract partial class UnitConfig : IHasSekeletonAsset
    {
        [SerializeField]
        private SkeletonDataAsset m_SkeletonDataAsset;
        public SkeletonDataAsset SkeletonDataAsset => m_SkeletonDataAsset;
    }
}
