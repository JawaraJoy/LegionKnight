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
        private Currency m_NormalTouchDown;
        [SerializeField]
        private Currency m_PerfectTouchDown;
        [SerializeField]
        private List<AssetReferenceGameObject> m_PlatformAssets = new();

        public float GetSpeed()
        {
            float random = Random.Range(m_MinSpeed, m_MaxSpeed);
            return random;
        }
        public Currency GetNormalTouchDown()
        {
            return m_NormalTouchDown;
        }
        public Currency GetPerfectTouchdown()
        {
            return m_PerfectTouchDown;
        }
        private AssetReferenceGameObject GetPlatformAssetsRandom()
        {
            int random = Random.Range(0, m_PlatformAssets.Count);
            return m_PlatformAssets[random];
        }
        public AssetReferenceGameObject PlatformAsset => GetPlatformAssetsRandom();
    }
}
