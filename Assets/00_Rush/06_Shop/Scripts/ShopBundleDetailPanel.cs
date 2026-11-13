using System.Collections;
using LegionKnight;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Rush
{
    public class ShopBundleDetailPanel : PanelView
    {
        [SerializeField] private Image m_BundleImage;
        [SerializeField] private ShopBundleArtwork m_Artwork;
        [SerializeField] private TextMeshProUGUI m_BundleNameText;
        [SerializeField] private TextMeshProUGUI m_BundleDescText;
        [SerializeField] private TextMeshProUGUI m_EntryCountText;
        [SerializeField] private Transform m_ContentsContainer;
        [SerializeField] private ShopBundleContentUI m_ContentItemPrefab;
        [SerializeField] private ScrollRect m_ContentsScroll;
        [SerializeField] private RectTransform m_Dialog;
        [SerializeField] private RectTransform m_ContentsCard;
        [SerializeField] private TextMeshProUGUI m_OfferStatus;
        [SerializeField] private float m_SingleHeight = 2120f;
        [SerializeField] private float m_MultipleHeight = 2740f;
        [SerializeField] private GameObject m_FreeLabel;
        [SerializeField] private GameObject m_PriceGroup;
        [SerializeField] private Image m_CurrencyIcon;
        [SerializeField] private GameObject m_OriginalCostContent;
        [SerializeField] private StrikethroughText m_OriginalPriceText;
        [SerializeField] private TextMeshProUGUI m_FinalPriceText;
        [SerializeField] private GameObject m_DiscountBadge;
        [SerializeField] private TextMeshProUGUI m_DiscountPercentText;
        [SerializeField] private GameObject m_FirstPurchaseBadge;
        [SerializeField] private GameObject m_DailyBadge;
        [SerializeField] private Button m_BuyButton;
        [SerializeField] private TextMeshProUGUI m_BuyButtonText;
        [SerializeField] private Button m_CancelButton;

        private const float c_RefreshSeconds = 1f;
        private ShopBundleConfig m_Bundle;
        private Coroutine m_RefreshRoutine;
        private ShopManager Manager => RushPlayer.Instance.ShopManager;

        public void Show(ShopBundleConfig bundle)
        {
            if (bundle == null) return;
            m_Bundle = bundle;
            ShowInternal();
        }

        protected override void ShowInternal()
        {
            if (m_Bundle == null) return;
            base.ShowInternal();
            if (!IsShown) return;
            m_BuyButton.onClick.RemoveListener(OnBuyClickedInternal);
            m_BuyButton.onClick.AddListener(OnBuyClickedInternal);
            m_CancelButton.onClick.RemoveListener(HideInternal);
            m_CancelButton.onClick.AddListener(HideInternal);
            if (m_OfferStatus != null) m_OfferStatus.text = string.Empty;
            PopulateContentsInternal();
            RefreshOfferInternal();
            if (m_RefreshRoutine == null) m_RefreshRoutine = StartCoroutine(RefreshWhileOpenInternal());
        }

        protected override void HideInternal()
        {
            StopRefreshingInternal();
            base.HideInternal();
        }

        private void OnDisable() => StopRefreshingInternal();

        private void StopRefreshingInternal()
        {
            if (m_RefreshRoutine == null) return;
            StopCoroutine(m_RefreshRoutine);
            m_RefreshRoutine = null;
        }

        private IEnumerator RefreshWhileOpenInternal()
        {
            var wait = new WaitForSecondsRealtime(c_RefreshSeconds);
            while (IsShown)
            {
                yield return wait;
                if (IsShown) RefreshOfferInternal();
            }
            m_RefreshRoutine = null;
        }

        private void PopulateContentsInternal()
        {
            if (m_BundleImage != null) m_BundleImage.sprite = m_Bundle.BundleSprite;
            if (m_Artwork != null) m_Artwork.Bind(m_Bundle);
            if (m_BundleNameText != null) m_BundleNameText.text = m_Bundle.BaseInfo.Name;
            int count = m_Bundle.Entries?.Length ?? 0;
            if (m_EntryCountText != null) m_EntryCountText.text = count == 1 ? "1 item" : $"{count} types";
            if (m_Dialog != null && m_Dialog.parent is RectTransform parent)
                m_Dialog.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical,
                    Mathf.Min(count > 1 ? m_MultipleHeight : m_SingleHeight, parent.rect.height * 0.92f));
            if (m_ContentsCard != null)
            {
                Vector2 max = m_ContentsCard.anchorMax;
                max.y = count == 1 ? 0.63f : 0.682f;
                m_ContentsCard.anchorMax = max;
            }
            if (m_BundleDescText != null)
            {
                string desc = m_Bundle.BaseInfo.Description;
                m_BundleDescText.text = !string.IsNullOrWhiteSpace(desc) ? desc : m_Bundle.ContentsLabel;
            }
            if (m_ContentsContainer == null || m_ContentItemPrefab == null) return;
            foreach (Transform child in m_ContentsContainer)
            {
                child.gameObject.SetActive(false);
                Destroy(child.gameObject);
            }
            if (m_Bundle.Entries != null)
                foreach (var entry in m_Bundle.Entries)
                    if (entry?.Collectible != null)
                        Instantiate(m_ContentItemPrefab, m_ContentsContainer).Setup(entry);
            if (m_ContentsScroll != null)
            {
                m_ContentsScroll.StopMovement();
                Canvas.ForceUpdateCanvases();
                m_ContentsScroll.verticalNormalizedPosition = 1f;
            }
        }

        internal void RefreshOfferInternal()
        {
            if (!IsShown || m_Bundle == null) return;
            var availability = Manager.GetAvailability(m_Bundle);
            var price = Manager.GetBreakdown(m_Bundle);
            bool discount = price.HasDiscount;
            if (m_PriceGroup != null) m_PriceGroup.SetActive(!price.IsFree);
            if (m_FreeLabel != null) m_FreeLabel.SetActive(price.IsFree);
            if (m_CurrencyIcon != null) m_CurrencyIcon.sprite = m_Bundle.CostCurrency?.CollectibleField?.Icon;
            if (m_OriginalCostContent != null) m_OriginalCostContent.SetActive(discount);
            if (m_OriginalPriceText != null && discount) m_OriginalPriceText.SetText(price.OriginalPrice.ToString());
            if (m_FinalPriceText != null) m_FinalPriceText.text = price.MainCurrencyAmount.ToString();
            if (m_DiscountBadge != null) m_DiscountBadge.SetActive(discount);
            if (m_DiscountPercentText != null && discount) m_DiscountPercentText.text = price.DiscountLabel;
            if (m_FirstPurchaseBadge != null) m_FirstPurchaseBadge.SetActive(price.IsFirstPurchaseDiscount);
            if (m_DailyBadge != null) m_DailyBadge.SetActive(availability.IsDaily);
            if (m_BuyButton != null) m_BuyButton.interactable = availability.CanPurchase && price.CanAfford;
            if (m_BuyButtonText != null)
                m_BuyButtonText.text = !availability.CanPurchase ? (availability.IsDaily ? "Claimed" : "Sold out")
                    : !price.CanAfford ? "Insufficient funds" : price.IsFree ? "Claim" : "Buy";
        }

        internal void ShowFailureInternal(string message)
        {
            if (!IsShown) return;
            RefreshOfferInternal();
            if (m_OfferStatus != null) m_OfferStatus.text = message;
        }

        private void OnBuyClickedInternal()
        {
            if (!m_BuyButton.interactable) return;
            if (m_OfferStatus != null) m_OfferStatus.text = string.Empty;
            Manager.RequestPurchase(m_Bundle);
        }
    }
}
