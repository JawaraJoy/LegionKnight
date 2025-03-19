using UnityEngine;

namespace LegionKnight
{
    public static partial class PanelId
    {
        public static string CharacterPanelId = "CharacterPanel";
    }
    public partial class CharacterPanel : PanelView
    {
        public override string UniqueId => PanelId.CharacterPanelId;
    }
}
