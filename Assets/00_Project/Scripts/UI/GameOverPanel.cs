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
            GameTimeScale.SetTimeScale(0);
        }
        protected override void OnHideInvoke()
        {
            base.OnHideInvoke();
            GameTimeScale.SetTimeScale(1);
            Currency coin = GameManager.Instance.CurrentCoinReward;
            Player.Instance.AddCurrencyAmount(coin.CurrencyDefinition, coin.Amount);
            GameManager.Instance.SetRewardAmount(0);
        }
    }
}
