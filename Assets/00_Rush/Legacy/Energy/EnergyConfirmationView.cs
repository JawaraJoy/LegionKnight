using System;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace LegionKnight
{
    public class EnergyConfirmationView : UIView
    {
        [SerializeField] private TextMeshProUGUI m_ConfirmationText;
        [SerializeField] private Button m_ConfirmButton;
        [SerializeField] private Button m_CancelButton;
        [SerializeField] private UnityEvent<Energy[]> m_OnTryPay;
        [SerializeField] private UnityEvent<Energy[]> m_OnCanPay;
        [SerializeField] private UnityEvent<Energy[]> m_OnCantPay;
        private Energy[] m_Costs;
        private Func<bool> m_CanConfirm;

        private static string FormatCosts(Energy[] costs, string qualifier = "")
        {
            return string.Join(", ", costs.Select(cost => $"{cost.Amount} {qualifier}{cost.Config.BaseInfo.Name}"));
        }

        public static void SetEnergyConfirmationText(TextMeshProUGUI textMesh, Energy[] costs)
        {
            textMesh.text = $"Spend {FormatCosts(costs)} to play?";
        }

        public static void SetEnergyWarningText(TextMeshProUGUI textMesh, Energy[] costs)
        {
            textMesh.text = $"You need {FormatCosts(costs, "more ")} to play.";
        }

        public void SetConfirmationText(Energy[] costs, UnityAction<Energy[]> onCanPayListen,
            UnityAction<Energy[]> onCantPayListen, Func<bool> canConfirm = null)
        {
            m_Costs = costs;
            m_CanConfirm = canConfirm;
            SetEnergyConfirmationText(m_ConfirmationText, costs);
            m_OnCanPay.RemoveAllListeners();
            m_OnCantPay.RemoveAllListeners();
            if (onCanPayListen != null) m_OnCanPay.AddListener(onCanPayListen);
            if (onCantPayListen != null) m_OnCantPay.AddListener(onCantPayListen);
            ShowInternal();
        }

        protected override void HideInternal()
        {
            m_Costs = null;
            m_CanConfirm = null;
            base.HideInternal();
        }

        public void Pay()
        {
            if (!isActiveAndEnabled || !IsShown || m_Costs == null || m_Costs.Length == 0) return;
            if (m_CanConfirm != null && !m_CanConfirm())
            {
                GetComponentInParent<EnergyConfirmationPanel>().Hide();
                return;
            }

            // Consume the confirmation before any listener can submit it again.
            Energy[] costs = m_Costs;
            m_Costs = null;
            m_CanConfirm = null;
            Player.Instance.EnergyController.Pay(costs, m_OnCanPay.Invoke, m_OnCantPay.Invoke);
        }
    }

    public partial class EnergyConfirmationPanel
    {
        private EnergyConfirmationView GetEnergyConfirmationView() => GetBindingInternal<EnergyConfirmationView>();

        public void SetConfirmationText(Energy[] costs, UnityAction<Energy[]> onCanPayListen,
            UnityAction<Energy[]> onCantPayListen, Func<bool> canConfirm = null)
        {
            GetEnergyConfirmationWarningView()?.Hide();
            ShowInternal();
            GetEnergyConfirmationView().SetConfirmationText(costs, onCanPayListen, onCantPayListen, canConfirm);
        }
    }
}
