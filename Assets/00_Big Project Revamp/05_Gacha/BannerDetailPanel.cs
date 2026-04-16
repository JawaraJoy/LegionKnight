using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using LegionKnight;

namespace Rush
{
    public class BannerDetailPanel : PanelView
    {
        [SerializeField] private TextMeshProUGUI m_BannerNameText;
        [SerializeField] private TextMeshProUGUI m_BannerDescText;
        [SerializeField] private Image m_BannerSplashImage;
        [SerializeField] private TextMeshProUGUI m_SmallPityText;
        [SerializeField] private TextMeshProUGUI m_FinalPityText;
        [SerializeField] private TextMeshProUGUI m_PityCountText;
        [SerializeField] private TextMeshProUGUI m_PityWindowText;
        [SerializeField] private GachaRateItemPool m_RateItemPool;
        [SerializeField] private Button m_CloseButton;

        private GachaBannerConfig m_Banner;
        private List<GachaRateInfo> m_Rates = new();

        protected override void ShowInternal()
        {
            base.ShowInternal();
            if (m_CloseButton != null) m_CloseButton.onClick.AddListener(Hide);
        }

        protected override void HideInternal()
        {
            if (m_CloseButton != null) m_CloseButton.onClick.RemoveListener(Hide);
            m_RateItemPool?.ReturnAll();
            base.HideInternal();
        }

        public void Show(GachaBannerConfig banner)
        {
            m_Banner = banner;
            RefreshViewInternal();
            Show();
        }

        private void RefreshViewInternal()
        {
            if (m_Banner == null) return;

            if (m_BannerNameText != null) m_BannerNameText.text = m_Banner.BaseInfo.Name;
            if (m_BannerDescText != null) m_BannerDescText.text = m_Banner.BaseInfo.Description;
            if (m_BannerSplashImage != null) m_BannerSplashImage.sprite = m_Banner.BannerSplashSprite;

            int smallLen = m_Banner.SmallPityGuarantees?.Length ?? 0;
            int finalLen = m_Banner.FinalPityGuarantees?.Length ?? 0;

            if (m_SmallPityText != null)
                m_SmallPityText.text = smallLen > 1
                    ? $"Small pity: draw {m_Banner.SmallPityCount - smallLen + 1}–{m_Banner.SmallPityCount}"
                    : $"Small pity: draw ke-{m_Banner.SmallPityCount}";

            if (m_FinalPityText != null)
                m_FinalPityText.text = finalLen > 1
                    ? $"Final pity: draw {m_Banner.FinalPityCount - finalLen + 1}–{m_Banner.FinalPityCount}"
                    : $"Final pity: draw ke-{m_Banner.FinalPityCount}";

            var pity = RushPlayer.Instance.GachaManager.PityTracker;
            if (m_PityCountText != null)
                m_PityCountText.text =
                    $"Pity saat ini: {pity.FinalPityCounter}/{m_Banner.FinalPityCount}";
            if (m_PityWindowText != null)
                m_PityWindowText.text = pity.IsInFinalPityWindow ? "Dalam final pity window!"
                    : pity.IsInSmallPityWindow ? "Dalam small pity window!" : "";

            PopulateRatesInternal();
        }

        private void PopulateRatesInternal()
        {
            if (m_RateItemPool == null) return;
            m_RateItemPool.ReturnAll();

            m_Rates = RushPlayer.Instance.GachaManager.GetRateInfo(m_Banner);
            foreach (var rate in m_Rates)
            {
                var item = m_RateItemPool.Rent();
                item.Setup(rate);
            }
        }
    }
}