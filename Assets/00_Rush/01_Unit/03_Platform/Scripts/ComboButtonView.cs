using LegionKnight;
using UnityEngine;
using UnityEngine.UI;

namespace Rush
{
    public class ComboButtonView : UIView
    {
        [SerializeField]
        private Button m_ComboButton;
        public Button ComboButton => m_ComboButton;
    }
}