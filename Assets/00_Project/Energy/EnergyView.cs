using UnityEngine;
using UnityEngine.UI;

namespace LegionKnight
{
    public class EnergyView : UIView
    {
        [SerializeField]
        private Image m_EnergyIcon;
        [SerializeField]
        private Text m_EnergyAmountText;
        public void SetEnergy(Energy energy)
        {
            m_EnergyIcon.sprite = energy.Definition.Icon;
            int currentAmount = energy.Amount;
            int maxAmount = energy.Definition.MaxAmount;
            m_EnergyAmountText.text = $"{currentAmount}/{maxAmount}";
        }
    }
}
