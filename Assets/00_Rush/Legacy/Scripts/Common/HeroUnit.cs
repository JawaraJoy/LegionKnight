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

        [SerializeField] private int m_Star = 0;
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
                    m_PreparationPanel = CanvasManager.Instance?.GetPanel<PreparationPanel>();
                }
                return m_PreparationPanel;
            }
        }
        public bool Owned => m_Owned;
        public int Level => m_Level;
        public int Exp => m_Exp;
        public int Star => m_Star;
        public int MaxStar => HeroEvolution.MaxStage;
        public int EvolveCost => HeroEvolution.NextShardCost(m_Star);
        public ItemConfig EvolveShard => m_HeroConfig.EvolutionConfig?.ShardConfig;
        private bool m_ProgressPurchaseInFlight;
        public bool IsUsed => m_IsUsed;

        // =========================
        // PROGRESSION CORE
        // =========================
        public void SetTrial(bool onTrial)
        {
            m_OnTrial = onTrial;
            UnityService.Instance.SaveData(m_HeroConfig.BaseInfo.Id + "OnTrial", m_OnTrial);
        }
        private void ProcessLevelUp()
        {
            int maxLevel = HeroEvolution.MaxLevel;

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
            PreparationPanel?.HeroTabView?.Init();
        }

        // =========================
        // EXP
        // =========================

        public void SetExp(int exp)
        {
            m_Exp = Mathf.Max(0, exp);
            ProcessLevelUp();
            SaveExp();
            m_OnExpUpdate?.Invoke(this);

            Debug.Log($"[EXP] {HeroName} Exp: {m_Exp}, Level: {m_Level}");
        }

        public void AddExp(int exp)
        {
            m_Exp = (int)System.Math.Max(0L, System.Math.Min(int.MaxValue, (long)m_Exp + exp));
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
            if (level <= 0 || m_Level >= HeroEvolution.MaxLevel) return;

            m_Level += Mathf.Min(level, HeroEvolution.MaxLevel - m_Level);

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
            int maxLevel = HeroEvolution.MaxLevel;

            if (m_Level >= maxLevel)
                return;

            if (consumeExp)
            {
                int requiredExp = m_HeroConfig.LevelFormulaConfig.GetCurrentMaxExperience(m_Level);
                if (m_Exp < requiredExp) return;

                m_Exp -= requiredExp;
                SaveExp();
            }

            m_Level++;

            SaveLevel();
            InvokeLevelUpEvents();

            Debug.Log($"[LEVEL UP] {HeroName} → {m_Level}");
        }

        public bool CanEvolve()
        {
            return m_Owned && EvolveCost > 0 && EvolveShard != null &&
                Player.Instance.CurrencyControl.GetCurrencyAmount(EvolveShard) >= EvolveCost;
        }

        public bool TryEvolve()
        {
            if (!m_Owned || EvolveCost <= 0) return false;
            int next = m_Star + 1;
            return TryBuyProgress(EvolveShard, EvolveCost, "Star", next, () => m_Star = next, () =>
            {
                m_OnCharacterStarUp?.Invoke(this);
                Player.Instance.HeroesCollection.OnCharacterStarUp.Invoke(m_HeroConfig);
                PreparationPanel?.HeroTabView?.Init();
            });
        }

        public bool TryUpgradeLevel()
        {
            if (!m_Owned || m_Level >= HeroEvolution.MaxLevel) return false;
            var formula = m_HeroConfig.LevelFormulaConfig;
            int next = m_Level + 1;
            return TryBuyProgress(formula.ItemRequirmentConfig, formula.GetCurrentMaxExperience(m_Level),
                "Lv", next, () => m_Level = next, InvokeLevelUpEvents);
        }

        private bool TryBuyProgress(ItemConfig item, int cost, string suffix, int next,
            System.Action apply, System.Action notify)
        {
            if (m_ProgressPurchaseInFlight || item == null || cost <= 0) return false;
            m_ProgressPurchaseInFlight = true;
            try
            {
                return Player.Instance.CurrencyControl.TrySpendForHeroProgress(item, cost,
                    m_HeroConfig.BaseInfo.Id + suffix, next, apply, notify);
            }
            finally
            {
                m_ProgressPurchaseInFlight = false;
            }
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
            if (UnityService.Instance.HasData(m_HeroConfig.BaseInfo.Id + "OnTrial"))
                m_OnTrial = UnityService.Instance.GetData<bool>(m_HeroConfig.BaseInfo.Id + "OnTrial");

            int savedLevel = m_Level;
            m_Level = Mathf.Clamp(m_Level, 1, HeroEvolution.MaxLevel);
            if (m_Level != savedLevel) SaveLevel();
            m_Star = Mathf.Clamp(m_Star, 0, HeroEvolution.MaxStage);

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
            var bonus = m_HeroConfig.EvolutionConfig?.GetStatBonus(m_Star) ?? StatField.Zero;

            return baseStat + bonus;
        }

        public StatField NextFinalStat()
        {
            var bonus = m_HeroConfig.EvolutionConfig?.GetStatBonus(m_Star) ?? StatField.Zero;
            return m_HeroConfig.MainStats.GetFinalStat(Mathf.Min(Level + 1, HeroEvolution.MaxLevel)) + bonus;
        }
    }
}
