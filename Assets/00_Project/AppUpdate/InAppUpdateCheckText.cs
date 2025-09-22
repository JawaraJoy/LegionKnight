using TMPro;
using UnityEngine;

namespace LegionKnight
{
    public class InAppUpdateCheckText : UIView
    {
        [SerializeField]
        private TextMeshProUGUI m_UpdateCheckText;

        public void SetCheckText(string text)
        {
            m_UpdateCheckText.text = text;
        }
    }
}
