using UnityEngine;
using UnityEngine.AddressableAssets;

namespace LegionKnight
{
    [CreateAssetMenu(fileName = "New Standby Platform", menuName = "Legion Knight/Standby Platform")]
    public partial class StandbyPlatformDefinition : ScriptableObject, IDescriptable
    {
        [SerializeField]
        private string m_Id;
        [SerializeField]
        private string m_Label;
        [SerializeField, TextArea]
        private string m_Description;
        [SerializeField]
        private Sprite m_Icon;
        
        [SerializeField]
        private Rarity m_Rarity = Rarity.Common;
        [SerializeField]
        private Color m_RarityColor; 
        [SerializeField, Range(1, 100)]
        private int m_ChanceRateTospawn;
        [SerializeField]
        private AssetReferenceGameObject m_Platform;

        [SerializeField]
        private AbilityDefinition m_AbilityDefinition;
        public int ChanceRateToSpawn => m_ChanceRateTospawn;
        public AssetReferenceGameObject Platform => m_Platform;
        public string Label => $"{m_Label} Platform";
        public string Description => m_Description;
        public Sprite Icon => m_Icon;
        public string Id => m_Id;
        public Rarity Rarity => m_Rarity;
        public Color RarityColor => m_RarityColor;
        public AbilityDefinition AbilityDefinition => m_AbilityDefinition;

        public void SetIsEquiped(bool isEquiped)
        {
            Player.Instance.SetPlatformUnitIsEquiped(this, isEquiped);
        }
    }
}
