using UnityEngine;

namespace LegionKnight
{
    public partial class GameplayPanelAgent : MonoBehaviour
    {
        public void Show()
        {
            CanvasManager.Instance.ShowPanel(PanelId.GameplayPanelId);
        }
        public void Hide()
        {
            CanvasManager.Instance.HidePanel(PanelId.GameplayPanelId);
        }
    }
}
