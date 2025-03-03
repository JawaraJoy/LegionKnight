using UnityEngine;

namespace LegionKnight
{
    public partial class HighScoreView : CurrencyView
    {
        
    }

    public partial class HomePanel
    {
        private HighScoreView GetHighScoreView()
        {
            return GetBinding<HighScoreView>();
        }

        public void SetHighScoreView(Currency currency)
        {
            GetHighScoreView().SetView(currency);
        }
    }
    public partial class GameManager
    {
        public void SetHomeHighScoreView(Currency currency)
        {
            GetPanelInternal<HomePanel>().SetHighScoreView(currency);
        }
    }
}
