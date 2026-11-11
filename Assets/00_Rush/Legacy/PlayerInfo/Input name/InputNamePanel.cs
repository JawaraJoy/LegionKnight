using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace LegionKnight
{
    public class InputNamePanel : PanelView
    {
        [SerializeField]
        private TMP_InputField m_InputName;
        [SerializeField]
        private Button m_ConfirmButton;
        [SerializeField]
        private Button m_CancelButton;

        private void Start()
        {
            m_ConfirmButton.onClick.RemoveAllListeners();
            m_CancelButton.onClick.RemoveAllListeners();
            m_ConfirmButton.onClick.AddListener(ConfirmName);
            m_CancelButton.onClick.AddListener(HideInternal);
        }
        private void ConfirmName()
        {
            string newName = m_InputName.text;
            Player.Instance.SetPlayerName(newName);
        }
    }
}
