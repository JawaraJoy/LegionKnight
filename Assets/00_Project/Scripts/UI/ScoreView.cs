using UnityEngine;

namespace LegionKnight
{
    public partial class ScoreView : CurrencyView
    {
        
    }
    public partial class GameplayPanel
    {
        private ScoreView GetScoreView()
        {
            return GetBinding<ScoreView>();
        }
        public void SetScoreView(Currency currency)
        {
            GetScoreView().SetView(currency);
        }
    }
    public partial class GameOverPanel
    {
        private ScoreView GetScoreView()
        {
            return GetBinding<ScoreView>();
        }
        public void SetScoreView(Currency currency)
        {
            GetScoreView().SetView(currency);
        }
    }
    public partial class GameManager
    {
        public void SetScoreView(Currency currency)
        {
            GetPanelInternal<GameplayPanel>().SetScoreView(currency);
            GetPanelInternal<GameOverPanel>().SetScoreView(currency);
        }
    }
    public partial class GameplayPanelAgent
    {
        public void SetScoreView(Currency currency)
        {
            GameManager.Instance.SetScoreView(currency);
        }
    }
}
