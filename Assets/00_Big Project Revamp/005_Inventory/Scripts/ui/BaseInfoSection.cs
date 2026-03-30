using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Rush
{
    /// <summary>
    /// Section yang menampilkan info dasar: nama, deskripsi, icon, splash image, rarity.
    /// Tampil untuk SEMUA tipe collectible — hero, card, maupun item.
    /// </summary>
    public class BaseInfoSection : CollectibleDetailSection
    {
        [SerializeField] private TextMeshProUGUI m_NameText;
        [SerializeField] private TextMeshProUGUI m_DescriptionText;
        [SerializeField] private Image           m_IconImage;
        [SerializeField] private Image           m_SplashImage;

        // Opsional: tampilkan rarity label/color jika ada
        [SerializeField] private TextMeshProUGUI m_RarityText;
        [SerializeField] private Image           m_RarityBadge;

        // ── CollectibleDetailSection ──────────────────────────────────
        public override bool IsRelevantFor(ICollectibleEntry entry)
        {
            // Selalu tampil untuk semua tipe
            return true;
        }

        protected override void OnBind(ICollectibleEntry entry)
        {
            if (m_NameText)        m_NameText.text        = entry.Name        ?? string.Empty;
            if (m_DescriptionText) m_DescriptionText.text = entry.Description ?? string.Empty;

            if (m_IconImage)
            {
                m_IconImage.sprite  = entry.Icon;
                m_IconImage.enabled = entry.Icon != null;
            }

            if (m_SplashImage)
            {
                m_SplashImage.sprite  = entry.SplashImage;
                m_SplashImage.enabled = entry.SplashImage != null;
            }

            if (entry.RarityConfig != null)
            {
                if (m_RarityText)  m_RarityText.text     = entry.RarityConfig.BaseInfo?.Name ?? string.Empty;
                if (m_RarityBadge) m_RarityBadge.enabled = true;
            }
            else
            {
                if (m_RarityText)  m_RarityText.text     = string.Empty;
                if (m_RarityBadge) m_RarityBadge.enabled = false;
            }
        }
    }
}
