using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Rush
{
    public class GachaPreviewItemUI : MonoBehaviour
    {
        [SerializeField] private Image m_SplashImage;
        [SerializeField] private Image m_RarityFrame;
        [SerializeField] private TextMeshProUGUI m_NameText;
        [SerializeField] private TextMeshProUGUI m_AmountText;
        [SerializeField] private TextMeshProUGUI m_RarityText;
        [SerializeField] private Image m_RarityMark;
        [SerializeField] private TextMeshProUGUI m_DuplicateText;
        [SerializeField] private int m_FlipCount = 6;
        [SerializeField] private float m_FlipInterval = 2f;
        [SerializeField] private UnityEvent<bool> m_OnFlip;

        private Coroutine m_FlipCoroutine;

        private void OnDisable() => StopFlipInternal();

        public void Setup(CollectibleResultEntry entry) => BindInternal(entry, true);
        internal void PrepareForReveal(CollectibleResultEntry entry) => BindInternal(entry, false);

        private void BindInternal(CollectibleResultEntry entry, bool animateConversion)
        {
            StopFlipInternal();
            BindRewardInternal(entry.Collectible, entry.Amount, null);
            if (!animateConversion || !entry.WasDuplicateHero || !(entry.Collectible is HeroUnitConfig hero))
                return;

            var conversion = hero.ItemDuplicateConverter;
            if (conversion?.ItemConfig != null)
                m_FlipCoroutine = StartCoroutine(FlipRoutine(entry, conversion));
        }

        private IEnumerator FlipRoutine(CollectibleResultEntry entry, LegionKnight.Currency conversion)
        {
            // The player has just dismissed the hero animation. Show what was granted first.
            for (int i = 0; i < m_FlipCount; i++)
            {
                bool showHero = i % 2 == 1;
                if (showHero) BindRewardInternal(entry.Collectible, entry.Amount, null);
                else BindRewardInternal(conversion.ItemConfig, conversion.Amount, entry.Collectible.BaseInfo.Name);
                OnFlipInvoke(showHero);
                yield return new WaitForSecondsRealtime(m_FlipInterval);
            }

            BindRewardInternal(conversion.ItemConfig, conversion.Amount, entry.Collectible.BaseInfo.Name);
            OnFlipInvoke(false);
            m_FlipCoroutine = null;
        }

        private void BindRewardInternal(CollectibleConfig collectible, int amount, string duplicateHero)
        {
            var field = collectible.CollectibleField;
            m_SplashImage.sprite = field?.SplashImage != null ? field.SplashImage : field?.Icon;
            m_SplashImage.enabled = m_SplashImage.sprite != null;
            m_NameText.text = collectible.BaseInfo.Name;
            m_AmountText.text = $"×{amount}";
            var rarity = field?.RarityConfig;
            m_RarityFrame.color = rarity != null ? rarity.Color : Color.white;
            m_RarityText.text = rarity != null ? rarity.BaseInfo.Name.ToUpperInvariant() : string.Empty;
            m_RarityMark.enabled = rarity != null;
            if (rarity != null) m_RarityMark.color = rarity.Color;
            m_DuplicateText.text = duplicateHero == null ? string.Empty : $"Duplicate {duplicateHero}";
        }

        private void OnFlipInvoke(bool showHero) => m_OnFlip?.Invoke(showHero);

        private void StopFlipInternal()
        {
            if (m_FlipCoroutine == null) return;
            StopCoroutine(m_FlipCoroutine);
            m_FlipCoroutine = null;
        }
    }
}
