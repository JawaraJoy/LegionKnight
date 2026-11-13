using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Rush
{
    [RequireComponent(typeof(RectTransform), typeof(CanvasGroup))]
    public sealed class ReviveBannerView : MonoBehaviour
    {
        [SerializeField] private RectTransform m_Banner;
        [SerializeField] private float m_ReferenceWidth = 1536f;
        [SerializeField] private TMP_Text m_HeroName;
        [SerializeField] private Image m_Portrait;
        [SerializeField] private Image[] m_RarityFrames;

        private void OnEnable()
        {
            FitBannerInternal();
            GetComponent<CanvasGroup>().alpha = 1f;
            var hero = RushPlayer.Instance.Unit?.Config;
            var art = hero?.CollectibleField;
            var rarity = art?.RarityConfig;
            m_HeroName.text = hero != null ? hero.BaseInfo.Name : string.Empty;
            m_Portrait.sprite = art?.Icon;
            m_Portrait.enabled = m_Portrait.sprite != null;
            foreach (var frame in m_RarityFrames)
                frame.color = rarity != null ? rarity.Color : Color.white;
        }

        private void OnRectTransformDimensionsChange() => FitBannerInternal();

        private void FitBannerInternal()
        {
            if (m_Banner == null || m_ReferenceWidth <= 0f) return;
            float scale = ((RectTransform)transform).rect.width / m_ReferenceWidth;
            if (scale > 0f) m_Banner.localScale = Vector3.one * scale;
        }
    }
}
