using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace LegionKnight
{
    public class EnergyView : UIView
    {
        [SerializeField]
        private Image m_EnergyIcon;
        [SerializeField]
        private TextMeshProUGUI m_EnergyAmountText;
        [SerializeField]
        private Slider m_Slider;

        [SerializeField]
        private EnergyDefinition m_Definition;
        public EnergyDefinition Definition => m_Definition;
        protected override void ShowInternal()
        {
            base.ShowInternal();
            Energy energy = Player.Instance.GetEnergy(m_Definition);
            SetEnergy(energy);
        }
        public void SetEnergy(Energy energy)
        {
            SetEnergyInternal(energy);
        }
        public void SetEnergyInternal(Energy energy)
        {
            m_EnergyIcon.sprite = energy.Definition.Icon;
            int currentAmount = energy.Amount;
            int maxAmount = energy.Definition.MaxAmount;
            m_EnergyAmountText.text = $"{currentAmount}/{maxAmount}";
            float rateVal = (float)currentAmount / (float)maxAmount;
            m_Slider.value = rateVal;
        }
    }
}
