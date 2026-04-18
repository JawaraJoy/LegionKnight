using UnityEngine;
using UnityEngine.UI;
using TMPro;
using LegionKnight;

namespace Rush
{
    public class GachaPanel : PanelView
    {
        [SerializeField] private Image m_BannerImage;
        [SerializeField] private GachaDrawButtonUI m_DrawSingleButtonUI;
        [SerializeField] private GachaDrawButtonUI m_DrawMultiButtonUI;
        [SerializeField] private Button m_DetailButton;
        [SerializeField] private TextMeshProUGUI m_PityProgressText;
        [SerializeField] private TextMeshProUGUI m_BannerNameText;
        [SerializeField] private Transform m_BannerButtonContainer;
        [SerializeField] private GachaBannerButtonUI m_BannerButtonPrefab;

        private GachaManager Manager => RushPlayer.Instance.GachaManager;

        protected override void ShowInternal()
        {
            base.ShowInternal();
            RefreshViewInternal();
            PopulateBannerButtonsInternal();
            SubscribeEventsInternal();
        }

        protected override void HideInternal()
        {
            UnsubscribeEventsInternal();
            base.HideInternal();
        }

        private void SubscribeEventsInternal()
        {
            Manager.OnDrawComplete.AddListener(OnDrawCompleteInternal);
            Manager.OnDrawFailed.AddListener(OnDrawFailedInternal);
            Manager.OnDrawRequested.AddListener(OnDrawRequestedInternal);

            if (m_DrawSingleButtonUI?.Button != null)
                m_DrawSingleButtonUI.Button.onClick.AddListener(OnDrawSingleClickedInternal);
            if (m_DrawMultiButtonUI?.Button != null)
                m_DrawMultiButtonUI.Button.onClick.AddListener(OnDrawMultiClickedInternal);
            if (m_DetailButton != null)
                m_DetailButton.onClick.AddListener(OnDetailClickedInternal);
        }

        private void UnsubscribeEventsInternal()
        {
            Manager.OnDrawComplete.RemoveListener(OnDrawCompleteInternal);
            Manager.OnDrawFailed.RemoveListener(OnDrawFailedInternal);
            Manager.OnDrawRequested.RemoveListener(OnDrawRequestedInternal);

            if (m_DrawSingleButtonUI?.Button != null)
                m_DrawSingleButtonUI.Button.onClick.RemoveListener(OnDrawSingleClickedInternal);
            if (m_DrawMultiButtonUI?.Button != null)
                m_DrawMultiButtonUI.Button.onClick.RemoveListener(OnDrawMultiClickedInternal);
            if (m_DetailButton != null)
                m_DetailButton.onClick.RemoveListener(OnDetailClickedInternal);
        }

        private void RefreshViewInternal()
        {
            var banner = Manager.ActiveBanner;
            if (banner == null) return;

            if (m_BannerImage != null) m_BannerImage.sprite = banner.BannerSplashSprite;

            if (m_PityProgressText != null)
                m_PityProgressText.text =
                    $"{Manager.PityTracker.FinalPityCounter}/{banner.FinalPityCount}";


            RefreshDrawButtonInternal(m_DrawSingleButtonUI, banner, false);
            RefreshDrawButtonInternal(m_DrawMultiButtonUI, banner, true);
        }

        private void RefreshDrawButtonInternal(GachaDrawButtonUI buttonUI,
            GachaBannerConfig banner, bool isMulti)
        {
            if (buttonUI == null) return;

            // Hitung biaya tanpa discount untuk original
            int baseCost = isMulti
                ? banner.SingleDrawCost * banner.MultiDrawCount
                : banner.SingleDrawCost;

            // Hitung biaya setelah discount (final)
            var breakdown = Manager.GetBreakdown(isMulti);
            // total final cost = main + alt yang akan dibayar
            int finalCost = breakdown.MainCurrencyAmount + ConvertAltToMainEquivalentInternal(
                banner, breakdown.AltCurrencyAmount);

            // Icon: ambil dari ItemConfig currency utama
            // Asumsikan ItemConfig memiliki field icon via CollectibleField
            Sprite currencyIcon = GetCurrencyIconInternal(banner.DrawCostCurrency);

            buttonUI.Refresh(baseCost, finalCost, currencyIcon);
            buttonUI.SetInteractable(breakdown.CanAfford);
        }

        // Konversi alt currency amount ke equivalent main untuk keperluan display total
        private int ConvertAltToMainEquivalentInternal(GachaBannerConfig banner, int altAmount)
        {
            if (altAmount <= 0 || banner.AltSingleDrawCost <= 0) return 0;
            return Mathf.RoundToInt((float)altAmount / banner.AltSingleDrawCost
                                    * banner.SingleDrawCost);
        }

        private Sprite GetCurrencyIconInternal(ItemConfig itemConfig)
        {
            if (itemConfig == null) return null;
            // Ambil icon melalui CollectibleField sesuai pattern yang sudah ada
            return itemConfig.CollectibleField?.Icon;
        }

        private void PopulateBannerButtonsInternal()
        {
            if (m_BannerButtonContainer == null || m_BannerButtonPrefab == null) return;
            foreach (Transform child in m_BannerButtonContainer) Destroy(child.gameObject);
            foreach (var banner in Manager.Banners)
            {
                var btn = Instantiate(m_BannerButtonPrefab, m_BannerButtonContainer);
                btn.Setup(banner, OnBannerSelectedInternal);
            }
        }

        private void OnBannerSelectedInternal(GachaBannerConfig banner)
        {
            Manager.SelectBanner(banner);
            if (m_BannerNameText != null)
                m_BannerNameText.text = banner.BaseInfo.Name;
            RefreshViewInternal();
        }

        private void OnDrawSingleClickedInternal() => Manager.RequestDrawSingle();
        private void OnDrawMultiClickedInternal() => Manager.RequestDrawMulti();

        private void OnDrawRequestedInternal(GachaConfirmData data)
        {
            var confirmPanel = CanvasManager.Instance.GetPanel<CurrencyConfirmationPanel>();
            System.Action executeAction = data.IsMulti
                ? Manager.ExecuteDrawMulti
                : Manager.ExecuteDrawSingle;
            confirmPanel?.ShowConfirmation(data, executeAction);
        }

        private void OnDetailClickedInternal()
        {
            var detailPanel = CanvasManager.Instance.GetPanel<BannerDetailPanel>();
            detailPanel.Show(Manager.ActiveBanner);
        }

        private void OnDrawCompleteInternal(CollectibleResultData result)
        {
            RefreshViewInternal();
            var resultPanel = CanvasManager.Instance.GetPanel<GachaResultPanel>();
            resultPanel?.Show(result);
        }

        private void OnDrawFailedInternal(string message) =>
            Debug.LogWarning($"[GachaPanel] {message}");
    }
}