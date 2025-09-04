using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace LegionKnight
{
    public class MissionMonitor : UIView
    {
        [SerializeField]
        private AssetReferenceGameObject m_MissionViewAsset;
        [SerializeField]
        private Transform m_MissionViewParent;

        private readonly List<MissionView> m_MissionViews = new();
    }
}
