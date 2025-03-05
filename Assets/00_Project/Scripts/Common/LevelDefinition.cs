using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace LegionKnight
{
    [CreateAssetMenu(fileName = "New Level", menuName = "Legion Knight/Level")]
    public partial class LevelDefinition : ScriptableObject
    {
        [SerializeField]
        private float m_MinSpeed;
        [SerializeField]
        private float m_MaxSpeed;
        [SerializeField]
        private CurrencyDefinition m_TouchDownRewardCurrency;
        [SerializeField]
        private int m_NormalTouchDownPoint;
        [SerializeField]
        private int m_PerfectTouchDownPoint;
        [SerializeField]
        private List<AssetReferenceGameObject> m_PlatformAssets = new();

        public float GetSpeed()
        {
            float random = Random.Range(m_MinSpeed, m_MaxSpeed);
            return random;
        }
        public Currency GetNormalTouchDown()
        {
            return new Currency(m_TouchDownRewardCurrency, m_NormalTouchDownPoint);
        }
        public Currency GetPerfectTouchdown()
        {
            return new Currency(m_TouchDownRewardCurrency, m_PerfectTouchDownPoint);
        }
        public int GetNormalTouchDownPoint()
        {
            return m_NormalTouchDownPoint;
        }
        public int GetPerfectTouchDownPoint()
        {
            return m_PerfectTouchDownPoint;
        }
        private AssetReferenceGameObject GetPlatformAssetsRandom()
        {
            int random = Random.Range(0, m_PlatformAssets.Count);
            return m_PlatformAssets[random];
        }
        public AssetReferenceGameObject PlatformAsset => GetPlatformAssetsRandom();
    }
}
