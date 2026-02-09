using Spine.Unity;
using UnityEngine;

namespace Rush
{
    public class UnitConfigSpine
    {   
    }

    public partial class UnitConfig
    {
        [SerializeField]
        private SkeletonDataAsset m_SkeletonDataAsset;
        public SkeletonDataAsset SkeletonDataAsset => m_SkeletonDataAsset;
    }
}
