using UnityEngine;
using UnityEngine.Events;
using Rush;
using MoreMountains.Tools;

namespace LegionKnight
{
    [System.Serializable]
    public partial class HeroUnit
    {
        [SerializeField] private HeroUnitConfig m_HeroConfig;
        [SerializeField] private bool m_Owned;

        [SerializeField] private int m_Star = 1;
        [SerializeField] private int m_Level = 1;
        private int m_Exp;

        [SerializeField] private bool m_IsUsed = false;
        [SerializeField, MMReadOnly]
        private bool m_OnTrial = false;
        public bool OnTrial => m_OnTrial;

        [SerializeField] private UnityEvent<HeroUnit> m_OnCharacterStarUp = new();
        [SerializeField] private UnityEvent<HeroUnit> m_OnCharacterShardUpdate = new();
        [SerializeField] private UnityEvent<bool> m_OnCharacterOwnedChanged = new();

        [SerializeField] private UnityEvent<HeroUnit> m_OnLevelUp = new();
        [SerializeField] private UnityEvent<HeroUnit> m_OnExpUpdate = new();

        public bool Owned => m_Owned;
        public int Level => m_Level;
        public int Exp => m_Exp;
        public int Star => m_Star;
        public int MaxStar => m_HeroConfig.MaxStars;
        public bool IsUsed => m_IsUsed;

        // =========================
        // PROGRESSION CORE
        // =========================
        public void SetTrial(bool onTrial)
        {
            m_OnTrial = onTrial;
        }
        public int GetMaxLevelByStar()
        {
            return 10;
        }

        private int GetAllowedMaxLevel()
        {
            int globalMax = m_HeroConfig.LevelFormulaConfig.MaxLevel;
            int starMax = GetMaxLevelByStar();

            if (starMax <= 0) return globalMax;

            return Mathf.Min(globalMax, starMax);
        }

        private void ProcessLevelUp()
        {
            int maxLevel = GetAllowedMaxLevel();

            while (m_Level < maxLevel)
            {
                int requiredExp = m_HeroConfig.LevelFormulaConfig.GetCurrentMaxExperience(m_Level);

                if (m_Exp < requiredExp)
                    break;

                m_Exp -= requiredExp;
                LevelUpInternal(false);
            }
        }

        private void InvokeLevelUpEvents()
        {
            m_OnLevelUp?.Invoke(this);
        }

        // =========================
        // EXP
        // =========================

        public void SetExp(int exp)
        {
            m_Exp = exp;
            ProcessLevelUp();
            m_OnExpUpdate?.Invoke(this);

            Debug.Log($"[EXP] {HeroName} Exp: {m_Exp}, Level: {m_Level}");
        }

        public void AddExp(int exp)
        {
            m_Exp += exp;
            ProcessLevelUp();
            m_OnExpUpdate?.Invoke(this);

            Debug.Log($"[EXP+] {HeroName} Exp: {m_Exp}, Level: {m_Level}");
        }

        // =========================
        // LEVEL
        // =========================

        public void AddLevel(int level)
        {
            if (level <= 0) return;

            m_Level += level;

            int maxAllowed = GetAllowedMaxLevel();
            if (m_Level > maxAllowed)
                m_Level = maxAllowed;
            InvokeLevelUpEvents();

            //TenjinManager.Instance.SendEventToHeroLevelUp(m_HeroConfig, m_Level);

            Debug.Log($"[LEVEL+] {HeroName} Level: {m_Level}");
        }

        public void LevelUp()
        {
            LevelUpInternal(true);
        }

        private void LevelUpInternal(bool consumeExp = true)
        {
            int maxLevel = GetAllowedMaxLevel();

            if (m_Level >= maxLevel)
                return;

            if (consumeExp)
            {
                int requiredExp = m_HeroConfig.LevelFormulaConfig.GetCurrentMaxExperience(m_Level);
                if (m_Exp < requiredExp) return;

                m_Exp -= requiredExp;
            }

            m_Level++;

            InvokeLevelUpEvents();

            Debug.Log($"[LEVEL UP] {HeroName} → {m_Level}");
        }

        // =========================
        // BREAKTHROUGH (STAR)
        // =========================

        

        

        // =========================
        // OWNERSHIP
        // =========================

        public void SetOwned(bool set)
        {
            m_Owned = set;
            m_OnCharacterOwnedChanged?.Invoke(m_Owned);

        }

        // =========================
        // INIT
        // =========================

        public void Init()
        {

            m_OnCharacterOwnedChanged?.Invoke(m_Owned);
        }

        // =========================
        // STAT
        // =========================

        public string HeroName => m_HeroConfig.name;
        public PlatformConfig[] UniquePlatforms => m_HeroConfig.UniquePlatforms;
        public HeroUnitConfig HeroConfig => m_HeroConfig;

        public StatField FinalStat()
        {
            var baseStat = m_HeroConfig.MainStats.GetFinalStat(m_Level);

            return baseStat;
        }

        public StatField NextFinalStat()
        {
            return m_HeroConfig.MainStats.GetFinalStat(Level + 1);
        }
    }
}