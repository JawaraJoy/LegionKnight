using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;

namespace Rush
{
    public class GachaPreviewItemUI : MonoBehaviour
    {
        [Header("Display")]
        [SerializeField] private Image m_SplashImage;
        [SerializeField] private Image m_RarityFrame;
        [SerializeField] private TextMeshProUGUI m_NameText;
        [SerializeField] private TextMeshProUGUI m_AmountText;
        [SerializeField] private TextMeshProUGUI m_RarityText;

        [Header("Flip Config")]
        [SerializeField] private int m_FlipCount = 6;
        [SerializeField] private float m_FlipInterval = 2f;

        [Header("Events")]
        // true = showing hero, false = showing converter
        [SerializeField] private UnityEvent<bool> m_OnFlip;

        private Coroutine m_FlipCoroutine;

        // Cached data for flip
        private Sprite m_HeroSplash;
        private string m_HeroName;
        private Sprite m_ConverterIcon;
        private string m_ConverterName;
        private string m_ConverterAmount;

        private void OnDisable() => StopFlipInternal();

        public void Setup(CollectibleResultEntry entry)
        {
            StopFlipInternal();

            var collectible = entry.Collectible;
            var field = collectible.CollectibleField;

            // Set normal display first
            if (m_SplashImage != null) m_SplashImage.sprite = field?.SplashImage;
            if (m_NameText != null) m_NameText.text = collectible.BaseInfo.Name;

            if (m_AmountText != null)
            {
                bool show = entry.Amount >= 2;
                m_AmountText.gameObject.SetActive(show);
                if (show) m_AmountText.text = $"x{entry.Amount}";
            }

            RefreshRarityInternal(field);

            // Start flip if duplicate hero
            if (GachaDuplicateHelper.IsDuplicateHero(collectible, out var heroConfig))
            {
                var converter = heroConfig.ItemDuplicateConverter;

                m_HeroSplash = field?.SplashImage;
                m_HeroName = collectible.BaseInfo.Name;
                m_ConverterIcon = converter.ItemConfig?.CollectibleField?.Icon;
                m_ConverterName = converter.ItemConfig?.BaseInfo.Name ?? "-";
                m_ConverterAmount = $"x{converter.Amount}";

                m_FlipCoroutine = StartCoroutine(FlipRoutine());
            }
        }

        // ── Flip ──────────────────────────────────────────────────────────────

        private IEnumerator FlipRoutine()
        {
            for (int i = 0; i < m_FlipCount; i++)
            {
                bool showHero = i % 2 == 0;
                ApplyFlipInternal(showHero);
                m_OnFlip?.Invoke(showHero);
                yield return new WaitForSeconds(m_FlipInterval);
            }

            // End on converter
            ApplyFlipInternal(false);
            m_OnFlip?.Invoke(false);
            m_FlipCoroutine = null;
        }

        private void ApplyFlipInternal(bool showHero)
        {
            if (showHero)
            {
                if (m_SplashImage != null) m_SplashImage.sprite = m_HeroSplash;
                if (m_NameText != null) m_NameText.text = m_HeroName;
                if (m_AmountText != null) m_AmountText.gameObject.SetActive(false);
            }
            else
            {
                if (m_SplashImage != null) m_SplashImage.sprite = m_ConverterIcon;
                if (m_NameText != null) m_NameText.text = m_ConverterName;
                if (m_AmountText != null)
                {
                    m_AmountText.gameObject.SetActive(true);
                    m_AmountText.text = m_ConverterAmount;
                }
            }
        }

        private void StopFlipInternal()
        {
            if (m_FlipCoroutine == null) return;
            StopCoroutine(m_FlipCoroutine);
            m_FlipCoroutine = null;
        }

        private void RefreshRarityInternal(CollectibleField field)
        {
            var rarity = field?.RarityConfig;
            if (rarity != null)
            {
                if (m_RarityText != null)
                {
                    m_RarityText.gameObject.SetActive(true);
                    m_RarityText.text = rarity.BaseInfo.Name;
                    m_RarityText.color = rarity.Color;
                }
                if (m_RarityFrame != null)
                {
                    m_RarityFrame.gameObject.SetActive(true);
                    m_RarityFrame.color = rarity.Color;
                }
            }
            else
            {
                if (m_RarityText != null) m_RarityText.gameObject.SetActive(false);
                if (m_RarityFrame != null) m_RarityFrame.gameObject.SetActive(false);
            }
        }
    }
}