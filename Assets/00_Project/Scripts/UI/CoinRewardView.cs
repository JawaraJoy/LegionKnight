using UnityEngine;

namespace LegionKnight
{
    public partial class CoinRewardView : CurrencyView
    {
        
    }
    public partial class GameplayPanel
    {
        private CoinRewardView GetCurrencyView()
        {
            return GetBinding<CoinRewardView>();
        }
        public void SetCoinReward(Currency currency)
        {
            GetCurrencyView().SetView(currency);
        }
    }
    public partial class GameOverPanel
    {
        private CoinRewardView GetCurrencyView()
        {
            return GetBinding<CoinRewardView>();
        }
        public void SetCoinReward(Currency currency)
        {
            GetCurrencyView().SetView(currency);
        }
    }
    public partial class GameManager
    {
        public void SetCoinRewardView(Currency currency)
        {
            GetPanelInternal<GameplayPanel>().SetCoinReward(currency);
            GetPanelInternal<GameOverPanel>().SetCoinReward(currency);
        }
    }
    public partial class GameplayPanelAgent
    {
        public void SetCoinRewardView(Currency currency)
        {
            GameManager.Instance.SetCoinRewardView(currency);
        }
    }
}
