using UnityEngine;
using UnityEngine.Events;

namespace LegionKnight
{
    public class PlayerEnergyViewAgent : MonoBehaviour
    {
        private EnergyConfirmationPanel GetEnergyConfirmationPanel()
        {
            return GameManager.Instance.GetPanel<EnergyConfirmationPanel>();
        }
        public void SetEnergyView(Energy energy)
        {
            GameManager.Instance.SetEnergyView(energy);
        }
        public void SetConfirmationText(Energy[] costs, UnityAction<Energy[]> onCanPayListen, UnityAction<Energy[]> onCantPayListen)
        {
            GetEnergyConfirmationPanel().SetConfirmationText(costs, onCanPayListen, onCantPayListen);
        }
        public void SetWarningText(Energy[] costs)
        {
            GetEnergyConfirmationPanel().SetWarningText(costs);
        }
        public void SetConfirmationEnergyView(Energy energy)
        {
            GetEnergyConfirmationPanel().SetEnergy(energy);
        }
    }
}
