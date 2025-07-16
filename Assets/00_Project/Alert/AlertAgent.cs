using UnityEngine;

namespace LegionKnight
{
    public class AlertAgent : MonoBehaviour
    {
        private AlertPanel GetAlertPanel()
        {
            AlertPanel alertPanel = GameManager.Instance.GetPanel<AlertPanel>();
            return alertPanel;
        }

        public void ShowAlert(string message)
        {
            AlertPanel alertPanel = GetAlertPanel();
            if (alertPanel != null)
            {
                alertPanel.ShowAlert(message);
            }
            else
            {
                Debug.LogError("AlertPanel not found!");
            }
        }
    }
}
