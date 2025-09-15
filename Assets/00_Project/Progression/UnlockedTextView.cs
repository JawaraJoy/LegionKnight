using TMPro;
using UnityEngine;

namespace LegionKnight
{
    public partial class UnlockedTextView : UIView
    {
        [SerializeField]
        private TextMeshProUGUI m_Text;

        public void ShowText(string text)
        {
            ShowInternal();
            m_Text.text = text;
        }
    }
}
