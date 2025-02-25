using UnityEngine;

namespace LegionKnight
{
    public static partial class PanelId
    {
        public static string GameOverPanelId = "GameOver";
    }
    public partial class GameOverPanel : PanelView
    {
        [SerializeField]
        private string m_GameplaySceneName = "ReworkGameplay";
        public override string UniqueId => PanelId.GameOverPanelId;

        public void PlayAgain()
        {
            //GameManager.Instance.LoadScene(m_GameplaySceneName);
            GameManager.Instance.Play();
        }
        protected override void OnShowInvoke()
        {
            base.OnShowInvoke();
            Time.timeScale = 0f;
        }
        protected override void OnHideInvoke()
        {
            base.OnHideInvoke();
            Time.timeScale = 1;
            Currency coin = GameManager.Instance.CurrentCoinReward;
            Player.Instance.AddCurrencyAmount(coin.CurrencyDefinition, coin.Amount);
        }
    }
}
