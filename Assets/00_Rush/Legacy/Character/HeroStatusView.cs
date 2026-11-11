using Rush;
using UnityEngine;
using UnityEngine.Events;

namespace LegionKnight
{
    public partial class HeroStatusView : UIView
    {
        [SerializeField]
        private StatView m_AttackView;
        [SerializeField]
        private StatView m_DefenseView;
        [SerializeField]
        private StatView m_HealthView;

        [SerializeField]
        private StatView m_LevelView;

        [SerializeField]
        private UpgradeButton m_UpgradeButton;
        [SerializeField]
        private UpgradeView m_UpgradeView;
        [SerializeField]
        private BreakThroughButton m_BreakButton;
        [SerializeField]
        private BreakThroughView m_BreakView;

        [SerializeField]
        private UnityEvent m_OnBreakAvaiable = new();
        [SerializeField]
        private UnityEvent m_OnBreakUnavailable = new();

        private void Start()
        {
            /*m_UpgradeView.OnShow.RemoveListener(ShowNextValueInternal);
            m_UpgradeView.OnHide.RemoveListener(HideNextValueInternal);

            m_BreakView.OnShow.RemoveListener(ShowNextValueInternal);
            m_BreakView.OnHide.RemoveListener(HideNextValueInternal);

            m_UpgradeView.OnShow.AddListener(ShowNextValueInternal);
            m_UpgradeView.OnHide.AddListener(HideNextValueInternal);

            m_BreakView.OnShow.AddListener(ShowNextValueInternal);
            m_BreakView.OnHide.AddListener(HideNextValueInternal);*/

            HideNextValueInternal();
        }

        public void Init(HeroUnitConfig heroConfig)
        {
            HeroUnit characterUnit = Player.Instance.HeroesCollection.GetHeroUnit(heroConfig);

            bool isTimeToBreak = heroConfig.BreakThroughFormulaConfig.CanBreakByLevel(characterUnit.Star, characterUnit.Level);
            //bool canBreak = Player.Instance.GetCurrencyAmount(breakShardDefi) >= breakShardAmount && 

            if (isTimeToBreak)
            {
                m_OnBreakAvaiable.Invoke();
                m_BreakButton.Init(heroConfig);
                m_BreakButton.Show();
                m_UpgradeButton.Hide();
                m_UpgradeView.Hide();
                m_LevelView.SetNextValue(characterUnit.Level);
            }
            else
            {
                m_OnBreakUnavailable.Invoke();
                m_UpgradeButton.Init(heroConfig);
                m_UpgradeButton.Show();
                m_BreakButton.Hide();
                m_BreakView.Hide();
                m_LevelView.SetNextValue(characterUnit.Level + 1);
            }

            bool isMaxLevel = characterUnit.Level >= heroConfig.Progression.MaxLevel;
            BreakThroughStep currentBreakThroughStep = heroConfig.BreakThroughFormulaConfig.BreakThroughSteps[characterUnit.Star];

            StatField bonusStat = currentBreakThroughStep.StatBonus * characterUnit.Star; // Assuming base stat is determined by breakthrough step and star
            StatField finalStat = characterUnit.FinalStat();
            StatField nextFinalStat = characterUnit.NextFinalStat();
            if (isMaxLevel)
            {   
                nextFinalStat = finalStat; // If max level, next stat is same as final stat
            }
            m_LevelView.SetCurrentValue(characterUnit.Level);

            m_AttackView.SetCurrentValue(finalStat.Attack + bonusStat.Attack);
            m_AttackView.SetNextValue(nextFinalStat.Attack + bonusStat.Attack);

            m_DefenseView.SetCurrentValue(finalStat.Defense + bonusStat.Defense);
            m_DefenseView.SetNextValue(nextFinalStat.Defense + bonusStat.Defense);

            m_HealthView.SetCurrentValue(finalStat.Health + bonusStat.Health);
            m_HealthView.SetNextValue(nextFinalStat.Health + bonusStat.Health);
            //HideNextValueInternal();
        }
        public void ShowNextValue()
        {
            ShowNextValueInternal();
        }
        public void HideNextValue()
        {
            HideNextValueInternal();
        }

        private void ShowNextValueInternal()
        {
            m_AttackView.ShowNextValue();
            m_DefenseView.ShowNextValue();
            m_HealthView.ShowNextValue();
            m_LevelView.ShowNextValue();
        }
        private void HideNextValueInternal()
        {
            m_AttackView.HideNextValue();
            m_DefenseView.HideNextValue();
            m_HealthView.HideNextValue();
            m_LevelView.HideNextValue();
        }
    }
}
