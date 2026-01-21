using TMPro;
using UnityEngine;

namespace LegionKnight
{
    public class EnergyWarningTextView : UIView
    {
        [SerializeField]
        private TextMeshProUGUI m_WarningText;

        public void SetWarningText(Energy[] costs)
        {
            ShowInternal();
            EnergyConfirmationView.SetEnergyWarningText(m_WarningText, costs);
        }
    }

    public partial class EnergyConfirmationPanel
    {
        private EnergyWarningTextView GetEnergyConfirmationWarningView()
        {
            return GetBindingInternal<EnergyWarningTextView>();
        }

        public void SetWarningText(Energy[] costs)
        {
            ShowInternal();
            GetEnergyConfirmationWarningView().SetWarningText(costs);
        }
    }
}
