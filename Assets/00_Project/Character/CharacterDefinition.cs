using Spine.Unity;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace LegionKnight
{
    public enum Rarity
    {
        Common,
        Rare,
        Epic,
    }
    [CreateAssetMenu(fileName = "New Character", menuName = "Legion Knight/Character/Unit")]
    public partial class CharacterDefinition : ScriptableObject, IDescriptable, IOwner, IAbilityOwner
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
        private Sprite m_SmallIcon;

        [SerializeField]
        private Sprite m_TypeIcon;
        [SerializeField]
        private Rarity m_Rarity = Rarity.Common;
        [SerializeField]
        private Color m_ColorRarity = Color.white;
        [SerializeField]
        private int m_StartingStars = 1;
        [SerializeField]
        private BreakThroughFormulaDefinition m_BreakThrough;
        [SerializeField]
        private AssetReferenceGameObject m_CharacterPrefab;
        [SerializeField]
        private SkeletonDataAsset m_SkeletonDataAsset;
        [SerializeField]
        private Stat m_BaseStat;
        [SerializeField]
        private Stat m_StatGainPerLevel;
        [SerializeField]
        private Currency m_ShardConvert;
        public string Id => m_Id;
        public string Description => m_Description;
        public Sprite Icon => m_Icon;
        public Stat BaseStat => m_BaseStat;
        public Stat StatGainPerLevel => m_StatGainPerLevel;
        public Currency ShardConvert => m_ShardConvert;
        public Sprite SmallIcon => m_SmallIcon;
        public Rarity Rarity => m_Rarity;
        public Sprite TypeIcon => m_TypeIcon;
        public string Label => m_Label;
        public int MaxStars => m_BreakThrough.GetMaxStar();
        public Color ColorRarity => m_ColorRarity;
        [SerializeField]
        private StandbyPlatformDefinition m_UniquePlatform;
        [SerializeField]
        private List<SkillDefinition> m_Weapons = new();
        [SerializeField]
        private List<SkillDefinition> m_Passives = new();

        public AssetReferenceGameObject CharacterPrefab => m_CharacterPrefab;
        public SkeletonDataAsset SkeletonDataAsset => m_SkeletonDataAsset;
        public StandbyPlatformDefinition UniquePlatform => m_UniquePlatform;
        public List<SkillDefinition> Weapons => m_Weapons;
        public List<SkillDefinition> Passives => m_Passives;
        public int StartingStars => m_StartingStars;

        public void SetOwner(Object owner)
        {
            // the owner no need to set
        }

        public Currency GetBreakShardCost(int star)
        {
            int amount = m_BreakThrough.GetShardAmountToBreak(star);
            return new Currency(m_BreakThrough.ShardDefinition, amount);
        }
        public Currency GetBreakCoinCost(int star)
        {
            int amount = m_BreakThrough.GetCoinAmountToBreak(star);
            return new Currency(m_BreakThrough.CoinDefinition, amount);
        }
        public bool CanBreak(int star, int level)
        {
            return m_BreakThrough.CanBreak(star, level);
        }
        public Stat FinalStat(int star, int level)
        {
            if (star < 0 || star > m_BreakThrough.GetMaxStar())
            {
                Debug.LogError($"Invalid star level: {star}. Must be between 0 and {m_BreakThrough.GetMaxStar()}.");
                return null;
            }
            if (level < 1)
            {
                Debug.LogError($"Invalid level: {level}. Must be greater than 0.");
                return null;
            }
            Stat levelStat = Stat.GetStatByLevel(m_BaseStat, m_StatGainPerLevel, level);
            Stat starStat = m_BreakThrough.GetStatBonus(star);
            if (starStat == null)
            {
                Debug.LogError($"No stat bonus defined for star level: {star}.");
                return null;
            }
            Stat finalStat = levelStat + starStat;
            return finalStat;
        }

        public Stat NextFinalStat(int star, int level)
        {
            if (star < 0 || star > m_BreakThrough.GetMaxStar())
            {
                Debug.LogError($"Invalid star level: {star}. Must be between 0 and {m_BreakThrough.GetMaxStar()}.");
                return null;
            }
            if (level < 1)
            {
                Debug.LogError($"Invalid level: {level}. Must be greater than 0.");
                return null;
            }
            Stat levelStat = Stat.GetStatByLevel(m_BaseStat, m_StatGainPerLevel, level);
            Stat nextLevelStat = Stat.GetStatByLevel(m_BaseStat, m_StatGainPerLevel, level + 1);
            bool canBreak = m_BreakThrough.CanBreak(star, level);
            Stat starStat = m_BreakThrough.GetStatBonus(star);
            Stat nextstarStat = m_BreakThrough.GetStatBonus(star + 1);
            if (nextstarStat == null)
            {
                Debug.LogError($"No stat bonus defined for star level: {star}.");
                return null;
            }
            Stat finalStat = nextLevelStat + starStat;
            if (canBreak)
            {
                finalStat = levelStat + nextstarStat;
            }
            return finalStat;
        }

        public int GetUnitLevel()
        {
            CharacterUnit unit = Player.Instance.GetCharacterUnit(this);
            return unit.Level;
        }

        public void InitAsOwner()
        {
            foreach (var pass in m_Passives)
            {
                pass.SetOwner(this);
            }
            m_UniquePlatform.SetOwner(this);
        }
    }
    [System.Serializable]
    public partial class SkillDefinition : IObjectHasOwner, IAbilityOwner
    {
        [SerializeField]
        private string m_SkillName;
        [SerializeField, TextArea]
        private string m_Description;
        [SerializeField]
        private Sprite m_Icon;
        [SerializeField]
        private AbilityDefinition m_AbilityDefinition;
        [SerializeField]
        private int m_ManaThreshold;
        [SerializeField]
        private AssetReferenceGameObject[] m_SkillAsset;
        public AssetReferenceGameObject[] SkillAsset => m_SkillAsset;
        public string SkillName => m_SkillName;
        public Sprite Icon => m_Icon;
        public int Manathreshold => m_ManaThreshold;
        public string Description => m_Description;
        public AbilityDefinition AbilityDefinition => m_AbilityDefinition;
        private Object m_Owner;
        public Object Owner => m_Owner;
        public void SetOwner(Object owner)
        {
            m_Owner = owner;
            if (m_AbilityDefinition == null) return;
            m_AbilityDefinition.SetOwner(owner);
        }

        public int GetUnitLevel()
        {
            return AbilityUtil.GetOwnerLevel(m_Owner);
        }
    }
}
