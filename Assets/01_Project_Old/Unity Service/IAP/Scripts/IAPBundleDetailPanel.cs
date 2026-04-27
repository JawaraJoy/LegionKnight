using UnityEngine;
using UnityEngine.UI;
using TMPro;
using LegionKnight;

namespace Rush
{
    public class IAPBundleDetailPanel : PanelView
    {
        [Header("Info")]
        [SerializeField] private Image m_BundleImage;
        [SerializeField] private TextMeshProUGUI m_BundleNameText;
        [SerializeField] private TextMeshProUGUI m_BundleDescText;

        [Header("Contents")]
        [SerializeField] private Transform m_ContentsContainer;
        [SerializeField] private ShopBundleContentUI m_ContentItemPrefab;

        [Header("First Purchase Bonus")]
        [SerializeField] private GameObject m_BonusSection;
        [SerializeField] private Transform m_BonusContainer;
        [SerializeField] private ShopBundleContentUI m_BonusItemPrefab;

        [Header("Price")]
        [SerializeField] private TextMeshProUGUI m_PriceText;

        [Header("Buttons")]
        [SerializeField] private Button m_BuyButton;
        [SerializeField] private TextMeshProUGUI m_BuyButtonText;
        [SerializeField] private Button m_CancelButton;

        private IAPBundleConfig m_Bundle;

        private IAPManager IAPManager => UnityService.Instance.IAPManager;

        protected override void ShowInternal()
        {
            base.ShowInternal();
            if (m_BuyButton != null) m_BuyButton.onClick.AddListener(OnBuyClickedInternal);
            if (m_CancelButton != null) m_CancelButton.onClick.AddListener(Hide);
            IAPManager.OnPurchaseSuccess.AddListener(OnPurchaseSuccessInternal);
        }

        protected override void HideInternal()
        {
            if (m_BuyButton != null) m_BuyButton.onClick.RemoveListener(OnBuyClickedInternal);
            if (m_CancelButton != null) m_CancelButton.onClick.RemoveListener(Hide);
            IAPManager.OnPurchaseSuccess.RemoveListener(OnPurchaseSuccessInternal);
            base.HideInternal();
        }

        public void Show(IAPBundleConfig bundle)
        {
            m_Bundle = bundle;
            Show();
            RefreshViewInternal();
        }

        // ── Refresh ───────────────────────────────────────────────────────────

        private void RefreshViewInternal()
        {
            if (m_Bundle == null) return;

            if (m_BundleImage != null) m_BundleImage.sprite = m_Bundle.BundleSprite;
            if (m_BundleNameText != null) m_BundleNameText.text = m_Bundle.BaseInfo.Name;
            if (m_BundleDescText != null) m_BundleDescText.text = m_Bundle.BaseInfo.Description;

            if (m_PriceText != null)
                m_PriceText.text = IAPManager.GetLocalizedPrice(m_Bundle);

            PopulateContentsInternal();
            RefreshBonusSectionInternal();
            RefreshBuyButtonInternal();
        }

        private void PopulateContentsInternal()
        {
            if (m_ContentsContainer == null || m_ContentItemPrefab == null) return;
            foreach (Transform child in m_ContentsContainer) Destroy(child.gameObject);
            if (m_Bundle.Entries == null) return;

            foreach (var entry in m_Bundle.Entries)
            {
                var item = Instantiate(m_ContentItemPrefab, m_ContentsContainer);
                item.SetupFromIAP(entry);
            }
        }

        private void RefreshBonusSectionInternal()
        {
            bool isFirst = IAPManager.IsFirstPurchase(m_Bundle);
            bool hasBonus = m_Bundle.HasFirstPurchaseBonus;
            bool show = isFirst && hasBonus;

            if (m_BonusSection != null) m_BonusSection.SetActive(show);
            if (!show || m_BonusContainer == null || m_BonusItemPrefab == null) return;

            foreach (Transform child in m_BonusContainer) Destroy(child.gameObject);
            foreach (var entry in m_Bundle.FirstPurchaseBonusEntries)
            {
                var item = Instantiate(m_BonusItemPrefab, m_BonusContainer);
                item.SetupFromIAP(entry);
            }
        }

        private void RefreshBuyButtonInternal()
        {
            if (m_BuyButton == null) return;
            bool canPurchase = IAPManager.CanPurchase(m_Bundle);
            m_BuyButton.interactable = canPurchase;
        }

        // ── Callbacks ─────────────────────────────────────────────────────────

        private void OnBuyClickedInternal()
        {
            if (!IAPManager.CanPurchase(m_Bundle))
            {
                ShowUnavailableMessageInternal();
                return;
            }

            IAPManager.Purchase(m_Bundle);
        }

        private void ShowUnavailableMessageInternal()
        {
            var popup = CanvasManager.Instance.GetPanel<TextPopUpPanel>();
            if (popup == null) return;

            string message = m_Bundle.PurchaseLimit switch
            {
                ShopBundlePurchaseLimit.OneTime =>
                    "This bundle can only be purchased once.",
                ShopBundlePurchaseLimit.Daily =>
                    "This bundle can only be purchased once per day. Come back tomorrow!",
                _ => "This bundle is not available right now."
            };

            popup.ShowText(message);
        }

        private void OnPurchaseSuccessInternal(IAPBundleConfig bundle)
        {
            if (m_Bundle != bundle) return;
            RefreshViewInternal();
        }
    }
}