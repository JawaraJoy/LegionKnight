using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace LegionKnight
{
    public class EnergyConfirmationView : UIView
    {
        [SerializeField]
        private TextMeshProUGUI m_ConfirmationText;
        [SerializeField]
        private Button m_ConfirmButton;
        [SerializeField]
        private Button m_CancelButton;

        public static string EnergyCostTextStart = $"<sprite=";
        public static string EnergyCostTextEnd = ">";
        public static string ConfirmationInfoTextStart = $"Spend ";
        public static string ConfirmationInfoTextEnd = $" to play this Level?";
        public static string WarningInfoTextStart = $"You need at less get ";
        public static string WarningInfoTextEnd = $" to Continue";

        private static string TotalCostText;

        // ✅ Reset setiap kali SetConfirmationText dipanggil
        private Energy[] m_Costs;

        [SerializeField]
        private UnityEvent<Energy[]> m_OnTryPay;
        [SerializeField]
        private UnityEvent<Energy[]> m_OnCanPay;
        [SerializeField]
        private UnityEvent<Energy[]> m_OnCantPay;

        private static string GetTotalCostText(Energy[] costs)
        {
            TotalCostText = "";
            for (int i = 0; i < costs.Length; i++)
            {
                string costSpriteText = $"{costs[i].Amount} {EnergyCostTextStart}{i}{EnergyCostTextEnd}";
                TotalCostText += costSpriteText;
            }
            return TotalCostText;
        }

        public static void SetEnergyConfirmationText(TextMeshProUGUI textMesh, Energy[] costs)
        {
            textMesh.text = $"{ConfirmationInfoTextStart} {GetTotalCostText(costs)} {ConfirmationInfoTextEnd}";
        }

        public static void SetEnergyWarningText(TextMeshProUGUI textMesh, Energy[] costs)
        {
            textMesh.text = $"{WarningInfoTextStart} {GetTotalCostText(costs)} {WarningInfoTextEnd}";
        }

        /// <summary>
        /// Setup ulang view dengan cost baru.
        /// Selalu clear m_Costs dan listeners lama sebelum assign yang baru,
        /// sehingga tidak ada sisa cost dari stage sebelumnya.
        /// </summary>
        public void SetConfirmationText(Energy[] costs, UnityAction<Energy[]> onCanPayListen, UnityAction<Energy[]> onCantPayListen)
        {
            // ✅ Reset cost lama sebelum assign baru
            m_Costs = null;
            m_Costs = costs;

            // ✅ Update teks sesuai cost baru
            SetEnergyConfirmationText(m_ConfirmationText, m_Costs);

            // ✅ Clear semua listener lama sebelum assign baru
            m_OnCanPay.RemoveAllListeners();
            m_OnCantPay.RemoveAllListeners();
            m_OnCanPay.AddListener(onCanPayListen);
            m_OnCantPay.AddListener(onCantPayListen);

            // ✅ Wire CancelButton untuk hide panel
            m_CancelButton.onClick.RemoveAllListeners();
            m_CancelButton.onClick.AddListener(HideInternal);

            ShowInternal();
        }

        /// <summary>
        /// Dipanggil oleh m_ConfirmButton via Inspector.
        /// Pakai m_Costs yang sudah di-set di SetConfirmationText() — dijamin fresh.
        /// </summary>
        public void Pay()
        {
            if (m_Costs == null || m_Costs.Length == 0)
            {
                Debug.LogWarning("[EnergyConfirmationView] Pay() dipanggil tapi m_Costs null/kosong.");
                return;
            }

            Player.Instance.EnergyController.Pay(m_Costs, OnCanPayInvoke, OnCantPayInvoke);
        }

        private void OnCanPayInvoke(Energy[] costs)
        {
            m_OnCanPay.Invoke(costs);
            Player.Instance.EnergyController.OnCanPay.Invoke(costs);
            HideInternal(); // ✅ Tutup view setelah bayar berhasil
        }

        private void OnCantPayInvoke(Energy[] costRest)
        {
            m_OnCantPay.Invoke(costRest);
            Player.Instance.EnergyController.OnCantPay.Invoke(costRest);
        }
    }

    // ─── EnergyConfirmationPanel partial ─────────────────────────────────────
    public partial class EnergyConfirmationPanel
    {
        private EnergyConfirmationView GetEnergyConfirmationView()
            => GetBindingInternal<EnergyConfirmationView>();

        /// <summary>
        /// Buka panel dan setup cost baru.
        /// EnergyConfirmationView akan clear cost lama sebelum assign baru.
        /// </summary>
        public void SetConfirmationText(Energy[] costs, UnityAction<Energy[]> onCanPayListen, UnityAction<Energy[]> onCantPayListen)
        {
            // ✅ Hide warning view dulu jika sedang tampil
            GetEnergyConfirmationWarningView()?.Hide();

            ShowInternal();
            GetEnergyConfirmationView().SetConfirmationText(costs, onCanPayListen, onCantPayListen);
        }
    }
}