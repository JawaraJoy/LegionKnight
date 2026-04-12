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
            EnergyConfirmationView.SetEnergyWarningText(m_WarningText, costs);
            ShowInternal();
        }
    }

    // ─── EnergyConfirmationPanel partial ─────────────────────────────────────
    public partial class EnergyConfirmationPanel
    {
        private EnergyWarningTextView GetEnergyConfirmationWarningView()
            => GetBindingInternal<EnergyWarningTextView>();

        /// <summary>
        /// Buka panel dan tampilkan warning.
        /// Hide confirmation view dulu jika sedang tampil.
        /// </summary>
        public void SetWarningText(Energy[] costs)
        {
            // ✅ Hide confirmation view dulu jika sedang tampil
            GetEnergyConfirmationView()?.Hide();

            ShowInternal();
            GetEnergyConfirmationWarningView().SetWarningText(costs);
        }
    }
}