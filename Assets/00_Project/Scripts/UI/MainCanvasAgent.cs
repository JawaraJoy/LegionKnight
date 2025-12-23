using UnityEngine;

namespace LegionKnight
{
    public class MainCanvasAgent : MonoBehaviour
    {
        private PanelView GetPanelView(string panelName)
        {
            PanelView panel = GameManager.Instance.GetPanel(panelName);
            if (panel == null)
            {
                Debug.LogError($"Panel with name {panelName} not found.");
            }
            return panel;
        }

        public void ShowPanel(string panelName)
        {
            Debug.Log("aaaaaaaaaaaaaaaa");
            Debug.Log(panelName);
            

            PanelView panel = GetPanelView(panelName);
            if (panel != null)
            {
                panel.Show();
            }
        }

        public void HidePanel(string panelName)
        {
            PanelView panel = GetPanelView(panelName);
            if (panel != null)
            {
                panel.Hide();
            }
        }
    }
}
