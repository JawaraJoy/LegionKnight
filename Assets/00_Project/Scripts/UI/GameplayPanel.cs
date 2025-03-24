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

    
}
