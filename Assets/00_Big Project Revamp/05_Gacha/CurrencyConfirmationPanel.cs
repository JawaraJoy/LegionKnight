using UnityEngine;
using UnityEngine.UI;
using TMPro;
using LegionKnight;

namespace Rush
{
    public class CurrencyConfirmationPanel : PanelView
    {

        [SerializeField] private TextMeshProUGUI m_DescriptionText;

        [SerializeField] private Image m_MainCostIcon;
        [SerializeField] private TextMeshProUGUI m_MainCostText;

        // Alt row — hanya tampil jika IConfirmData.HasAltCurrency true
        [SerializeField] private GameObject m_AltCostRow;
        [SerializeField] private TextMeshProUGUI m_AltCostText;
        [SerializeField] private GameObject m_PlusLabel;

        [SerializeField] private Button m_ConfirmButton;
        [SerializeField] private Button m_CancelButton;
        [SerializeField] private Toggle m_DontShowAgainToggle;

        private System.Action m_ConfirmCallback;
        // tidak disave — reset otomatis tiap game restart
        private bool m_DontShowAgain;

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

        public void ShowConfirmation(IConfirmData data, System.Action onConfirm)
        {
            if (m_DontShowAgain)
            {
                onConfirm?.Invoke();
                return;
            }

            m_ConfirmCallback = onConfirm;
            RefreshViewInternal(data);

            if (m_DontShowAgainToggle != null) m_DontShowAgainToggle.isOn = false;
            Show();
        }

        private void RefreshViewInternal(IConfirmData data)
        {
            if (m_DescriptionText != null)
                m_DescriptionText.text = data.DescriptionText;

            if (m_MainCostIcon != null)
            {
                m_MainCostIcon.sprite = data.MainCurrencyIcon;
                m_MainCostIcon.gameObject.SetActive(data.MainCurrencyIcon != null);
            }

            if (m_MainCostText != null)
                m_MainCostText.text = data.IsFree
                    ? "Gratis"
                    : $"{data.MainCurrencyAmount} {data.MainCurrencyName}";

            if (m_AltCostRow != null) m_AltCostRow.SetActive(data.HasAltCurrency);
            if (m_PlusLabel != null) m_PlusLabel.SetActive(data.HasAltCurrency);

            if (data.HasAltCurrency && m_AltCostText != null)
                m_AltCostText.text = $"{data.AltCurrencyAmount} {data.AltCurrencyName}";
        }

        private void OnConfirmClickedInternal()
        {
            if (m_DontShowAgainToggle != null)
                m_DontShowAgain = m_DontShowAgainToggle.isOn;

            m_ConfirmCallback?.Invoke();
            m_ConfirmCallback = null;
            Hide();
        }

        private void OnCancelClickedInternal()
        {
            m_ConfirmCallback = null;
            Hide();
        }
    }
}