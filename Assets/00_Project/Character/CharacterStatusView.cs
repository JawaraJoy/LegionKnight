using UnityEngine;

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

        public void Init(CharacterDefinition definition)
        {
            CharacterUnit characterUnit = Player.Instance.GetCharacterUnit(definition);
            Stat finalStat = characterUnit.FinalStat();
            Stat nextFinalStat = characterUnit.NextFinalStat();

            m_LevelView.SetCurrentValue(characterUnit.Level);
            m_LevelView.SetNextValue(characterUnit.Level + 1);

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
