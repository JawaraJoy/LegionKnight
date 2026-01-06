using UnityEngine;

namespace LegionKnight
{
    public class FadePanelAgent : MonoBehaviour
    {
        private FadePanel GetFadePanel()
        {
            return CanvasManager.Instance.GetPanel<FadePanel>();
        }
        public void Show()
        {
            var fadePanel = GetFadePanel();
            if (fadePanel != null)
            {
                fadePanel.Show();
            }
        }
        public void Hide()
        {
            var fadePanel = GetFadePanel();
            if (fadePanel != null)
            {
                fadePanel.Hide();
            }
        }
    }
}
