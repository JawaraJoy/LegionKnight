using Rush;
using UnityEngine;

namespace LegionKnight
{
    public partial class CurrentScoreView : CurrencyView
    {
        private PlayerScore m_PlayerScore;

        private PlayerScore PlayerScore
        {
            get
            {
                if (m_PlayerScore == null)
                {
                    m_PlayerScore = RushPlayer.Instance.PlayerScore;
                }
                return m_PlayerScore;
            }
        }
        private void Awake()
        {
            PlayerScore.OnScoreCurrencyChanged.AddListener(SetViewInternal);
        }
    }
    public partial class GameplayPanel
    {
        private CurrentScoreView GetScoreView()
        {
            return GetBinding<CurrentScoreView>();
        }
        public void SetScoreView(Currency currency)
        {
            GetScoreView().SetView(currency);
        }
    }
    public partial class GameOverPanel
    {
        private CurrentScoreView GetScoreView()
        {
            return GetBinding<CurrentScoreView>();
        }
        public void SetScoreView(Currency currency)
        {
            GetScoreView().SetView(currency);
        }
    }
    public partial class CanvasManager
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
            CanvasManager.Instance.SetScoreView(currency);
        }
    }
}
