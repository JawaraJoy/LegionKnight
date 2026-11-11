using UnityEngine;

namespace LegionKnight
{
    public partial class EnergyConfirmationPanel : PanelView
    {
        public void SetEnergy(Energy energy)
        {
            GetBinding<EnergyView>().SetEnergy(energy);
        }
    }
}
