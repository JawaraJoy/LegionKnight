using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace LegionKnight
{
    public class CopyButtons : UIView
    {
        [SerializeField] 
        private TextMeshProUGUI m_TextToCopy;
        [SerializeField] 
        private Button m_CopyButton;

        private TextPopUpPanel m_PopUpPanel;
        private TextPopUpPanel GetPopUpPanel()
        {
            if (m_PopUpPanel == null)
            {
                m_PopUpPanel = CanvasManager.Instance.GetPanel<TextPopUpPanel>();
            }
            return m_PopUpPanel;
        }

        private void Awake()
        {
            m_CopyButton.onClick.AddListener(() =>
            {
                CopyText();
            });
        }
        private void CopyText()
        {
            GUIUtility.systemCopyBuffer = m_TextToCopy.text;
            GetPopUpPanel().ShowText("Success copied the content");
        }
    }
}
