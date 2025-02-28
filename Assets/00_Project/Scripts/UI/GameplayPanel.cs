using UnityEngine;

namespace LegionKnight
{
    public static partial class PanelId
    {
        public static string GameplayPanelId = "Gameplay";
    }
    public partial class GameplayPanel : PanelView
    {
        public override string UniqueId => PanelId.GameplayPanelId;
    }

    public partial class GameManager
    {
        public void SetCurrencyRewardView(Currency currency)
        {
            GetPanelInternal<GameplayPanel>().SetCurrencyReward(currency);
        }
    }
}
