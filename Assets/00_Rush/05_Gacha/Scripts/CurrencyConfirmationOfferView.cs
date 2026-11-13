using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Rush
{
    public sealed class CurrencyConfirmationOfferView : MonoBehaviour
    {
        [System.Serializable]
        private sealed class HeadingField
        {
            [SerializeField] private TMP_Text m_Title;
            [SerializeField] private TMP_Text m_Name;
            [SerializeField] private TMP_Text m_Subtitle;
            [SerializeField] private TMP_Text m_AmountHeading;
            [SerializeField] private GameObject m_FreeBadge;
            [SerializeField] private GameObject m_FirstPurchase;

            public void Bind(string title, string name, string subtitle, bool free, bool firstPurchase)
            {
                m_Title.text = title;
                m_Name.text = name;
                m_Subtitle.text = subtitle;
                m_AmountHeading.text = free ? "You receive" : "You spend";
                m_FreeBadge.SetActive(free);
                m_FirstPurchase.SetActive(firstPurchase);
            }
        }

        [System.Serializable]
        private sealed class ArtworkField
        {
            [SerializeField] private ShopBundleArtwork m_Bundle;
            [SerializeField] private Image m_PortraitFrame;
            [SerializeField] private Image m_Portrait;

            public void Bind(ShopBundleConfig bundle, GachaBannerConfig banner)
            {
                m_Bundle.gameObject.SetActive(bundle != null);
                if (bundle != null) m_Bundle.Bind(bundle);
                Sprite icon = banner != null ? banner.BannerButtonSprite : null;
                m_PortraitFrame.gameObject.SetActive(icon != null);
                if (icon == null) return;
                m_Portrait.sprite = icon;
                var hero = PortraitCollectible(banner.FinalPityGuarantees, icon) ?? PortraitCollectible(banner.Collectables, icon);
                m_PortraitFrame.color = hero?.CollectibleField?.RarityConfig?.Color ?? Color.white;
            }

            private static CollectibleConfig PortraitCollectible(GachaCollectableConfig[] pool, Sprite icon)
            {
                if (pool != null)
                    foreach (var entry in pool)
                        if (entry?.Collect?.CollectibleField?.Icon == icon) return entry.Collect;
                return null;
            }
        }

        private const int c_VisibleRows = 2;
        [SerializeField] private HeadingField m_Heading;
        [SerializeField] private ArtworkField m_Artwork;
        [SerializeField] private ScrollRect m_Amounts;
        [SerializeField] private CurrencyConfirmationRow m_RowTemplate;
        private readonly List<CurrencyConfirmationRow> m_Rows = new();
        private int m_Count;

        internal void Bind(IConfirmData data)
        {
            m_Count = 0;
            var shop = data as ShopConfirmData;
            var draw = data as GachaConfirmData;
            var bundle = shop?.Bundle;
            var banner = draw?.Banner;
            bool free = data.IsFree;
            string title = draw != null ? "Confirm Summon" : free ? "Confirm Claim" : "Confirm Purchase";
            string name = bundle?.BaseInfo.Name ?? banner?.BaseInfo.Name ?? data.DescriptionText;
            string subtitle = bundle?.ContentsLabel;
            if (draw != null) subtitle = $"Draw ×{(draw.IsMulti ? banner.MultiDrawCount : 1)}";
            else if (free && bundle?.PurchaseLimit == ShopBundlePurchaseLimit.Daily) subtitle = "Daily reward";
            m_Heading.Bind(title, name, subtitle, free, !free && shop?.Breakdown.IsFirstPurchaseDiscount == true);
            m_Artwork.Bind(bundle, banner);

            if (free && bundle?.Entries != null)
            {
                foreach (var entry in bundle.Entries)
                    if (entry?.Collectible != null && entry.Amount > 0)
                        AddItemInternal(entry.Collectible, $"×{entry.Amount:N0}");
            }
            else
            {
                if (data.MainCurrencyAmount > 0)
                    AddAmountInternal(data.MainCurrencyName, data.MainCurrencyIcon,
                        (bundle?.CostCurrency ?? banner?.DrawCostCurrency)?.CollectibleField?.RarityConfig,
                        data.MainCurrencyAmount.ToString("N0"));
                if (data.HasAltCurrency && data.AltCurrencyAmount > 0)
                    AddAmountInternal(data.AltCurrencyName, data.AltCurrencyIcon,
                        banner?.AltCostCurrency?.CollectibleField?.RarityConfig, data.AltCurrencyAmount.ToString("N0"));
            }
            LayoutRowsInternal();
        }

        private void AddItemInternal(CollectibleConfig item, string amount) =>
            AddAmountInternal(item.BaseInfo.Name, item.CollectibleField?.Icon, item.CollectibleField?.RarityConfig, amount);

        private void AddAmountInternal(string name, Sprite icon, RarityConfig rarity, string amount)
        {
            if (m_Count == m_Rows.Count)
                m_Rows.Add(Instantiate(m_RowTemplate, m_Amounts.content, false));
            var row = m_Rows[m_Count];
            row.gameObject.SetActive(true);
            row.Bind(name, icon, rarity, amount, m_Count > 0);
            m_Count++;
        }

        private void LayoutRowsInternal()
        {
            for (int i = m_Count; i < m_Rows.Count; i++) m_Rows[i].gameObject.SetActive(false);
            m_Amounts.StopMovement();
            m_Amounts.vertical = m_Count > c_VisibleRows;
            var content = m_Amounts.content;
            float pages = Mathf.Max(1f, (float)m_Count / c_VisibleRows);
            content.anchorMin = new Vector2(0f, 1f - pages);
            content.anchorMax = Vector2.one;
            content.sizeDelta = Vector2.zero;
            content.anchoredPosition = Vector2.zero;
            for (int i = 0; i < m_Count; i++)
            {
                var rect = (RectTransform)m_Rows[i].transform;
                rect.anchorMin = new Vector2(0f, 1f - (float)(i + 1) / m_Count);
                rect.anchorMax = new Vector2(1f, 1f - (float)i / m_Count);
                rect.sizeDelta = Vector2.zero;
                rect.anchoredPosition = Vector2.zero;
            }
        }
    }
}
