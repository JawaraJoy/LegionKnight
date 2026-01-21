using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace LegionKnight
{
    public class PopUpView : UIView
    {
        [SerializeField]
        private TextMeshProUGUI m_TextPopUp;
        [SerializeField]
        private Image m_Icon;

        public void ShowPopUp(ScriptableObject so)
        {
            if (so is IPopUpInfo popUp)
            {
                m_TextPopUp.text = popUp.Info;
                m_Icon.sprite = popUp.Icon;
            }
            ShowInternal();
        }
    }
}
