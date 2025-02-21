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
        private AssetReferenceGameObject m_PlatformAsset;

        public float GetSpeed()
        {
            float random = Random.Range(m_MinSpeed, m_MaxSpeed);
            return random;
        }
        public AssetReferenceGameObject PlatformAsset => m_PlatformAsset;
    }
}
