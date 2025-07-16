using TMPro;
using UnityEngine;

namespace LegionKnight
{
    public static partial class PanelId
    {
        public const string AlertPanel = "Alert";
    }
    public class AlertPanel : PanelView
    {
        public override string UniqueId => PanelId.AlertPanel;

        [SerializeField]
        private TextMeshProUGUI m_AlertText;

        public void ShowAlert(string message)
        {
            ShowInternal();
            m_AlertText.text = $"! {message} !";
        }
    }
}
