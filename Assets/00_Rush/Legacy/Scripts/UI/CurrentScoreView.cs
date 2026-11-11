using Rush;
using UnityEngine;

namespace LegionKnight
{
    public partial class CurrentScoreView : CurrencyView
    {
        private void Refresh(StageConfig config)
        {
            if (config.StageMode == StageMode.Collosal)
            {
                ShowInternal();
            }
            else
            {
                HideInternal();
            }
        }
        private void Awake()
        {
            RushGameManager.Instance.StageManager.OnSelectStage.AddListener(Refresh);
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
