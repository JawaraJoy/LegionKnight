using UnityEngine;
using UnityEngine.UI;
using TMPro;
using LegionKnight;

namespace Rush
{
    public class CurrencyConfirmationPanel : PanelView
    {
        private const string DontShowKey = "GachaConfirmDontShow";

        [SerializeField] private TextMeshProUGUI m_DescriptionText;
        [SerializeField] private TextMeshProUGUI m_CostText;
        [SerializeField] private Button m_ConfirmButton;
        [SerializeField] private Button m_CancelButton;
        [SerializeField] private Toggle m_DontShowAgainToggle;

        private System.Action m_ConfirmCallback;

        protected override void ShowInternal()
        {
            base.ShowInternal();
            if (m_ConfirmButton != null) m_ConfirmButton.onClick.AddListener(OnConfirmClickedInternal);
            if (m_CancelButton != null) m_CancelButton.onClick.AddListener(OnCancelClickedInternal);
        }

        protected override void HideInternal()
        {
            if (m_ConfirmButton != null) m_ConfirmButton.onClick.RemoveListener(OnConfirmClickedInternal);
            if (m_CancelButton != null) m_CancelButton.onClick.RemoveListener(OnCancelClickedInternal);
            base.HideInternal();
        }

        public void ShowConfirmation(string description, int cost, ItemConfig costCurrency,
            System.Action onConfirm)
        {
            if (IsDontShowEnabledInternal())
            {
                onConfirm?.Invoke();
                return;
            }

            m_ConfirmCallback = onConfirm;

            if (m_DescriptionText != null) m_DescriptionText.text = description;
            if (m_CostText != null) m_CostText.text = $"{cost} {costCurrency?.BaseInfo.Name}";
            if (m_DontShowAgainToggle != null) m_DontShowAgainToggle.isOn = false;

            Show();
        }

        public void ResetDontShow() =>
            UnityService.Instance.SaveData(DontShowKey, false);

        private void OnConfirmClickedInternal()
        {
            if (m_DontShowAgainToggle != null && m_DontShowAgainToggle.isOn)
                UnityService.Instance.SaveData(DontShowKey, true);

            m_ConfirmCallback?.Invoke();
            m_ConfirmCallback = null;
            Hide();
        }

        private void OnCancelClickedInternal()
        {
            m_ConfirmCallback = null;
            Hide();
        }

        private bool IsDontShowEnabledInternal() =>
            UnityService.Instance.HasData(DontShowKey)
            && UnityService.Instance.GetData<bool>(DontShowKey);
    }
}   