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

        // baris utama: selalu tampil
        [SerializeField] private TextMeshProUGUI m_MainCostText;
        [SerializeField] private Image m_MainCostIcon;

        // baris alt: hanya tampil jika mixed
        [SerializeField] private GameObject m_AltCostRow;
        [SerializeField] private TextMeshProUGUI m_AltCostText;
        [SerializeField] private Image m_AltCostIcon;

        // label "+" pemisah antar currency row
        [SerializeField] private GameObject m_PlusLabel;

        [SerializeField] private Button m_ConfirmButton;
        [SerializeField] private Button m_CancelButton;
        [SerializeField] private Toggle m_DontShowAgainToggle;

        private System.Action m_ConfirmCallback;
        private GachaConfirmData m_ConfirmData;

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

        // Dipanggil dari GachaPanel saat menerima OnDrawRequested
        public void ShowConfirmation(GachaConfirmData data, System.Action onConfirm)
        {
            if (IsDontShowEnabledInternal())
            {
                onConfirm?.Invoke();
                return;
            }

            m_ConfirmData = data;
            m_ConfirmCallback = onConfirm;

            RefreshViewInternal();

            if (m_DontShowAgainToggle != null) m_DontShowAgainToggle.isOn = false;
            Show();
        }

        public void ResetDontShow() =>
            UnityService.Instance.SaveData(DontShowKey, false);

        private void RefreshViewInternal()
        {
            if (m_ConfirmData == null) return;

            var banner = m_ConfirmData.Banner;
            var breakdown = m_ConfirmData.Breakdown;
            bool isMulti = m_ConfirmData.IsMulti;

            string drawLabel = isMulti ? $"Draw {banner.MultiDrawCount}x?" : "Draw 1x?";
            if (m_DescriptionText != null) m_DescriptionText.text = drawLabel;

            // Main currency row
            if (m_MainCostText != null)
                m_MainCostText.text = $"{breakdown.MainCurrencyAmount} " +
                                      $"{banner.DrawCostCurrency.BaseInfo.Name}";
            if (m_MainCostIcon != null && banner.DrawCostCurrency is ItemConfig mainItem)
                m_MainCostIcon.sprite = mainItem.CollectibleField.Icon;
            // pasang icon dari ItemConfig jika ada field icon-nya

            // Alt currency row — tampil hanya jika mixed
            bool showAlt = breakdown.IsMixed && breakdown.AltCurrencyAmount > 0;
            if (m_AltCostRow != null) m_AltCostRow.SetActive(showAlt);
            if (m_PlusLabel != null) m_PlusLabel.SetActive(showAlt);

            if (showAlt)
            {
                if (m_AltCostText != null)
                    m_AltCostText.text = $"{breakdown.AltCurrencyAmount} " +
                                         $"{banner.AltCostCurrency?.BaseInfo.Name}";
            }
        }

        private void OnConfirmClickedInternal()
        {
            if (m_DontShowAgainToggle != null && m_DontShowAgainToggle.isOn)
                UnityService.Instance.SaveData(DontShowKey, true);

            m_ConfirmCallback?.Invoke();
            m_ConfirmCallback = null;
            m_ConfirmData = null;
            Hide();
        }

        private void OnCancelClickedInternal()
        {
            m_ConfirmCallback = null;
            m_ConfirmData = null;
            Hide();
        }

        private bool IsDontShowEnabledInternal() =>
            UnityService.Instance.HasData(DontShowKey)
            && UnityService.Instance.GetData<bool>(DontShowKey);
    }
}