using UnityEngine;

namespace LegionKnight
{
    public partial class GameplayPanelAgent : MonoBehaviour
    {
        public void Show()
        {
            CanvasManager.Instance.GetPanel<GameplayPanel>().Show();
        }
        public void Hide()
        {
            CanvasManager.Instance.GetPanel<GameplayPanel>().Hide();
        }
    }
}
