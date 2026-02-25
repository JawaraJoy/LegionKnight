using Rush;
using UnityEngine;
using UnityEngine.UI;

namespace LegionKnight
{
    public partial class NotEnoughDrawCostView : UIView
    {
        private ItemConfig m_ItemConfig;
        [SerializeField]
        private Image m_Icon;
        public void SetShow(ItemConfig definition)
        {
            m_ItemConfig = definition;
            ShowInternal();
            m_Icon.sprite = m_ItemConfig.CollectibleField.Icon;
        }
    }
}
