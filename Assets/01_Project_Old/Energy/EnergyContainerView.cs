using UnityEngine;

namespace LegionKnight
{
    public class EnergyContainerView : UIView
    {
        [SerializeField]
        private EnergyView[] m_EnergyViews;

        private EnergyView GetEnergyView(EnergyDefinition defi)
        {
            EnergyView view = null;
            foreach(EnergyView energyView in m_EnergyViews)
            {
                if (energyView.Definition == defi)
                {
                    view = energyView;
                }
            }
            if (view == null)
            {
                Debug.LogError($"No Energy View is found with {defi.Label} on the array");
            }
            return view;
        }

        public void SetEnergyView(Energy energy)
        {
            if (GetEnergyView(energy.Definition) != null)
            {
                GetEnergyView(energy.Definition).SetEnergy(energy);
            }
        }
    }
    public partial class HomePanel
    {
        private EnergyContainerView GetEnergyContainerView()
        {
            return GetBindingInternal<EnergyContainerView>();
        }

        public void SetEnergyView(Energy energy)
        {
            GetEnergyContainerView().SetEnergyView(energy);
        }
    }

    public partial class CanvasManager
    {
        public void SetEnergyView(Energy energy)
        {
            HomePanel homePanel = GetPanelInternal<HomePanel>();
            homePanel.SetEnergyView(energy);
        }
    }
}
