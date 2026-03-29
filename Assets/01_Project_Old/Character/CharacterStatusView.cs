using Rush;
using UnityEngine;
using UnityEngine.Events;

namespace LegionKnight
{
    public partial class CharacterStatusView : UIView
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
            StatField finalStat = characterUnit.FinalStat();
            StatField nextFinalStat = characterUnit.NextFinalStat();
            if (isMaxLevel)
            {
                nextFinalStat = finalStat; // If max level, next stat is same as final stat
            }
            m_LevelView.SetCurrentValue(characterUnit.Level);

            m_AttackView.SetCurrentValue(finalStat.Attack);
            m_AttackView.SetNextValue(nextFinalStat.Attack);

            m_DefenseView.SetCurrentValue(finalStat.Defense);
            m_DefenseView.SetNextValue(nextFinalStat.Defense);

            m_HealthView.SetCurrentValue(finalStat.Health);
            m_HealthView.SetNextValue(nextFinalStat.Health);
            //HideNextValueInternal();
        }
        public void ShowNextValue()
        {
            m_AttackView.ShowNextValue();
            m_DefenseView.ShowNextValue();
            m_HealthView.ShowNextValue();

            m_LevelView.ShowNextValue();
        }
        public void HideNextValue()
        {
            HideNextValueInternal();
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
