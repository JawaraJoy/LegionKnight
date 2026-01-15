using LegionKnight;
using System;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Rush
{
    public class PlatformConfig : UnitConfig
    {
        [SerializeField]
        private Sprite m_BigIcon;
        public Sprite BigIcon => m_BigIcon;
        [SerializeField, Range(0.01f, 1f)]
        private int m_ChanceToSpawn = 1;

        [Obsolete]
        [SerializeField]
        private WheelItemDefinition m_WheelItemDefinition;
        [Obsolete]
        public WheelItemDefinition WheelItemDefinition => m_WheelItemDefinition;
        public int ChanceToSpawn => m_ChanceToSpawn;
        [SerializeField]
        private AssetReferenceGameObject m_PlatformPrefab;
        public AssetReferenceGameObject PlatformPrefab => m_PlatformPrefab;
        public void SetIsEquiped(bool isEquiped)
        {
            Player.Instance.SetPlatformUnitIsEquiped(this, isEquiped);
        }
    }
}
