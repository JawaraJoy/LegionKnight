using System.Collections.Generic;
using System.Globalization;
using LegionKnight;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Rush
{
    public class IAPBundleDetailPanel : PanelView
    {
        [SerializeField] private Image m_BundleImage;
        [SerializeField] private TextMeshProUGUI m_BundleNameText;
        [SerializeField] private TextMeshProUGUI m_BundleDescText;
        [SerializeField] private Transform m_ContentsContainer;
        [SerializeField] private ShopBundleContentUI m_ContentItemPrefab;
        [SerializeField] private GameObject m_BonusSection;
        [SerializeField] private Transform m_BonusContainer;
        [SerializeField] private ShopBundleContentUI m_BonusItemPrefab;
        [SerializeField] private TextMeshProUGUI m_PriceText;
        [SerializeField] private Button m_BuyButton;
        [SerializeField] private TextMeshProUGUI m_BuyButtonText;
        [SerializeField] private Button m_CancelButton;

        [System.Serializable]
        private class SummaryField
        {
            public TMP_Text Label => m_Label;
            public TMP_Text Amount => m_Amount;
            public Image Frame => m_Frame;
            public ScrollRect Scroll => m_Scroll;
            [SerializeField] private TMP_Text m_Label;
            [SerializeField] private TMP_Text m_Amount;
            [SerializeField] private Image m_Frame;
            [SerializeField] private ScrollRect m_Scroll;
        }

        [SerializeField] private SummaryField m_Summary = new();
        [SerializeField] private RechargeStoreView m_StoreView;
        private IAPBundleConfig m_Bundle;
        private IAPBundleConfig m_Displayed;
        private bool m_DisplayedBonus;
        private IAPManager Store => UnityService.Instance.IAPManager;

        public void Show(IAPBundleConfig bundle)
        {
            if (bundle == null) return;
            m_Bundle = bundle;
            ShowInternal();
            if (IsShown && m_Summary.Scroll != null)
            {
                Canvas.ForceUpdateCanvases();
                m_Summary.Scroll.StopMovement();
                m_Summary.Scroll.verticalNormalizedPosition = 1f;
            }
        }

        protected override void ShowInternal()
        {
            if (m_Bundle == null) return;
            bool opened = IsShown;
            if (!opened) base.ShowInternal();
            if (!IsShown) return;
            m_StoreView.Bind();
            if (!opened)
            {
                m_StoreView.Changed += RefreshStoreInternal;
                Store.OnPurchaseFailed.AddListener(OnPurchaseFailedInternal);
                if (m_BuyButton != null) m_BuyButton.onClick.AddListener(OnBuyClickedInternal);
                if (m_CancelButton != null) m_CancelButton.onClick.AddListener(HideInternal);
            }
            RefreshStoreInternal();
        }

        protected override void HideInternal()
        {
            if (IsShown)
            {
                m_StoreView.Changed -= RefreshStoreInternal;
                Store.OnPurchaseFailed.RemoveListener(OnPurchaseFailedInternal);
                if (m_BuyButton != null) m_BuyButton.onClick.RemoveListener(OnBuyClickedInternal);
                if (m_CancelButton != null) m_CancelButton.onClick.RemoveListener(HideInternal);
            }
            base.HideInternal();
        }

        private void RefreshContentsInternal()
        {
            if (m_BundleImage != null) m_BundleImage.sprite = m_Bundle.BundleSprite;
            if (m_BundleNameText != null) m_BundleNameText.text = m_Bundle.BaseInfo.Name;
            if (m_BundleDescText != null) m_BundleDescText.text = m_Bundle.BaseInfo.Description;
            bool bonus = Store.IsFirstPurchase(m_Bundle) && m_Bundle.HasFirstPurchaseBonus;
            var totals = new Dictionary<CollectibleConfig, long>();
            PopulateInternal(m_ContentsContainer, m_ContentItemPrefab, m_Bundle.Entries, totals);
            PopulateInternal(m_BonusContainer, m_BonusItemPrefab, bonus ? m_Bundle.FirstPurchaseBonusEntries : null, totals);
            if (m_BonusSection != null) m_BonusSection.SetActive(bonus);
            if (m_Summary.Frame != null && m_Bundle.Entries is { Length: > 0 })
                m_Summary.Frame.color = m_Bundle.Entries[0]?.Collectible?.CollectibleField.RarityConfig?.Color ?? Color.white;
            string label = "Reward types";
            string amount = totals.Count.ToString(CultureInfo.InvariantCulture);
            if (totals.Count == 1)
            {
                foreach (var pair in totals)
                {
                    label = "Total " + pair.Key.BaseInfo.Name;
                    amount = pair.Value.ToString("N0", CultureInfo.InvariantCulture);
                }
            }
            if (m_Summary.Label != null) m_Summary.Label.text = label;
            if (m_Summary.Amount != null) m_Summary.Amount.text = amount;
        }

        private void PopulateInternal(Transform container, ShopBundleContentUI prefab, IAPBundleEntry[] entries,
            Dictionary<CollectibleConfig, long> totals)
        {
            if (container == null || prefab == null) return;
            foreach (Transform child in container) { child.gameObject.SetActive(false); Destroy(child.gameObject); }
            if (entries == null) return;
            foreach (var entry in entries)
            {
                if (entry?.Collectible == null) continue;
                Instantiate(prefab, container, false).SetupFromIAP(entry);
                totals.TryGetValue(entry.Collectible, out long count);
                totals[entry.Collectible] = count + entry.Amount;
            }
        }

        private void RefreshStoreInternal()
        {
            if (m_Bundle == null || !IsShown) return;
            bool bonus = Store.IsFirstPurchase(m_Bundle) && m_Bundle.HasFirstPurchaseBonus;
            if (m_Displayed != m_Bundle || bonus != m_DisplayedBonus)
            {
                m_Displayed = m_Bundle;
                m_DisplayedBonus = bonus;
                RefreshContentsInternal();
            }
            bool eligible = Store.CanPurchase(m_Bundle);
            bool available = m_StoreView.IsAvailable(m_Bundle);
            bool waiting = m_StoreView.PurchaseInProgress;
            if (m_BuyButton != null) m_BuyButton.interactable = eligible && available && !waiting;
            string price = Store.GetLocalizedPrice(m_Bundle);
            if (m_BuyButtonText != null) m_BuyButtonText.text = !eligible ? "Sold out" :
                waiting ? "Processing..." : available ? "Buy - " + price :
                m_StoreView.Offline || m_StoreView.HasProducts ? "Unavailable" : "Loading price...";
            if (m_PriceText != null) m_PriceText.text = !eligible ? "Purchase limit reached." :
                m_StoreView.Offline ? "Store unavailable." : "Price provided by your store.";
        }

        private void OnPurchaseFailedInternal(IAPBundleConfig bundle, string reason)
        {
            if (bundle != null && bundle != m_Bundle) return;
            RefreshStoreInternal();
            if (m_PriceText != null) m_PriceText.text = reason == "UserCancelled" ?
                "Purchase canceled." : reason == "Your purchase is pending delivery." ? reason : "Purchase failed. Please try again.";
        }

        private void OnBuyClickedInternal()
        {
            if (m_Bundle != null && m_BuyButton.interactable) m_StoreView.RequestPurchase(m_Bundle);
        }
    }
}

