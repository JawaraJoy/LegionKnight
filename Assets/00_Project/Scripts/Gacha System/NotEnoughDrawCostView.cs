using UnityEngine;
using UnityEngine.UI;

namespace LegionKnight
{
    public partial class NotEnoughDrawCostView : UIView
    {
        private CurrencyDefinition m_Definition;
        [SerializeField]
        private Image m_Icon;
        public void SetShow(CurrencyDefinition definition)
        {
            m_Definition = definition;
            ShowInternal();
            m_Icon.sprite = m_Definition.Icon;
        }
    }
}
