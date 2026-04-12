using Rush;
using UnityEngine;
using UnityEngine.UI;

namespace LegionKnight
{
    public partial class PausePanel : PanelView
    {
        [SerializeField]
        private Button m_ContinueButton;
        [SerializeField]
        private Button m_RestartButton;
        [SerializeField]
        private Button m_HomeButton;

        [SerializeField]
        private GameStateConfig m_HomeState;
        [SerializeField]
        private PreviousEnergyCost m_PreviousEnergyCost;
        private void Start()
        {
            m_ContinueButton.onClick.AddListener(ContinueButtonClick);
            m_RestartButton.onClick.AddListener(RestartButtonClick);
            m_HomeButton.onClick.AddListener(HomeButtonClick);
        }

        private void ContinueButtonClick()
        {
            HideInternal();
        }
        private void HomeButtonClick()
        {
            HideInternal();
            RushGameManager.Instance.GameStateManager.ChangeState(m_HomeState);
        }
        private void RestartButtonClick()
        {
            m_PreviousEnergyCost.TryPay();
            //RushGameManager.Instance.GameStateManager.ChangeState(m_GameState);
        }
        protected override void HideInternal()
        {
            base.HideInternal();
            //Player.Instance.SetPause(false);
            GameTimeScale.SetTimeScale(1f);
            
        }
        protected override void ShowInternal()
        {
            base.ShowInternal();
            //Player.Instance.SetPause(true);
            GameTimeScale.SetTimeScale(0f);
        }
    }
}
