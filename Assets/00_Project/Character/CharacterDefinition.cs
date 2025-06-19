using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace LegionKnight
{
    public enum Rarity
    {
        Common,
        Uncommon,
        Rare,
        Epic,
        Legendary
    }
    [CreateAssetMenu(fileName = "New Character", menuName = "Legion Knight/Character")]
    public partial class CharacterDefinition : ScriptableObject
    {
        [SerializeField]
        private string m_Id;
        [SerializeField]
        private string m_Label;
        [SerializeField]
        private Sprite m_Icon;
        [SerializeField]
        private Sprite m_SmallIcon;

        [SerializeField]
        private Stat m_BaseStat;
        [SerializeField]
        private Stat m_StatGainPerLevel;
        [SerializeField]
        private Currency m_ShardConvert;
        public string Id => m_Id;
        public Sprite Icon => m_Icon;
        public Stat BaseStat => m_BaseStat;
        public Stat StatGainPerLevel => m_StatGainPerLevel;
        public Currency ShardConvert => m_ShardConvert;
        public Sprite SmallIcon => m_SmallIcon;
        public string Label => m_Label;
        [SerializeField]
        private StandbyPlatformDefinition m_UniquePlatform;
        [SerializeField]
        private List<SkillDefinition> m_Weapons = new();
        [SerializeField]
        private List<SkillDefinition> m_Passives = new();
        [SerializeField]
        private AbilityDefinition m_Ability;
        public AbilityDefinition Ability => m_Ability;
        public StandbyPlatformDefinition UniquePlatform => m_UniquePlatform;
        public List<SkillDefinition> Weapons => m_Weapons;
        public List<SkillDefinition> Passives => m_Passives;

        public Stat FinalStat(int level)
        {
            return Stat.GetStatByLevel(m_BaseStat, m_StatGainPerLevel, level);
        }
    }
    [System.Serializable]
    public partial class SkillDefinition
    {
        [SerializeField]
        private string m_SkillName;
        [SerializeField, TextArea]
        private string m_Description;
        [SerializeField]
        private Sprite m_Icon;
        [SerializeField]
        private int m_ManaThreshold;
        [SerializeField]
        private AssetReferenceGameObject[] m_SkillAsset;
        public AssetReferenceGameObject[] SkillAsset => m_SkillAsset;
        public string SkillName => m_SkillName;
        public Sprite Icon => m_Icon;
        public int Manathreshold => m_ManaThreshold;
        public string Description => m_Description;
    }
}
