using UnityEngine;

namespace LegionKnight
{
    public class WinPanelAgent : MonoBehaviour
    {
        public void OpenWinPanel()
        {
            if (!GameManager.Instance.IsInfiniteLevel)
            {
                WinPanel winPanel = CanvasManager.Instance.GetPanel<WinPanel>();
                winPanel.Show();
            }
        }
    }
}
