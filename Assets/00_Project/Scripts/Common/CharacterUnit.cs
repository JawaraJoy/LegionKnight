using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace LegionKnight
{
    [System.Serializable]
    public partial class CharacterUnit
    {
        [SerializeField]
        private CharacterDefinition m_Definition;
        [SerializeField]
        private bool m_Owned;

        [SerializeField]
        private int m_Star = 1;
        [SerializeField]
        private int m_Level = 1;
        private int m_Exp;

        [SerializeField]
        private bool m_IsUsed = false;
        [SerializeField]
        private UnityEvent<CharacterUnit> m_OnCharacterStarUp = new();
        [SerializeField]
        private UnityEvent<CharacterUnit> m_OnCharacterShardUpdate = new();
        public bool Owned => m_Owned;

        public int Level => m_Level;
        public int MaxLevel => m_LevelFormulaDefinition.MaxLevel;
        public int Exp => m_Exp;
        public int Star => m_Star;
        public int MaxStar => m_Definition.MaxStars;
        public bool IsUsed => m_IsUsed;
        public int CurrentMaxExp => m_LevelFormulaDefinition.GetCurrentMaxExperience(m_Level);
        public CurrencyDefinition ShardDefinition => m_LevelFormulaDefinition.ShardDefinition;
        [SerializeField]
        private LevelFormulaDefinition m_LevelFormulaDefinition;

        [SerializeField]
        private UnityEvent<CharacterUnit> m_OnLevelUp = new();
        [SerializeField]
        private UnityEvent<CharacterUnit> m_OnExpUpdate = new();
        public void SetExp(int exp)
        {
            m_Exp = exp;
            int maxLevel = m_LevelFormulaDefinition.MaxLevel;
            int currentMaxExp = m_LevelFormulaDefinition.GetCurrentMaxExperience(m_Level);
            while (m_Level < maxLevel && m_Exp >= currentMaxExp)
            {
                LevelUpInternal();
            }
            UnityService.Instance.SaveData(m_Definition.Id + "Exp", m_Exp);
            m_OnExpUpdate?.Invoke(this);
            Debug.Log($"Current Exp: {m_Exp}, Level: {m_Level}");
        }
        public void SetIsUsed(bool isUsed)
        {
            m_IsUsed = isUsed;
            UnityService.Instance.SaveData("used" + m_Definition.Id, m_IsUsed);
        }
        public void AddExp(int exp)
        {
            AddExpInternal(exp);
        }
        private void AddExpInternal(int exp)
        {
            m_Exp += exp;
            int maxLevel = m_LevelFormulaDefinition.MaxLevel;
            int currentMaxExp = m_LevelFormulaDefinition.GetCurrentMaxExperience(m_Level);
            while (m_Level < maxLevel && m_Exp >= currentMaxExp)
            {
                LevelUpInternal();
            }
            UnityService.Instance.SaveData(m_Definition.Id + "Exp", m_Exp);
            m_OnExpUpdate?.Invoke(this);
            Debug.Log($"Current Exp: {m_Exp}, Level: {m_Level}");
        }
        public void AddLevel(int level)
        {
            if (level <= 0) return;
            m_Level += level;
            if (m_Level > m_LevelFormulaDefinition.MaxLevel)
            {
                m_Level = m_LevelFormulaDefinition.MaxLevel;
            }
            UnityService.Instance.SaveData(m_Definition.Id + "Lv", m_Level);
            m_OnLevelUp?.Invoke(this);
            Player.Instance.OnHeroLevelUp.Invoke(m_Definition);
        }
        public void LevelUp()
        {
            LevelUpInternal();
        }
        private void LevelUpInternal()
        {
            int maxLevel = m_LevelFormulaDefinition.MaxLevel;
            int currentMaxExp = m_LevelFormulaDefinition.GetCurrentMaxExperience(m_Level);
            if (m_Level < maxLevel)
            {
                m_Exp -= currentMaxExp;
                m_Level++;
                //UnityService.Instance.SaveData(m_CurrentLevelKey, m_Level);
                //OnLevelUpInvoke();
                m_OnLevelUp?.Invoke(this);
                Player.Instance.OnHeroLevelUp.Invoke(m_Definition);
                UnityService.Instance.SaveData(m_Definition.Id + "Lv", m_Level);
            }
            else
            {
                m_Exp = currentMaxExp; // Ensure exp does not exceed max level exp
            }
        }
        public void AddStar(int add)
        {
            AddStarInternal(add);
        }
        private void AddStarInternal(int add)
        {
            if (add <= 0) return;
            m_Star += add;
            if (m_Star > m_Definition.MaxStars)
            {
                m_Star = m_Definition.MaxStars;
            }
            UnityService.Instance.SaveData(m_Definition.Id + "Star", m_Star);
            OnCharacterStarUpInvoke();
        }
        private void OnCharacterStarUpInvoke()
        {
            m_OnCharacterStarUp?.Invoke(this);
        }
        private void OnCharacterShardUpdateInvoke()
        {
            m_OnCharacterShardUpdate?.Invoke(this);
        }
        public void SetOwned(bool set)
        {
            
            SetOwnedInternal(set);
        }
        private void SetOwnedInternal(bool set)
        {
            m_Owned = set;
            UnityService.Instance.SaveData(m_Definition.Id + "Owned", m_Owned);
        }
        public void Init()
        {
            if (UnityService.Instance.HasData(m_Definition.Id + "Owned"))
            {
                m_Owned = UnityService.Instance.GetData<bool>(m_Definition.Id + "Owned");
            }
            if (UnityService.Instance.HasData(m_Definition.Id + "Exp"))
            {
                m_Exp = UnityService.Instance.GetData<int>(m_Definition.Id + "Exp");
            }
            else
            {
                m_Exp = 0;
            }
            if (UnityService.Instance.HasData(m_Definition.Id + "Lv"))
            {
                m_Level = UnityService.Instance.GetData<int>(m_Definition.Id + "Lv");
            }
            if (UnityService.Instance.HasData(m_Definition.Id + "Star"))
            {
                m_Star = UnityService.Instance.GetData<int>(m_Definition.Id + "Star");
            }
            else
            {
                m_Star = m_Definition.StartingStars;
            }
            //m_Owned = UnityService.Instance.GetData<bool>(m_Definition.Id + "Owned");
            if (m_Definition == Player.Instance.DefaultCharacter)
            {
                SetOwnedInternal(true);
            }
            if (UnityService.Instance.HasData("used" + m_Definition.Id))
            {
                bool used = UnityService.Instance.GetData<bool>("used" + m_Definition.Id);
                m_IsUsed = used;
            }
        }
        public string CharacterName => m_Definition.name;
        public Sprite Icon => m_Definition.Icon;
        public Sprite SmallIcon => m_Definition.SmallIcon;
        public StandbyPlatformDefinition UniquePlatform => m_Definition.UniquePlatform;
        public List<SkillDefinition> Passives => m_Definition.Passives;
        public List<SkillDefinition> Weapons => m_Definition.Weapons;
        public AbilityDefinition Ability => m_Definition.Ability;
        public CharacterDefinition Definition => m_Definition;
        public Stat FinalStat()
        {
            return m_Definition.FinalStat(m_Star, m_Level);
        }
        public Stat NextFinalStat()
        {
            return m_Definition.NextFinalStat(m_Star, m_Level);
        }
        public bool CanBreak()
        {
            return m_Definition.CanBreak(m_Star, m_Level);
        }
        public Currency GetBreakCost()
        {
            return m_Definition.GetBreakCost(m_Star);
        }
    }
}
