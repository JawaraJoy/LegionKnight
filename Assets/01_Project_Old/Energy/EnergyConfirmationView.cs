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
        public void SetConfirmationText(Energy[] costs, UnityAction<Energy[]> onCanPayListen, UnityAction<Energy[]> onCantPayListen)
        {
            ShowInternal();
            m_Costs = costs;
            SetEnergyConfirmationText(m_ConfirmationText, costs);
            m_OnCanPay.RemoveAllListeners();
            m_OnCantPay.RemoveAllListeners();

            m_OnCanPay.AddListener(onCanPayListen);
            m_OnCantPay.AddListener(onCantPayListen);
        }

        public void Pay()
        {
            Player.Instance.PayEnergies(m_Costs, OnCanPayInvoke, OnCantPayInvoke);
        }
        private void OnCanPayInvoke(Energy[] costs)
        {
            m_OnCanPay.Invoke(costs);
            Player.Instance.OnCanPayEnergy.Invoke(costs);
        }

        private void OnCantPayInvoke(Energy[] costRest)
        {
            m_OnCantPay.Invoke(costRest);
            Player.Instance.OnCantPayEnergy.Invoke(costRest);
        }
    }

    public partial class EnergyConfirmationPanel
    {
        private EnergyConfirmationView GetEnergyConfirmationView()
        {
            return GetBindingInternal<EnergyConfirmationView>();
        }

        public void SetConfirmationText(Energy[] costs, UnityAction<Energy[]> onCanPayListen, UnityAction<Energy[]> onCantPayListen)
        {
            ShowInternal();
            GetEnergyConfirmationView().SetConfirmationText(costs, onCanPayListen, onCantPayListen);
        }
    }
}
