using Rush;
using UnityEngine;

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

        private HeroUnitConfig m_ShownHero;

        private void OnEnable()
        {
            m_UpgradeView.OnShow.AddListener(m_BreakView.Hide);
            m_BreakView.OnShow.AddListener(m_UpgradeView.Hide);
        }

        private void OnDisable()
        {
            m_UpgradeView.OnShow.RemoveListener(m_BreakView.Hide);
            m_BreakView.OnShow.RemoveListener(m_UpgradeView.Hide);
        }

        private void Start() => HideNextValueInternal();

        public void Init(HeroUnitConfig heroConfig)
        {
            var hero = Player.Instance.HeroesCollection.GetHeroUnit(heroConfig);
            if (m_ShownHero != heroConfig)
            {
                m_UpgradeView.Hide();
                m_BreakView.Hide();
                m_ShownHero = heroConfig;
            }
            if (!m_UpgradeButton.IsShown) m_UpgradeButton.Show();
            if (!m_BreakButton.IsShown) m_BreakButton.Show();
            m_UpgradeButton.Init(heroConfig);
            m_BreakButton.Init(heroConfig);

            var current = hero.FinalStat();
            var next = hero.NextFinalStat();
            m_LevelView.SetCurrentValue(hero.Level);
            m_LevelView.SetNextValue(Mathf.Min(hero.Level + 1, HeroEvolution.MaxLevel));
            m_AttackView.SetCurrentValue(current.Attack);
            m_AttackView.SetNextValue(next.Attack);
            m_DefenseView.SetCurrentValue(current.Defense);
            m_DefenseView.SetNextValue(next.Defense);
            m_HealthView.SetCurrentValue(current.Health);
            m_HealthView.SetNextValue(next.Health);
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

