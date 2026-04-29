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

            // Pity hanya trigger tepat di draw ke-N, tidak ada window
            if (m_SmallPityText != null)
                m_SmallPityText.text = $"Small pity: draw -{m_Banner.SmallPityCount}";

            if (m_FinalPityText != null)
                m_FinalPityText.text = $"Final pity: draw -{m_Banner.FinalPityCount}";

            var pity = RushPlayer.Instance.GachaManager.PityTracker;
            if (m_PityCountText != null)
                m_PityCountText.text =
                    $"Current Pity: {pity.FinalPityCounter}/{m_Banner.FinalPityCount}";

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