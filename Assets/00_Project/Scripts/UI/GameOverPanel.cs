using UnityEngine;

namespace LegionKnight
{
    public static partial class PanelId
    {
        public static string GameOverPanelId = "GameOver";
    }
    public partial class GameOverPanel : PanelView
    {
        public override string UniqueId => PanelId.GameOverPanelId;

        public void PlayAgain()
        {
            LevelManager.Instance.Play();
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
            Currency coin = LevelManager.Instance.CurrentCoinReward;
            Player.Instance.AddCurrencyAmount(coin.CurrencyDefinition, coin.Amount);
        }
    }
}
