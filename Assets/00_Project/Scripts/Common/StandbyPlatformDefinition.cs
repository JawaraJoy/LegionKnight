using UnityEngine;
using UnityEngine.AddressableAssets;

namespace LegionKnight
{
    public class StandbyPlatformDefinition : ScriptableObject
    {
        [SerializeField]
        private string m_Name;
        [SerializeField, TextArea]
        private string m_Description;
        [SerializeField]
        private Sprite m_Icon;
        [SerializeField, Range(1, 100)]
        private int m_ChanceRateTospawn;
        [SerializeField]
        private AssetReferenceGameObject m_Platform;
        public int ChanceRateToSpawn => m_ChanceRateTospawn;
        public AssetReferenceGameObject Platform => m_Platform;
        public string Name => $"{m_Name} Platform";
        public Sprite Icon => m_Icon;
    }
}
