using UnityEngine;

namespace LegionKnight
{
    public static partial class PanelId
    {
        public static string CharacterPanelId = "Character";
    }
    public partial class CharacterPanel : PanelView
    {
        public override string UniqueId => PanelId.CharacterPanelId;
    }
}
