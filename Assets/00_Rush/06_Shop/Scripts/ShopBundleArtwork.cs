using UnityEngine;
using UnityEngine.UI;

namespace Rush
{
    public sealed class ShopBundleArtwork : MonoBehaviour
    {
        [SerializeField] private Image m_SingleFrame;
        [SerializeField] private Image m_SingleIcon;
        [SerializeField] private Image[] m_CardFrames;
        [SerializeField] private Image[] m_CardIcons;

        public void Bind(ShopBundleConfig bundle)
        {
            var entries = bundle.Entries;
            bool cards = entries != null && entries.Length > 0 && entries[0]?.Collectible is CardConfig;
            m_SingleFrame.gameObject.SetActive(!cards);
            if (!cards)
            {
                var collectible = entries != null && entries.Length > 0 ? entries[0]?.Collectible : null;
                m_SingleIcon.sprite = bundle.BundleSprite != null ? bundle.BundleSprite : collectible?.CollectibleField?.Icon;
                m_SingleIcon.enabled = m_SingleIcon.sprite != null;
                m_SingleFrame.color = collectible?.CollectibleField?.RarityConfig?.Color ?? Color.white;
            }
            for (int i = 0; i < m_CardFrames.Length; i++)
            {
                var entry = cards && i < entries.Length ? entries[i] : null;
                m_CardFrames[i].gameObject.SetActive(entry?.Collectible != null);
                if (entry?.Collectible == null) continue;
                var field = entry.Collectible.CollectibleField;
                m_CardFrames[i].color = field?.RarityConfig?.Color ?? Color.white;
                m_CardIcons[i].sprite = field?.Icon;
                m_CardIcons[i].enabled = m_CardIcons[i].sprite != null;
            }
        }
    }
}
