using UnityEngine;

namespace LegionKnight
{
    public partial class GameplayPanelAgent : MonoBehaviour
    {
        public void Show()
        {
            GameManager.Instance.ShowPanel(PanelId.GameplayPanelId);
        }
        public void Hide()
        {
            GameManager.Instance.HidePanel(PanelId.GameplayPanelId);
        }
    }
}
