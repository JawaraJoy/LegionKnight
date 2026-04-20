using UnityEngine;
using UnityEngine.UI;
using TMPro;
using LegionKnight;

namespace Rush
{
    public class ShopBundleDetailPanel : PanelView
    {
        [Header("Info")]
        [SerializeField] private Image m_BundleImage;
        [SerializeField] private TextMeshProUGUI m_BundleNameText;
        [SerializeField] private TextMeshProUGUI m_BundleDescText;

        [Header("Contents — isi bundle")]
        [SerializeField] private Transform m_ContentsContainer;
        [SerializeField] private ShopBundleContentUI m_ContentItemPrefab;

        [Header("Buttons")]
        [SerializeField] private Button m_BuyButton;
        [SerializeField] private TextMeshProUGUI m_BuyButtonText;
        [SerializeField] private Button m_CancelButton;

        private ShopBundleConfig m_Bundle;

        private ShopManager Manager => RushPlayer.Instance.ShopManager;

        protected override void ShowInternal()
        {
            base.ShowInternal();
            if (m_BuyButton != null) m_BuyButton.onClick.AddListener(OnBuyClickedInternal);
            if (m_CancelButton != null) m_CancelButton.onClick.AddListener(Hide);
            Manager.OnPurchaseCompleteBundle.AddListener(OnPurchaseCompleteBundleInternal);
        }

        protected override void HideInternal()
        {
            if (m_BuyButton != null) m_BuyButton.onClick.RemoveListener(OnBuyClickedInternal);
            if (m_CancelButton != null) m_CancelButton.onClick.RemoveListener(Hide);
            Manager.OnPurchaseCompleteBundle.RemoveListener(OnPurchaseCompleteBundleInternal);
            base.HideInternal();
        }

        public void Show(ShopBundleConfig bundle)
        {
            m_Bundle = bundle;
            Show();
            RefreshViewInternal();
        }

        // ── Refresh ───────────────────────────────────────────────────────────

        private void RefreshViewInternal()
        {
            if (m_Bundle == null) return;

            var availability = Manager.GetAvailability(m_Bundle);
            var breakdown = Manager.GetBreakdown(m_Bundle);

            if (m_BundleImage != null) m_BundleImage.sprite = m_Bundle.BundleSprite;
            if (m_BundleNameText != null) m_BundleNameText.text = m_Bundle.BaseInfo.Name;
            if (m_BundleDescText != null) m_BundleDescText.text = m_Bundle.BaseInfo.Description;

            PopulateContentsInternal(m_Bundle);
            RefreshButtonInternal(breakdown, availability);
        }

        private void PopulateContentsInternal(ShopBundleConfig bundle)
        {
            if (m_ContentsContainer == null || m_ContentItemPrefab == null) return;

            foreach (Transform child in m_ContentsContainer)
                Destroy(child.gameObject);

            if (bundle.Entries == null) return;

            foreach (var entry in bundle.Entries)
            {
                var item = Instantiate(m_ContentItemPrefab, m_ContentsContainer);
                item.Setup(entry);
            }
        }

        private void RefreshButtonInternal(ShopCostBreakdown breakdown,
            ShopBundleAvailability availability)
        {
            if (m_BuyButton == null) return;
            m_BuyButton.interactable = breakdown.CanAfford && availability.CanPurchase;

            if (m_BuyButtonText != null)
                m_BuyButtonText.text = breakdown.IsFree ? "Take" : "Buy";
        }

        // ── Callbacks ─────────────────────────────────────────────────────────

        private void OnBuyClickedInternal() =>
            Manager.RequestPurchase(m_Bundle);

        // Refresh tombol setelah beli — misal daily jadi disabled setelah dibeli
        private void OnPurchaseCompleteBundleInternal(ShopBundleConfig bundle)
        {
            if (m_Bundle != bundle) return;
            RefreshViewInternal();
        }
    }
}