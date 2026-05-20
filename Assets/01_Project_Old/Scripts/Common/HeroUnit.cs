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
        private PreparationPanel m_PreparationPanel;
        private PreparationPanel PreparationPanel
        {
            get
            {
                if (m_PreparationPanel == null)
                {
                    m_PreparationPanel = CanvasManager.Instance.GetPanel<PreparationPanel>();
                }
                return m_PreparationPanel;
            }
        }
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
            return m_HeroConfig.BreakThroughFormulaConfig.GetLevelNeeded(m_Star);
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

        private void SaveExp()
        {
            UnityService.Instance.SaveData(m_HeroConfig.BaseInfo.Id + "Exp", m_Exp);
        }

        private void SaveLevel()
        {
            UnityService.Instance.SaveData(m_HeroConfig.BaseInfo.Id + "Lv", m_Level);
        }

        private void InvokeLevelUpEvents()
        {
            m_OnLevelUp?.Invoke(this);
            Player.Instance.HeroesCollection.OnCharacterLevelUp.Invoke(m_HeroConfig);
            Player.Instance.HeroesCollection.OnCharacterLevelUpAmount.Invoke(m_Level);
            PreparationPanel.HeroTabView.Init();
        }

        // =========================
        // EXP
        // =========================

        public void SetExp(int exp)
        {
            m_Exp = exp;
            ProcessLevelUp();
            SaveExp();
            m_OnExpUpdate?.Invoke(this);

            Debug.Log($"[EXP] {HeroName} Exp: {m_Exp}, Level: {m_Level}");
        }

        public void AddExp(int exp)
        {
            m_Exp += exp;
            ProcessLevelUp();
            SaveExp();
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

            SaveLevel();
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

            SaveLevel();
            InvokeLevelUpEvents();

            Debug.Log($"[LEVEL UP] {HeroName} → {m_Level}");
        }

        // =========================
        // BREAKTHROUGH (STAR)
        // =========================

        public bool CanBreak()
        {
            return m_HeroConfig.BreakThroughFormulaConfig
                .CanBreakByLevel(m_Star, m_Level);
        }

        public bool TryBreak()
        {
            var config = m_HeroConfig.BreakThroughFormulaConfig;

            if (!config.CanBreakByLevel(m_Star, m_Level))
            {
                Debug.Log("Break failed: level requirement not met");
                return false;
            }

            if (!config.TryGetBreakCosts(m_Star, out int shardCost, out int coinCost))
            {
                Debug.Log("Break failed: invalid config");
                return false;
            }

            var currencyControl = Player.Instance.CurrencyControl;

            if (!currencyControl.HasCurrency(config.ShardConfig, out var shardCurrency))
            {
                Debug.Log("Break failed: shard currency not found");
                return false;
            }

            if (!currencyControl.HasCurrency(config.CoinConfig, out var coinCurrency))
            {
                Debug.Log("Break failed: coin currency not found");
                return false;
            }

            if (shardCurrency.Amount < shardCost)
            {
                Debug.Log("Break failed: not enough shard");
                return false;
            }

            if (coinCurrency.Amount < coinCost)
            {
                Debug.Log("Break failed: not enough coin");
                return false;
            }

            currencyControl.RemoveCurrencyAmount(config.ShardConfig, shardCost);
            currencyControl.RemoveCurrencyAmount(config.CoinConfig, coinCost);

            AddStar(1);

            Debug.Log($"[BREAKTHROUGH] {HeroName} → ⭐{m_Star}");

            return true;
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
                m_Star = m_HeroConfig.MaxStars;

            UnityService.Instance.SaveData(m_HeroConfig.BaseInfo.Id + "Star", m_Star);

            m_OnCharacterStarUp?.Invoke(this);
            Player.Instance.HeroesCollection.OnCharacterStarUp.Invoke(m_HeroConfig);

            //TenjinManager.Instance.SendEventToCharacterBreakthrough(m_Star);    

            Debug.Log($"[STAR UP] {HeroName} → ⭐{m_Star}");

            PreparationPanel.HeroTabView.Init();
        }

        // =========================
        // OWNERSHIP
        // =========================

        public void SetOwned(bool set)
        {
            m_Owned = set;
            m_OnCharacterOwnedChanged?.Invoke(m_Owned);

            UnityService.Instance.SaveData(m_HeroConfig.BaseInfo.Id + "Owned", m_Owned);
        }

        public void SetIsUsed(bool isUsed)
        {
            m_IsUsed = isUsed;
            UnityService.Instance.SaveData("used" + m_HeroConfig.BaseInfo.Id, m_IsUsed);
        }

        // =========================
        // INIT
        // =========================

        public void Init()
        {
            if (UnityService.Instance.HasData(m_HeroConfig.BaseInfo.Id + "Owned"))
                m_Owned = UnityService.Instance.GetData<bool>(m_HeroConfig.BaseInfo.Id + "Owned");
            if (UnityService.Instance.HasData(m_HeroConfig.BaseInfo.Id + "Exp"))
                m_Exp = UnityService.Instance.GetData<int>(m_HeroConfig.BaseInfo.Id + "Exp");
            if (UnityService.Instance.HasData(m_HeroConfig.BaseInfo.Id + "Lv"))
                m_Level = UnityService.Instance.GetData<int>(m_HeroConfig.BaseInfo.Id + "Lv");
            if (UnityService.Instance.HasData(m_HeroConfig.BaseInfo.Id + "Star"))
                m_Star = UnityService.Instance.GetData<int>(m_HeroConfig.BaseInfo.Id + "Star");

            if (m_HeroConfig.OwnedAtFirst)
                SetOwned(true);

            if (UnityService.Instance.HasData("used" + m_HeroConfig.BaseInfo.Id))
                m_IsUsed = UnityService.Instance.GetData<bool>("used" + m_HeroConfig.BaseInfo.Id);

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
            var bonus = m_HeroConfig.BreakThroughFormulaConfig.GetStatBonus(m_Star);

            return baseStat + bonus;
        }

        public StatField NextFinalStat()
        {
            return m_HeroConfig.MainStats.GetFinalStat(Level + 1);
        }
    }
}