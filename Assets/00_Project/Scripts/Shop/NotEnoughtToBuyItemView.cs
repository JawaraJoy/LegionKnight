using UnityEngine;
using UnityEngine.UI;

namespace LegionKnight
{
    public partial class NotEnoughtToBuyItemView : UIView
    {
        private ShopItemDefinition m_Definition;
        [SerializeField]
        private Image m_Icon;
        public void SetShow(ShopItemDefinition definition)
        {
            m_Definition = definition;
            ShowInternal();
            m_Icon.sprite = m_Definition.Currency.Icon;
        }
            
        
    }

}
