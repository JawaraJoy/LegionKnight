using AppsFlyerSDK;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Rush;

namespace LegionKnight
{
    [System.Serializable]
    public partial class HeroUnit
    {
        [SerializeField]
        private HeroUnitConfig m_HeroConfig;
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
        private UnityEvent<HeroUnit> m_OnCharacterStarUp = new();
        [SerializeField]
        private UnityEvent<HeroUnit> m_OnCharacterShardUpdate = new();
        [SerializeField]
        private UnityEvent<bool> m_OnCharacterOwnedChanged = new();
        public bool Owned => m_Owned;

        public int Level => m_Level;
        public int Exp => m_Exp;
        public int Star => m_Star;
        public int MaxStar => m_HeroConfig.MaxStars;
        public bool IsUsed => m_IsUsed;
        [SerializeField]
        private LevelFormulaConfig m_LevelFormulaDefinition;
        public LevelFormulaConfig LevelFormulaDefinition => m_LevelFormulaDefinition;

        [SerializeField]
        private UnityEvent<HeroUnit> m_OnLevelUp = new();
        [SerializeField]
        private UnityEvent<HeroUnit> m_OnExpUpdate = new();
        public void SetExp(int exp)
        {
            m_Exp = exp;
            int maxLevel = m_LevelFormulaDefinition.MaxLevel;
            int currentMaxExp = m_LevelFormulaDefinition.GetCurrentMaxExperience(m_Level);
            while (m_Level < maxLevel && m_Exp >= currentMaxExp)
            {
                LevelUpInternal();
            }
            UnityService.Instance.SaveData(m_HeroConfig.BaseInfo.Id + "Exp", m_Exp);
            m_OnExpUpdate?.Invoke(this);
            Debug.Log($"Current Exp: {m_Exp}, Level: {m_Level}");
        }
        public void SetIsUsed(bool isUsed)
        {
            m_IsUsed = isUsed;
            UnityService.Instance.SaveData("used" + m_HeroConfig.BaseInfo.Id, m_IsUsed);
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
            UnityService.Instance.SaveData(m_HeroConfig.BaseInfo.Id + "Exp", m_Exp);
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
            UnityService.Instance.SaveData(m_HeroConfig.BaseInfo.Id + "Lv", m_Level);
            m_OnLevelUp?.Invoke(this);
            Player.Instance.HeroDeck.OnCharacterLevelUp.Invoke(m_HeroConfig);
            Player.Instance.HeroDeck.OnCharacterLevelUpAmount.Invoke(m_Level);
            Debug.Log($"Level Up! New Level: {m_Level}");

            //--Tenjin Record
            TenjinManager.Instance.SendEventToHeroLevelUp(m_HeroConfig, m_Level);
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
                Player.Instance.HeroDeck.OnCharacterLevelUp.Invoke(m_HeroConfig);
                Player.Instance.HeroDeck.OnCharacterLevelUpAmount.Invoke(m_Level);

                Debug.Log($"Level Up! New Level: {m_Level}");

                UnityService.Instance.SaveData(m_HeroConfig.BaseInfo.Id + "Lv", m_Level);
                
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
            if (m_Star > m_HeroConfig.MaxStars)
            {
                m_Star = m_HeroConfig.MaxStars;
            }
            UnityService.Instance.SaveData(m_HeroConfig.BaseInfo.Id + "Star", m_Star);
            OnCharacterStarUpInvoke();
            Player.Instance.HeroDeck.OnCharacterStarUp.Invoke(m_HeroConfig);

            //--Tenjin Record
            TenjinManager.Instance.SendEventToCharacterBreakthrough(m_Star);
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
            OnCharacterOwnedChangedInvoke();
            UnityService.Instance.SaveData(m_HeroConfig.BaseInfo.Id + "Owned", m_Owned);
        }

        private void OnCharacterOwnedChangedInvoke()
        {
            m_OnCharacterOwnedChanged?.Invoke(m_Owned);
        }

        public void Init()
        {
            if (UnityService.Instance.HasData(m_HeroConfig.BaseInfo.Id + "Owned"))
            {
                m_Owned = UnityService.Instance.GetData<bool>(m_HeroConfig.BaseInfo.Id + "Owned");
            }
            if (UnityService.Instance.HasData(m_HeroConfig.BaseInfo.Id + "Exp"))
            {
                m_Exp = UnityService.Instance.GetData<int>(m_HeroConfig.BaseInfo.Id + "Exp");
            }
            else
            {
                m_Exp = 0;
            }
            if (UnityService.Instance.HasData(m_HeroConfig.BaseInfo.Id + "Lv"))
            {
                m_Level = UnityService.Instance.GetData<int>(m_HeroConfig.BaseInfo.Id + "Lv");
            }
            if (UnityService.Instance.HasData(m_HeroConfig.BaseInfo.Id + "Star"))
            {
                m_Star = UnityService.Instance.GetData<int>(m_HeroConfig.BaseInfo.Id + "Star");
            }
            else
            {
                m_Star = m_HeroConfig.StartingStars;
            }
            //m_Owned = UnityService.Instance.GetData<bool>(m_Definition.Id + "Owned");
            if (m_HeroConfig == Player.Instance.HeroDeck.DefaultHero)
            {
                SetOwnedInternal(true);
            }
            if (UnityService.Instance.HasData("used" + m_HeroConfig.BaseInfo.Id))
            {
                bool used = UnityService.Instance.GetData<bool>("used" + m_HeroConfig.BaseInfo.Id);
                m_IsUsed = used;
            }
            OnCharacterOwnedChangedInvoke();
        }
        public string HeroName => m_HeroConfig.name;
        public PlatformConfig[] UniquePlatforms => m_HeroConfig.UniquePlatforms;
        public HeroUnitConfig HeroConfig => m_HeroConfig;
        public StatField FinalStat()
        {
            return m_HeroConfig.MainStats.GetFinalStat(m_Level);
        }
        public StatField NextFinalStat()
        {
            return m_HeroConfig.MainStats.GetFinalStat(Level + 1);
        }
        
    }
}
