using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Rush
{
    public sealed class DailySignInRewardCard : MonoBehaviour
    {
        [System.Serializable]
        private sealed class ArtworkField
        {
            [SerializeField] private Image m_Icon;
            [SerializeField] private Image m_Frame;

            internal void Bind(CollectibleConfig item)
            {
                var art = item.CollectibleField;
                m_Icon.sprite = art?.Icon;
                m_Icon.enabled = m_Icon.sprite != null;
                m_Frame.color = art?.RarityConfig != null ? art.RarityConfig.Color : Color.white;
            }
        }

        [System.Serializable]
        private sealed class LabelsField
        {
            [SerializeField] private TMP_Text m_ItemName;
            [SerializeField] private TMP_Text m_Quantity;
            [SerializeField] private TMP_Text m_Rarity;

            internal void Bind(CollectibleConfig item, int amount)
            {
                m_ItemName.text = item.BaseInfo.Name;
                m_Quantity.text = $"×{amount}";
                m_Rarity.text = item.CollectibleField?.RarityConfig?.BaseInfo.Name?.ToUpperInvariant() ?? string.Empty;
            }
        }

        [SerializeField] private ArtworkField m_Artwork;
        [SerializeField] private LabelsField m_Labels;

        internal void Bind(CollectibleResultEntry entry)
        {
            var conversion = entry.WasDuplicateHero && entry.Collectible is HeroUnitConfig hero
                ? hero.ItemDuplicateConverter : null;
            var converted = conversion?.ItemConfig != null;
            var item = converted ? (CollectibleConfig)conversion.ItemConfig : entry.Collectible;
            m_Artwork.Bind(item);
            m_Labels.Bind(item, converted ? conversion.Amount : entry.Amount);
        }
    }
}
