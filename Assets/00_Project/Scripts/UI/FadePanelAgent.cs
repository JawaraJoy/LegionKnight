using UnityEngine;

namespace LegionKnight
{
    public class FadePanelAgent : MonoBehaviour
    {
        private FadePanel GetFadePanel()
        {
            return GameManager.Instance.GetPanel<FadePanel>();
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
