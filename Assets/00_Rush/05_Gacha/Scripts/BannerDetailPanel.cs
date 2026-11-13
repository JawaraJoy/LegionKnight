using System.Collections.Generic;
using LegionKnight;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Rush
{
    public class BannerDetailPanel : PanelView
    {
        [System.Serializable]
        private sealed class BannerHeaderField
        {
            [SerializeField] private TextMeshProUGUI m_Name;
            [SerializeField] private TextMeshProUGUI m_Rarity;
            [SerializeField] private TextMeshProUGUI m_ChanceLabel;
            [SerializeField] private TextMeshProUGUI m_Chance;
            [SerializeField] private Image m_Artwork;
            [SerializeField] private Image m_Frame;

            public void Bind(GachaBannerConfig banner, CollectibleConfig featured, List<GachaRateInfo> rates)
            {
                m_Name.text = banner.BaseInfo.Name;
                m_Artwork.sprite = banner.PanelArtwork;
                m_Artwork.enabled = m_Artwork.sprite != null;
                var rarity = featured?.CollectibleField?.RarityConfig;
                m_Rarity.text = rarity == null ? string.Empty : rarity.BaseInfo.Name.ToUpperInvariant();
                m_Frame.color = rarity == null ? Color.white : rarity.Color;
                m_ChanceLabel.text = featured == null ? string.Empty : $"{featured.BaseInfo.Name} base chance";
                float chance = 0f;
                foreach (var rate in rates)
                    if (rate.Collectable.Collect == featured) chance += rate.Percent;
                m_Chance.text = featured == null ? string.Empty : $"{chance:F2}%";
            }
        }

        [System.Serializable]
        private sealed class PityField
        {
            [SerializeField] private TextMeshProUGUI m_Title;
            [SerializeField] private TextMeshProUGUI m_Count;
            [SerializeField] private TextMeshProUGUI m_Remaining;
            [SerializeField] private Image m_Fill;

            public void Bind(string title, int count, int threshold, bool available)
            {
                m_Title.text = title;
                m_Count.text = available ? $"{count} / {threshold}" : string.Empty;
                int remaining = Mathf.Max(1, threshold - count);
                m_Remaining.text = available
                    ? (remaining == 1 ? "Next draw" : $"In {remaining} draws")
                    : "Unavailable";
                m_Fill.fillAmount = available ? Mathf.Clamp01((float)count / threshold) : 0f;
            }
        }

        [System.Serializable]
        private sealed class RatesField
        {
            [SerializeField] private TextMeshProUGUI m_EntryCount;
            [SerializeField] private TextMeshProUGUI m_Note;
            [SerializeField] private ScrollRect m_Scroll;

            public void Bind(int count, string note)
            {
                m_EntryCount.text = count == 1 ? "1 entry" : $"{count} entries";
                m_Note.text = note;
                m_Note.gameObject.SetActive(!string.IsNullOrWhiteSpace(note));
                m_Note.transform.SetAsLastSibling();
                LayoutRebuilder.ForceRebuildLayoutImmediate(m_Scroll.content);
                m_Scroll.StopMovement();
                m_Scroll.verticalNormalizedPosition = 1f;
            }
        }

        [SerializeField] private BannerHeaderField m_Header;
        [SerializeField] private PityField m_SmallPity;
        [SerializeField] private PityField m_FinalPity;
        [SerializeField] private RatesField m_RateList;
        [SerializeField] private GachaRateItemPool m_RateItemPool;
        [SerializeField] private Button m_CloseButton;

        [SerializeField] private PanelView m_HomePanel;
        [SerializeField] private GachaPanel m_SummonPanel;
        private GachaBannerConfig m_Banner;
        private GachaManager Manager => RushPlayer.Instance.GachaManager;

        public void Show(GachaBannerConfig banner)
        {
            if (banner == null) return;
            m_Banner = banner;
            ShowInternal();
        }

        protected override void ShowInternal()
        {
            m_Banner ??= Manager.ActiveBanner;
            if (m_Banner == null) return;
            base.ShowInternal();
            if (!IsShown) return;
            UnsubscribeInternal();
            m_CloseButton.onClick.AddListener(Hide);
            m_HomePanel.OnHide.AddListener(Hide);
            m_SummonPanel.OnHide.AddListener(Hide);
            RefreshViewInternal();
        }

        protected override void HideInternal()
        {
            UnsubscribeInternal();
            m_RateItemPool.ReturnAll();
            m_Banner = null;
            base.HideInternal();
        }

        private void UnsubscribeInternal()
        {
            m_CloseButton.onClick.RemoveListener(Hide);
            m_HomePanel.OnHide.RemoveListener(Hide);
            m_SummonPanel.OnHide.RemoveListener(Hide);
        }

        private void RefreshViewInternal()
        {
            var rates = Manager.GetRateInfo(m_Banner);
            var featured = GachaRateInfo.SingleReward(m_Banner.FinalPityGuarantees);
            m_Header.Bind(m_Banner, featured, rates);

            var progress = Manager.PityTracker.ProgressFor(m_Banner);
            BindPityInternal(m_SmallPity, m_Banner.SmallPityGuarantees, progress.Small, m_Banner.SmallPityCount, "Small pity");
            BindPityInternal(m_FinalPity, m_Banner.FinalPityGuarantees, progress.Final, m_Banner.FinalPityCount, "Final pity");

            m_RateItemPool.ReturnAll();
            foreach (var rate in rates)
            {
                var row = m_RateItemPool.Rent();
                row.transform.SetAsLastSibling();
                row.Setup(rate, rate.Collectable.Collect == featured);
            }

            string note = m_Banner.BaseInfo.Description;
            if (!progress.FirstDone && m_Banner.HasFirstDrawGuarantee &&
                GachaRateInfo.TotalWeight(m_Banner.FirstDrawGuarantees) > 0f)
            {
                string first = GachaRateInfo.GuaranteeTitle(m_Banner.FirstDrawGuarantees, "Guaranteed reward");
                note = $"First draw: {first}.\n{note}".TrimEnd();
            }
            m_RateList.Bind(rates.Count, note);
        }

        private void BindPityInternal(PityField view, GachaCollectableConfig[] pool, int count, int threshold, string fallback)
        {
            bool available = threshold > 0 && GachaRateInfo.TotalWeight(pool) > 0f;
            view.Bind(GachaRateInfo.GuaranteeTitle(pool, fallback), count, threshold, available);
        }
    }
}
