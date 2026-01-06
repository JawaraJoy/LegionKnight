using UnityEngine;

namespace LegionKnight
{
    public class TextPopUpPanelAgent : MonoBehaviour
    {
        private TextPopUpPanel m_Panel;
        private TextPopUpPanel GetPanel()
        {
            if (m_Panel == null)
            {
                m_Panel = CanvasManager.Instance.GetPanel<TextPopUpPanel>();
            }
            return m_Panel;
        }
        public void ShowText(string text)
        {
            GetPanel().ShowText(text);
        }
    }
}
