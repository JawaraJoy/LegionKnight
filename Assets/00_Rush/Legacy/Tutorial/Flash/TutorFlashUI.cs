using System.Collections;
using TMPro;
using UnityEngine;

namespace LegionKnight
{
    public class TutorFlashUI : UIView
    {
        [SerializeField]
        private TextMeshProUGUI m_FlashText;

        public void SetFlashText(string message)
        {
            m_FlashText.text = message;
        }
        public void ShowFlash(string message)
        {
            m_FlashText.text = message;
            ShowInternal();
        }
    }
}
