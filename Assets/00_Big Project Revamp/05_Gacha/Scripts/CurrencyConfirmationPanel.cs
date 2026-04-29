using UnityEngine;
using UnityEngine.UI;
using TMPro;
using LegionKnight;

namespace Rush
{
    public class CurrencyConfirmationPanel : PanelView
    {
        [SerializeField] private TextMeshProUGUI m_DescriptionText;

        [Header("Main Currency")]
        [SerializeField] private Image m_MainCostIcon;
        [SerializeField] private TextMeshProUGUI m_MainCostText;

        [Header("Alt Currency — tampil jika HasAltCurrency")]
        [SerializeField] private GameObject m_AltCostRow;
        [SerializeField] private Image m_AltCostIcon;
        [SerializeField] private TextMeshProUGUI m_AltCostText;
        [SerializeField] private GameObject m_PlusLabel;

        [SerializeField] private Button m_ConfirmButton;
        [SerializeField] private Button m_CancelButton;
        [SerializeField] private Toggle m_DontShowAgainToggle;

        private System.Action m_ConfirmCallback;
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

            // Main currency
            if (m_MainCostIcon != null)
            {
                m_MainCostIcon.sprite = data.MainCurrencyIcon;
                m_MainCostIcon.gameObject.SetActive(data.MainCurrencyIcon != null);
            }

            if (m_MainCostText != null)
                m_MainCostText.text = data.IsFree
                    ? "Free"
                    : $"{data.MainCurrencyAmount}";

            // Alt currency
            bool showAlt = !data.IsFree && data.HasAltCurrency;
            if (m_AltCostRow != null) m_AltCostRow.SetActive(showAlt);
            if (m_PlusLabel != null) m_PlusLabel.SetActive(showAlt);

            if (showAlt)
            {
                if (m_AltCostIcon != null)
                {
                    m_AltCostIcon.sprite = data.AltCurrencyIcon;
                    m_AltCostIcon.gameObject.SetActive(data.AltCurrencyIcon != null);
                }

                if (m_AltCostText != null)
                    m_AltCostText.text = $"{data.AltCurrencyAmount}";
            }
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